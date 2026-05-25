using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Contracts.Queries.Snapshots;
using HeroService.Domain.Heroes.Core;
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
  private readonly SnapshotResolver _resolver;
  private readonly IFranzMapper _mapper;

  public GetHeroSnapshotQueryHandler(
      IEntityRepository<Hero, Guid> heroRepository,
      ISkillRepository skillRepository,
      ISkillBaseStatsRepository skillBaseStatsRepository,
      ISkillModifierRepository skillModifierRepository,
      IHeroModifierRepository heroModifierRepository,
      SnapshotResolver resolver,
      IFranzMapper mapper)
  {
    _heroRepository = heroRepository;
    _skillRepository = skillRepository;
    _skillBaseStatsRepository = skillBaseStatsRepository;
    _skillModifierRepository = skillModifierRepository;
    _heroModifierRepository = heroModifierRepository;
    _resolver = resolver;
    _mapper = mapper;
  }

  public async Task<HeroSnapshotDto> Handle(
      GetHeroSnapshotQuery request,
      CancellationToken ct)
  {
    // 1. Hero (source of truth)
    var hero = await _heroRepository.GetByIdAsync(request.HeroId, ct)
        ?? throw new InvalidOperationException($"Hero '{request.HeroId}' not found.");

    // 2. Modifier (versioned)
    var heroModifier = await _heroModifierRepository.GetAsync(
        request.HeroId,
        request.GameVersionId,
        ct);

    // 3. Skills from hero aggregate
    var skillKit = hero.SkillKit;

    if (skillKit is null)
      throw new InvalidOperationException($"Hero '{hero.Id}' has no SkillKit defined.");

    var skillIds = new[]
    {
    skillKit.PassiveSkillId,
    skillKit.PrimarySkillId,
    skillKit.SecondarySkillId,
    skillKit.TertiarySkillId,
    skillKit.UltimateSkillId
    }
    .Where(id => id != Guid.Empty)
    .ToList();

    if (skillIds.Count == 0)
      throw new InvalidOperationException($"Hero '{hero.Id}' has no skills defined.");

    var skills = await _skillRepository.GetByIdsAsync(skillIds, ct);

    var skillBaseStats =
        await _skillBaseStatsRepository.GetBySkillIdsAsync(skillIds, ct);

    var skillModifiers =
        await _skillModifierRepository.GetBySkillIdsAsync(skillIds, ct);

    // 5. Resolve snapshot (pure deterministic engine)
    var snapshot = _resolver.ResolveHero(
        hero.Id,
        request.GameVersionId,
        hero.BaseStats,
        heroModifier,
        skills,
        skillBaseStats,
        skillModifiers
    );

    // 6. PURE mapping layer (FIX)
    return _mapper.Map<HeroSnapshot, HeroSnapshotDto>(snapshot);
  }
}