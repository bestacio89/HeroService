using Franz.Common.Caching.Abstractions;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;

using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Contracts.Queries.Snapshots;

using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;

namespace HeroService.Application.Heroes.Versioned.Snapshotting.Queries;


public sealed class GetHeroSnapshotQueryHandler
    : IQueryHandler<GetHeroSnapshotQuery, HeroSnapshotDto>
{
  private readonly IHeroRepository _heroRepository;
  private readonly ISkillRepository _skillRepository;
  private readonly ISkillBaseStatsRepository _skillBaseStatsRepository;
  private readonly ISkillModifierRepository _skillModifierRepository;
  private readonly IHeroModifierRepository _heroModifierRepository;
  private readonly IGameVersionRepository _gameVersionRepository;

  private readonly SnapshotResolver _resolver;
  private readonly IFranzMapper _mapper;
  private readonly ICacheProvider _cache;


  public GetHeroSnapshotQueryHandler(
      IHeroRepository heroRepository,
      ISkillRepository skillRepository,
      ISkillBaseStatsRepository skillBaseStatsRepository,
      ISkillModifierRepository skillModifierRepository,
      IHeroModifierRepository heroModifierRepository,
      IGameVersionRepository gameVersionRepository,
      SnapshotResolver resolver,
      IFranzMapper mapper,
      ICacheProvider cache)
  {
    _heroRepository = heroRepository;
    _skillRepository = skillRepository;
    _skillBaseStatsRepository = skillBaseStatsRepository;
    _skillModifierRepository = skillModifierRepository;
    _heroModifierRepository = heroModifierRepository;
    _gameVersionRepository = gameVersionRepository;

    _resolver = resolver;
    _mapper = mapper;
    _cache = cache;
  }



  public async Task<HeroSnapshotDto> Handle(
      GetHeroSnapshotQuery request,
      CancellationToken ct)
  {
    var key =
        HeroSnapshotCaching.SingleActiveKey(
            request.HeroId);


    var result =
        await _cache.GetOrSetAsync(
            key,
            ct => ResolveAsync(request, ct),
            options: null,
            ct);


    return result.Value;
  }



  private async Task<HeroSnapshotDto> ResolveAsync(
      GetHeroSnapshotQuery request,
      CancellationToken ct)
  {
    var activeVersion =
        await _gameVersionRepository.GetActiveAsync(ct)
        ?? throw new InvalidOperationException(
            "No active game version exists.");



    var hero =
        await _heroRepository.GetDetailsAsync(
            request.HeroId,
            ct)
        ?? throw new InvalidOperationException(
            $"Hero '{request.HeroId}' not found.");



    var heroModifier =
        await _heroModifierRepository.GetByHeroAndVersionAsync(
            hero.Id,
            activeVersion.Id,
            ct);



    if (heroModifier is null)
    {
      throw new InvalidOperationException(
          $"Hero '{hero.Name}' has no modifier for active version '{activeVersion.Id}'.");
    }



    Guid[] skillIds =
    [
        hero.SkillKit.PassiveSkillId,
            hero.SkillKit.PrimarySkillId,
            hero.SkillKit.SecondarySkillId,
            hero.SkillKit.TertiarySkillId,
            hero.SkillKit.UltimateSkillId
    ];



    var skills =
        await _skillRepository.GetByIdsAsync(
            skillIds,
            ct);



    if (skills.Count != 5)
    {
      throw new InvalidOperationException(
          $"Hero '{hero.Name}' does not have a complete skill kit.");
    }



    var skillBaseStats =
        await _skillBaseStatsRepository.GetBySkillIdsAsync(
            skillIds,
            ct);



    var skillModifiers =
        await _skillModifierRepository
            .GetBySkillIdsAndVersionAsync(
                skillIds,
                activeVersion.Id,
                ct);



    var skillModifierLookup =
        skillModifiers.ToDictionary(
            x => x.SkillId);



    foreach (var skillId in skillIds)
    {
      if (!skillModifierLookup.ContainsKey(skillId))
      {
        throw new InvalidOperationException(
            $"Skill '{skillId}' has no modifier for active version '{activeVersion.Id}'.");
      }
    }



    var snapshot =
        _resolver.ResolveHero(
            hero.Id,
            hero.Name,
            activeVersion,
            hero.BaseStats,
            heroModifier,
            skills,
            skillBaseStats.ToDictionary(
                x => x.Key,
                x => x.Value),
            skillModifierLookup);



    return _mapper.Map<HeroSnapshot, HeroSnapshotDto>(
        snapshot);
  }
}