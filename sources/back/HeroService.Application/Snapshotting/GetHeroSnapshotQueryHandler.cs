using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Contracts.Queries.Snapshots;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;

namespace HeroService.Application.Heroes.Versioned.Snapshotting.Queries;

public sealed class GetHeroSnapshotQueryHandler
    : IQueryHandler<GetHeroSnapshotQuery, HeroSnapshotDto>
{
  private readonly IEntityRepository<Hero, Guid> _heroRepository;
  private readonly ISkillRepository _skillRepository;
  private readonly ISkillBaseStatsRepository _skillBaseStatsRepository;
  private readonly ISkillModifierRepository _skillModifierRepository;
  private readonly IHeroModifierRepository _heroModifierRepository;
  private readonly IGameVersionRepository _gVersionRepository;
  private readonly SnapshotResolver _resolver;
  private readonly IFranzMapper _mapper;

  public GetHeroSnapshotQueryHandler(
      IEntityRepository<Hero, Guid> heroRepository,
      ISkillRepository skillRepository,
      ISkillBaseStatsRepository skillBaseStatsRepository,
      ISkillModifierRepository skillModifierRepository,
      IHeroModifierRepository heroModifierRepository,
      SnapshotResolver resolver,
      IFranzMapper mapper,
      IGameVersionRepository gversion)
  {
    _heroRepository = heroRepository;
    _skillRepository = skillRepository;
    _skillBaseStatsRepository = skillBaseStatsRepository;
    _skillModifierRepository = skillModifierRepository;
    _heroModifierRepository = heroModifierRepository;
    _resolver = resolver;
    _mapper = mapper;
    _gVersionRepository = gversion;
  }

  public async Task<HeroSnapshotDto> Handle(
    GetHeroSnapshotQuery request,
    CancellationToken ct)
  {
    var hero = await _heroRepository.GetByIdAsync(request.HeroId, ct)
        ?? throw new InvalidOperationException(
            $"Hero '{request.HeroId}' not found.");

    var heroModifier =
        await _heroModifierRepository.GetByHeroAndVersionAsync(
            request.HeroId,
            request.GameVersionId,
            ct);

    var skillIds = new[]
    {
        hero.SkillKit.PassiveSkillId,
        hero.SkillKit.PrimarySkillId,
        hero.SkillKit.SecondarySkillId,
        hero.SkillKit.TertiarySkillId,
        hero.SkillKit.UltimateSkillId
    };

    var skills =
        await _skillRepository.GetByIdsAsync(skillIds, ct);

    var skillBaseStats =
        await _skillBaseStatsRepository.GetBySkillIdsAsync(
            skillIds,
            ct);

    var skillModifiers =
        await _skillModifierRepository.GetBySkillIdsAndVersionAsync(
            skillIds,
            request.GameVersionId,
            ct);

    var skillModifierLookup =
        skillModifiers.ToDictionary(
            x => x.SkillId,
            x => x);

    var snapshot = _resolver.ResolveHero(
        hero.Id,
        request.GameVersionId,
        hero.BaseStats,
        heroModifier,
        skills,
        skillBaseStats,
        skillModifierLookup);

    return _mapper.Map<HeroSnapshot, HeroSnapshotDto>(snapshot);
  }
}