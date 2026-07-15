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


public sealed class BrowseHeroSnapshotsQueryHandler
    : IQueryHandler<BrowseHeroSnapshotsQuery, IReadOnlyList<HeroSnapshotDto>>
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


  public BrowseHeroSnapshotsQueryHandler(
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



  public async Task<IReadOnlyList<HeroSnapshotDto>> Handle(
      BrowseHeroSnapshotsQuery request,
      CancellationToken ct)
  {
    var key = HeroSnapshotCaching.BrowseActiveKey();


    var result = await _cache.GetOrSetAsync(
        key,
        ResolveAsync,
        options: null,
        ct);


    return result.Value;
  }



  private async Task<IReadOnlyList<HeroSnapshotDto>> ResolveAsync(
      CancellationToken ct)
  {
    var version =
        await _gameVersionRepository.GetActiveAsync(ct)
        ?? throw new InvalidOperationException(
            "No active game version exists.");



    var heroes =
        await _heroRepository.GetAllWithDetailsAsync(ct);


    if (heroes.Count == 0)
      return Array.Empty<HeroSnapshotDto>();



    var skillIds =
    heroes
        .SelectMany(hero => new[]
        {
            hero.SkillKit.PassiveSkillId,
            hero.SkillKit.PrimarySkillId,
            hero.SkillKit.SecondarySkillId,
            hero.SkillKit.TertiarySkillId,
            hero.SkillKit.UltimateSkillId
        })
        .Distinct()
        .ToList();



    var skills =
        await _skillRepository.GetByIdsAsync(
            skillIds,
            ct);



    var baseStats =
        await _skillBaseStatsRepository.GetBySkillIdsAsync(
            skillIds,
            ct);



    var skillModifiers =
        await _skillModifierRepository
            .GetBySkillIdsAndVersionAsync(
                skillIds,
                version.Id,
                ct);



    var heroModifiers =
        await _heroModifierRepository
            .GetByGameVersionIdAsync(
                version.Id,
                ct);



    var skillLookup =
        skills.ToDictionary(
            x => x.Id);



    var skillStatsLookup =
        baseStats.ToDictionary(
            x => x.Key,
            x => x.Value);



    var skillModifierLookup =
        skillModifiers.ToDictionary(
            x => x.SkillId);



    var heroModifierLookup =
        heroModifiers.ToDictionary(
            x => x.HeroId);



    var result =
        new List<HeroSnapshotDto>(
            heroes.Count);



    foreach (var hero in heroes)
    {
      Guid [] ids =
      [
          hero.SkillKit.PassiveSkillId,
                hero.SkillKit.PrimarySkillId,
                hero.SkillKit.SecondarySkillId,
                hero.SkillKit.TertiarySkillId,
                hero.SkillKit.UltimateSkillId
      ];



      var heroSkills =
          ids
              .Select(id =>
              {
                if (!skillLookup.TryGetValue(id, out var skill))
                {
                  throw new InvalidOperationException(
                            $"Hero '{hero.Name}' references missing skill '{id}'.");
                }

                return skill;

              })
              .ToList();



      if (!heroModifierLookup.TryGetValue(
              hero.Id,
              out var heroModifier))
      {
        throw new InvalidOperationException(
            $"Hero '{hero.Name}' has no modifier for active version '{version.VersionName}'.");
      }



      var snapshot =
          _resolver.ResolveHero(
              hero.Id,
              hero.Name,
              version,
              hero.BaseStats,
              heroModifier,
              heroSkills,
              skillStatsLookup,
              skillModifierLookup);



      result.Add(
          _mapper.Map<HeroSnapshot, HeroSnapshotDto>(
              snapshot));
    }


    return result;
  }
}