using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Contracts.Persistence.Heroes;
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
  private readonly IHeroRepository _heroRepository;
  private readonly ISkillRepository _skillRepository;
  private readonly ISkillBaseStatsRepository _skillBaseStatsRepository;
  private readonly ISkillModifierRepository _skillModifierRepository;
  private readonly IHeroModifierRepository _heroModifierRepository;
  private readonly SnapshotResolver _resolver;
  private readonly IFranzMapper _mapper;

  public BrowseHeroSnapshotsQueryHandler(
      IHeroRepository heroRepository,
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
    var heroes = await _heroRepository.GetAllWithDetailsAsync(ct);
    if (heroes.Count == 0) return Array.Empty<HeroSnapshotDto>();

    var allSkillIds = heroes.SelectMany(h => new[]
    {
            h.SkillKit.PassiveSkillId, h.SkillKit.PrimarySkillId,
            h.SkillKit.SecondarySkillId, h.SkillKit.TertiarySkillId,
            h.SkillKit.UltimateSkillId
        }).Distinct().ToList();

    // Fetch data
    var skills = await _skillRepository.GetByIdsAsync(allSkillIds, ct);
    var skillBaseStats = await _skillBaseStatsRepository.GetBySkillIdsAsync(allSkillIds, ct);
    var skillModifiers = await _skillModifierRepository.GetBySkillIdsAndVersionAsync(allSkillIds, request.GameVersionId, ct);
    var heroModifiers = await _heroModifierRepository.GetByGameVersionIdAsync(request.GameVersionId, ct);

    // Build dictionaries compatible with IReadOnlyDictionary
    var skillLookup = skills.ToDictionary(x => x.Id);

    // Convert to Dictionary<Guid, SkillBaseStats> to satisfy IReadOnlyDictionary
    var skillBaseStatsLookup = skillBaseStats.ToDictionary(x => x.Key, x => x.Value);

    // Convert to Dictionary<Guid, SkillModifier> 
    // Note: Using a group-first approach or Ensure unique if your logic allows
    var skillModifierLookup = skillModifiers.ToDictionary(x => x.SkillId, x => x);

    var heroModifierLookup = heroModifiers.ToDictionary(x => x.HeroId, x => x);

    var result = new List<HeroSnapshotDto>(heroes.Count);

    foreach (var hero in heroes)
    {
      var kit = hero.SkillKit;
      var heroSkillIds = new[] { kit.PassiveSkillId, kit.PrimarySkillId, kit.SecondarySkillId, kit.TertiarySkillId, kit.UltimateSkillId };

      var heroSkills = heroSkillIds
          .Where(skillLookup.ContainsKey)
          .Select(id => skillLookup[id])
          .ToList();

      var snapshot = _resolver.ResolveHero(
          hero.Id,
          request.GameVersionId,
          hero.BaseStats,
          heroModifierLookup.GetValueOrDefault(hero.Id),
          heroSkills,
          skillBaseStatsLookup,
          skillModifierLookup
      );

      result.Add(_mapper.Map<HeroSnapshot, HeroSnapshotDto>(snapshot));
    }

    return result;
  }
}