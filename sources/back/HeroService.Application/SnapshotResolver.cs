using Franz.Common.Business.Repositories;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Heroes.Versioned.Snapshotting;

namespace HeroService.Application.Heroes.Versioned.Snapshotting;

public class SnapshotResolver
{
  private readonly IEntityRepository<Hero, Guid> _heroes;
  private readonly IEntityRepository<Skill, Guid> _skills;

  private readonly IEntityRepository<HeroBaseStats, Guid> _heroBaseStats;
  private readonly IEntityRepository<SkillBaseStats, Guid> _skillBaseStats;

  private readonly IEntityRepository<HeroModifier, Guid> _heroModifiers;
  private readonly IEntityRepository<SkillModifier, Guid> _skillModifiers;

  public SnapshotResolver(
    IEntityRepository<Hero, Guid> heroes,
    IEntityRepository<Skill, Guid> skills,
    IEntityRepository<HeroBaseStats, Guid> heroBaseStats,
    IEntityRepository<SkillBaseStats, Guid> skillBaseStats,
    IEntityRepository<HeroModifier, Guid> heroModifiers,
    IEntityRepository<SkillModifier, Guid> skillModifiers)
  {
    _heroes = heroes;
    _skills = skills;
    _heroBaseStats = heroBaseStats;
    _skillBaseStats = skillBaseStats;
    _heroModifiers = heroModifiers;
    _skillModifiers = skillModifiers;
  }

  public async Task<HeroSnapshot> ResolveHero(Guid heroId, Guid gameVersionId, CancellationToken ct = default)
  {
    var hero = await _heroes.GetByIdAsync(heroId, ct)
      ?? throw new Exception($"Hero {heroId} not found");

    var baseStats = await _heroBaseStats.GetByIdAsync(heroId, ct)
      ?? throw new Exception($"HeroBaseStats for {heroId} not found");

    var heroModifier = await _heroModifiers.GetByIdAsync(heroId, ct);

    var skills = await _skills.GetByIdAsync(heroId, ct)
      ?? throw new Exception($"Skills for Hero {heroId} not found");

    var skillSnapshots = new List<SkillSnapshot>();

    foreach (var skill in skills.Effects.Select(e => skill)) // adapt if your structure differs
    {
      var baseSkill = await _skillBaseStats.GetByIdAsync(skill.Id, ct)
        ?? throw new Exception($"SkillBaseStats for {skill.Id} not found");

      var modifier = await _skillModifiers.GetByIdAsync(skill.Id, ct);

      skillSnapshots.Add(
        new SkillSnapshot(
          skill.Id,
          gameVersionId,
          Apply(baseSkill.BaseCooldown, modifier?.CooldownDelta),
          Apply(baseSkill.BaseManaCost, modifier?.ManaCostDelta),
          Apply(baseSkill.BaseDamage, modifier?.DamageMultiplier)
        )
      );
    }

    return new HeroSnapshot(
      heroId,
      gameVersionId,
      Apply(baseStats.BaseHealth, heroModifier?.HealthMultiplier),
      Apply(baseStats.BaseMana, heroModifier?.ManaMultiplier),
      Apply(baseStats.BaseAttackDamage, heroModifier?.DamageMultiplier),
      skillSnapshots
    );
  }

  // ------------------------
  // Snapshot math helpers
  // ------------------------

  private static float Apply(float baseValue, float? multiplier)
    => baseValue * (multiplier ?? 1f);

  private static float ApplyDelta(float baseValue, float? delta)
    => baseValue + (delta ?? 0f);
}