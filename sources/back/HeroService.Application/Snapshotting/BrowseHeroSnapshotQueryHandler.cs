using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Contracts.Queries.Snapshots;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;

namespace HeroService.Application.Heroes.Versioned.Snapshotting.Queries;

public sealed class BrowseHeroSnapshotsQueryHandler
    : IQueryHandler<BrowseHeroSnapshotsQuery, IReadOnlyList<HeroSnapshotDto>>
{
  private readonly IEntityRepository<Hero, Guid> _heroRepository;
  private readonly ISkillRepository _skillRepository;
  private readonly ISkillBaseStatsRepository _skillBaseStatsRepository;
  private readonly ISkillModifierRepository _skillModifierRepository;
  private readonly IHeroModifierRepository _heroModifierRepository;
  private readonly SnapshotResolver _resolver;
  private readonly IFranzMapper _mapper;

  public BrowseHeroSnapshotsQueryHandler(
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

  public async Task<IReadOnlyList<HeroSnapshotDto>> Handle(
      BrowseHeroSnapshotsQuery request,
      CancellationToken ct)
  {
    // 1. Load heroes
    var heroes = await _heroRepository.GetAllAsync(ct);

    if (heroes.Count == 0)
      return Array.Empty<HeroSnapshotDto>();

    // 2. Extract ALL skill IDs from fixed SkillKit (5-slot structure)
    var allSkillIds = new HashSet<Guid>();

    foreach (var hero in heroes)
    {
      var kit = hero.SkillKit;

      allSkillIds.Add(kit.PassiveSkillId);
      allSkillIds.Add(kit.PrimarySkillId);
      allSkillIds.Add(kit.SecondarySkillId);
      allSkillIds.Add(kit.TertiarySkillId);
      allSkillIds.Add(kit.UltimateSkillId);
    }

    var skillIdList = allSkillIds.ToList();

    // 3. Batch load projections
    var skills = await _skillRepository.GetByIdsAsync(skillIdList, ct);

    var skillBaseStats =
        await _skillBaseStatsRepository.GetBySkillIdsAsync(skillIdList, ct);

    var skillModifiers =
        await _skillModifierRepository.GetBySkillIdsAndVersionAsync(
            skillIdList,
            request.GameVersionId,
            ct);

    var heroModifiers =
        await _heroModifierRepository.GetByGameVersionIdAsync(
            request.GameVersionId,
            ct);

    // 4. Build lookups (CRITICAL FIX SECTION)

    var skillLookup =
        skills.ToDictionary(x => x.Id);

    var heroModifierLookup =
        heroModifiers.ToDictionary(x => x.HeroId);

    var skillModifierLookup =
        skillModifiers.ToDictionary(x => x.SkillId, x => x);

    var skillBaseStatsLookup =
    skillBaseStats.ToDictionary(
        x => x.Key,
        x => x.Value);

    // 5. Resolve snapshots
    var result = new List<HeroSnapshotDto>(heroes.Count);

    foreach (var hero in heroes)
    {
      var kit = hero.SkillKit;

      var heroSkillIds = new[]
      {
                kit.PassiveSkillId,
                kit.PrimarySkillId,
                kit.SecondarySkillId,
                kit.TertiarySkillId,
                kit.UltimateSkillId
            };

      // No LINQ scanning — direct lookup usage
      var heroSkills = new List<Skill>(5);
      var heroBaseStats = new List<SkillBaseStats>(5);
      var heroSkillModifiers = new Dictionary<Guid, SkillModifier>(5);

      foreach (var skillId in heroSkillIds)
      {
        if (skillLookup.TryGetValue(skillId, out var skill))
          heroSkills.Add(skill);

        if (skillBaseStatsLookup.TryGetValue(skillId, out var baseStats))
          heroBaseStats.Add(baseStats);

        if (skillModifierLookup.TryGetValue(skillId, out var modifier))
          heroSkillModifiers[skillId] = modifier;
      }

      heroModifierLookup.TryGetValue(hero.Id, out var heroModifier);

      // 6. Resolve deterministic snapshot
      var snapshot = _resolver.ResolveHero(
          hero.Id,
          request.GameVersionId,
          hero.BaseStats,
          heroModifier,
          heroSkills,
          skillBaseStatsLookup,
          heroSkillModifiers
      );

      result.Add(
          _mapper.Map<HeroSnapshot, HeroSnapshotDto>(snapshot));
    }

    return result;
  }
}