using Franz.Common.Mapping.Abstractions;
using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;

namespace HeroService.Application.Heroes.Versioned.Snapshotting;

/// <summary>
/// Resolves one or more hero snapshots from a single batched set of skill,
/// base-stat, and versioned modifier reads.
/// </summary>
public sealed class HeroSnapshotResolutionService
{
  private readonly ISkillRepository _skillRepository;
  private readonly ISkillBaseStatsRepository _skillBaseStatsRepository;
  private readonly ISkillModifierRepository _skillModifierRepository;
  private readonly SnapshotResolver _resolver;
  private readonly IFranzMapper _mapper;

  public HeroSnapshotResolutionService(
      ISkillRepository skillRepository,
      ISkillBaseStatsRepository skillBaseStatsRepository,
      ISkillModifierRepository skillModifierRepository,
      SnapshotResolver resolver,
      IFranzMapper mapper)
  {
    _skillRepository = skillRepository;
    _skillBaseStatsRepository = skillBaseStatsRepository;
    _skillModifierRepository = skillModifierRepository;
    _resolver = resolver;
    _mapper = mapper;
  }

  public async Task<IReadOnlyList<HeroSnapshotDto>> ResolveAsync(
      IReadOnlyCollection<Hero> heroes,
      GameVersion activeVersion,
      IReadOnlyCollection<HeroModifier> heroModifiers,
      CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(heroes);
    ArgumentNullException.ThrowIfNull(activeVersion);
    ArgumentNullException.ThrowIfNull(heroModifiers);

    if (heroes.Count == 0)
      return Array.Empty<HeroSnapshotDto>();

    var skillIdsByHero = heroes.ToDictionary(
        hero => hero.Id,
        GetSkillIds);

    var skillIds = skillIdsByHero.Values
        .SelectMany(ids => ids)
        .Distinct()
        .ToArray();

    var skills = await _skillRepository.GetByIdsAsync(
        skillIds,
        cancellationToken);

    var skillLookup = skills.ToDictionary(skill => skill.Id);
    var skillBaseStats = await _skillBaseStatsRepository.GetBySkillIdsAsync(
        skillIds,
        cancellationToken);
    var skillModifiers = await _skillModifierRepository.GetBySkillIdsAndVersionAsync(
        skillIds,
        activeVersion.Id,
        cancellationToken);

    var skillModifierLookup = skillModifiers.ToDictionary(
        modifier => modifier.SkillId);
    var heroModifierLookup = heroModifiers.ToDictionary(
        modifier => modifier.HeroId);

    var snapshots = new List<HeroSnapshotDto>(heroes.Count);

    foreach (var hero in heroes)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var heroSkills = skillIdsByHero[hero.Id]
          .Select(skillId =>
          {
            if (!skillLookup.TryGetValue(skillId, out var skill))
            {
              throw new InvalidOperationException(
                  $"Hero '{hero.Name}' references missing skill '{skillId}'.");
            }

            return skill;
          })
          .ToArray();

      if (!heroModifierLookup.TryGetValue(hero.Id, out var heroModifier))
      {
        throw new InvalidOperationException(
            $"Hero '{hero.Name}' has no modifier for active version '{activeVersion.VersionName}'.");
      }

      var snapshot = _resolver.ResolveHero(
          hero.Id,
          hero.Name,
          activeVersion,
          hero.BaseStats,
          heroModifier,
          heroSkills,
          skillBaseStats,
          skillModifierLookup);

      snapshots.Add(_mapper.Map<HeroSnapshot, HeroSnapshotDto>(snapshot));
    }

    return snapshots;
  }

  private static Guid[] GetSkillIds(Hero hero)
  {
    var kit = hero.SkillKit
        ?? throw new InvalidOperationException(
            $"Hero '{hero.Name}' does not have a skill kit.");

    return
    [
      kit.PassiveSkillId,
      kit.PrimarySkillId,
      kit.SecondarySkillId,
      kit.TertiarySkillId,
      kit.UltimateSkillId
    ];
  }
}
