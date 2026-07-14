using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

public sealed class SkillSeeder : ISeeder
{
  public int Order => 3;

  private readonly IEntityFactory<Guid, Skill> _skillFactory;
  private readonly IEntityFactory<Guid, SkillEffect> _effectFactory;
  private readonly IEntityFactory<Guid, SkillLore> _loreFactory;
  private readonly IEntityFactory<Guid, SkillBaseStats> _statsFactory;

  private readonly IEntityRepository<Skill, Guid> _skills;
  private readonly IEntityRepository<SkillEffect, Guid> _effects;
  private readonly IEntityRepository<SkillLore, Guid> _lore;
  private readonly IEntityRepository<SkillBaseStats, Guid> _stats;

  private readonly IUnitOfWork _unitOfWork;

  public SkillSeeder(
      IEntityFactory<Guid, Skill> skillFactory,
      IEntityFactory<Guid, SkillEffect> effectFactory,
      IEntityFactory<Guid, SkillLore> loreFactory,
      IEntityFactory<Guid, SkillBaseStats> statsFactory,
      IEntityRepository<Skill, Guid> skills,
      IEntityRepository<SkillEffect, Guid> effects,
      IEntityRepository<SkillLore, Guid> lore,
      IEntityRepository<SkillBaseStats, Guid> stats,
      IUnitOfWork unitOfWork)
  {
    _skillFactory = skillFactory;
    _effectFactory = effectFactory;
    _loreFactory = loreFactory;
    _statsFactory = statsFactory;

    _skills = skills;
    _effects = effects;
    _lore = lore;
    _stats = stats;

    _unitOfWork = unitOfWork;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _skills.GetAllAsync(ct)).Any())
      return;

    const string system = "seed-system";

    // =====================================================
    // THOR
    // =====================================================

    await CreateAsync(ct, system,
        name: "Mjolnir Strike",
        type: SkillType.Damage,
        loreBrief: "Thor crushes a single enemy with divine hammer force.",
        loreDesc: "A lightning-charged hammer impact from above.",
        baseStats: new BaseStatsParams(
            Cooldown: 6f,
            ManaCost: 50f,
            Damage: 150f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CC: 0.75f,
            Range: 4f),
        effects: new[]
        {
            Effect(EffectType.Damage, 150f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.4f, isInstant: true),
            Effect(EffectType.Stun, 0f, 0.75f, 0f, TargetType.Enemy, StackType.None, 1, isInstant: false),
        });

    await CreateAsync(ct, system,
        name: "Thunder Leap",
        type: SkillType.Mobility,
        loreBrief: "Thor leaps and shatters the ground upon landing.",
        loreDesc: "Shockwave of lightning expands outward.",
        baseStats: new BaseStatsParams(
            Cooldown: 10f,
            ManaCost: 40f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0f,
            ChannelDuration: null,
            CC: 1.2f,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None),
            Effect(EffectType.ZoneControl, 80f, 1.2f, 4f, TargetType.AreaEnemies, StackType.None, adRatio: 0.6f, isInstant: false),
        });

    await CreateAsync(ct, system,
        name: "Storm Aura",
        type: SkillType.Buff,
        loreBrief: "Thor surrounds himself with continuous storm energy.",
        loreDesc: "Electric aura pulses around the body.",
        baseStats: new BaseStatsParams(
            Cooldown: 14f,
            ManaCost: 60f,
            Damage: 20f,
            Healing: null,
            Shield: null,
            CastTime: 0f,
            ChannelDuration: 6f,
            CC: null,
            Range: 3f),
        effects: new[]
        {
            Effect(
    EffectType.Buff,
    25f,
    6f,
    0f,
    TargetType.Self,
    StackType.RefreshDuration,
    isPeriodic: true,
    isInstant: false,
    buffType: BuffType.AttackDamage),

            Effect(EffectType.Damage, 20f, 6f, 3f, TargetType.AreaEnemies, StackType.None, apRatio: 0.3f, isPeriodic: true, isInstant: false),
        });

    await CreateAsync(ct, system,
        name: "Lightning Chain",
        type: SkillType.Damage,
        loreBrief: "Lightning jumps between enemies in a devastating arc.",
        loreDesc: "Electric arcs chain across targets.",
        baseStats: new BaseStatsParams(
            Cooldown: 8f,
            ManaCost: 55f,
            Damage: 90f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CC: 2f,
            Range: 7f),
        effects: new[]
        {
            Effect(EffectType.Damage, 90f, 0f, 0f, TargetType.Chain, StackType.None, adRatio: 1.0f, isInstant: true),
            Effect(EffectType.Debuff, 15f, 2f, 0f, TargetType.Chain, StackType.None, isInstant: false),
        });

    await CreateAsync(ct, system,
        name: "God of Thunder",
        type: SkillType.Ultimate,
        loreBrief: "Thor unleashes full divine storm upon the battlefield.",
        loreDesc: "Sky fractures with continuous lightning strikes.",
        baseStats: new BaseStatsParams(
            Cooldown: 90f,
            ManaCost: 120f,
            Damage: 300f,
            Healing: null,
            Shield: null,
            CastTime: 0.5f,
            ChannelDuration: 4f,
            CC: 4f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Damage, 300f, 0f, 6f, TargetType.AreaEnemies, StackType.None, adRatio: 2.5f, isInstant: true),
            Effect(EffectType.ZoneControl, 0f, 4f, 6f, TargetType.AreaEnemies, StackType.None, isPeriodic: true, isInstant: false),
        });

    // =====================================================
    // HERAKLES
    // =====================================================

    await CreateAsync(ct, system,
        name: "Lion's Might",
        type: SkillType.Buff,
        loreBrief: "Herakles channels the strength of the Nemean Lion.",
        loreDesc: "Golden aura of overwhelming strength.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 45f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0f,
            ChannelDuration: 8f,
            CC: null,
            Range: 0f),
        effects: new[]
        {
          Effect( EffectType.Buff, 40f, 8f, 0f, TargetType.Self, StackType.Refresh, hpRatio: 0.25f, buffType: BuffType.MaxHealth),
          Effect( EffectType.Buff, 20f, 8f, 0f, TargetType.Self, StackType.Refresh, buffType: BuffType.AttackDamage),
        });

    await CreateAsync(ct, system,
        name: "Hydra Strike",
        type: SkillType.Damage,
        loreBrief: "A crushing strike inspired by the Hydra's relentless nature.",
        loreDesc: "Multiple phantom blows overlap the impact.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 170f,
            Healing: null,
            Shield: null,
            CastTime: 0.25f,
            ChannelDuration: null,
            CC: null,
            Range: 2f),
        effects: new[]
        {
            Effect(EffectType.Damage, 170f, 0f, 0f, TargetType.Enemy, StackType.None, adRatio: 1.5f),
            Effect(EffectType.Debuff, 40f,  3f,  0f,  TargetType.Enemy, StackType.None, debuffType: DebuffType.ArmorReduction),
        });

    await CreateAsync(ct, system,
        name: "Titan Grip",
        type: SkillType.Shield,
        loreBrief: "Herakles withstands any force with divine resilience.",
        loreDesc: "A titan-like shield manifests around him.",
        baseStats: new BaseStatsParams(
            Cooldown: 16f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: 120f,
            CastTime: 0f,
            ChannelDuration: 5f,
            CC: null,
            Range: 0f),
        effects: new[]
        {
            Effect(EffectType.Shield, 120f, 5f, 0f, TargetType.Self, StackType.None, hpRatio: 0.4f),
            Effect(EffectType.Buff, 20f, 5f, 0f, TargetType.Self, StackType.None, buffType: BuffType.DamageReduction),
        });

    await CreateAsync(ct, system,
        name: "Labors Rush",
        type: SkillType.Mobility,
        loreBrief: "Herakles surges forward with unstoppable momentum.",
        loreDesc: "A heroic blur of motion and strength.",
        baseStats: new BaseStatsParams(
            Cooldown: 9f,
            ManaCost: 35f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0f,
            ChannelDuration: 3f,
            CC: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.Refresh),
            Effect(EffectType.Buff, 30f, 3f, 0f, TargetType.Self, StackType.Refresh, buffType: BuffType.MovementSpeed),
        });

    await CreateAsync(ct, system,
        name: "Divine Endurance",
        type: SkillType.Ultimate,
        loreBrief: "Herakles enters a divine state of unmatched endurance.",
        loreDesc: "Golden aura of mythic resilience.",
        baseStats: new BaseStatsParams(
            Cooldown: 110f,
            ManaCost: 140f,
            Damage: null,
            Healing: 50f,
            Shield: 250f,
            CastTime: 0f,
            ChannelDuration: 10f,
            CC: null,
            Range: 0f),
        effects: new[]
        {
            Effect(EffectType.Shield, 250f, 10f, 0f, TargetType.Self, StackType.Refresh, hpRatio: 0.6f),
            Effect(EffectType.Buff, 50f, 10f, 0f, TargetType.Self, StackType.Refresh, isPeriodic: true),
        });

    await _unitOfWork.CommitAsync(ct);
  }

  // =========================================================
  // CREATE CORE
  // =========================================================

  private async Task CreateAsync(
      CancellationToken ct,
      string system,
      string name,
      SkillType type,
      string loreBrief,
      string loreDesc,
      EffectParams[] effects,
      BaseStatsParams? baseStats = null)
  {
    var skill = _skillFactory.Create();
    skill.Define(name, type, system);
    await _skills.AddAsync(skill, ct);

    if (baseStats is not null)
    {
      var stats = _statsFactory.Create();

      stats.Define(
          skill.Id,
          baseStats.Value.Cooldown,
          baseStats.Value.ManaCost,
          baseStats.Value.Damage,
          baseStats.Value.Healing,
          baseStats.Value.Shield,
          baseStats.Value.CastTime,
          baseStats.Value.ChannelDuration,
          baseStats.Value.CC,
          baseStats.Value.Range,
          system);

      await _stats.AddAsync(stats, ct);
    }

    foreach (var p in effects)
    {
      var effect = _effectFactory.Create();

      effect.Define(
    skill.Id,
    p.EffectType,

    p.Magnitude,
    p.Duration,
    p.Radius,

    p.TargetType,
    p.StackType,
    p.MaxStacks,

    p.AdRatio,
    p.ApRatio,
    p.HpRatio,

    p.IsPeriodic,
    p.IsInstant,
    p.IsChannelled,

    p.BuffType,
    p.DebuffType,

    system);

      skill.AddEffect(effect);
      await _effects.AddAsync(effect, ct);
    }

    var lore = _loreFactory.Create();
    lore.Define(skill.Id, loreBrief, loreDesc, system);
    await _lore.AddAsync(lore, ct);
  }

  // =========================================================
  // EFFECT PARAMS
  // =========================================================

  private readonly record struct EffectParams(
    EffectType EffectType,
    float Magnitude,
    float Duration,
    float Radius,

    TargetType TargetType,
    StackType StackType,

    int MaxStacks = 1,

    float? AdRatio = null,
    float? ApRatio = null,
    float? HpRatio = null,

    bool IsPeriodic = false,
    bool IsInstant = true,
    bool IsChannelled = false,

    BuffType? BuffType = null,
    DebuffType? DebuffType = null);

  private static EffectParams Effect(
    EffectType effectType,
    float magnitude,
    float duration,
    float radius,

    TargetType target,
    StackType stack,

    int maxStacks = 1,

    float? adRatio = null,
    float? apRatio = null,
    float? hpRatio = null,

    bool isPeriodic = false,
    bool isInstant = true,
    bool isChannelled = false,

    BuffType? buffType = null,
    DebuffType? debuffType = null)
  {
    return new EffectParams(
        effectType,
        magnitude,
        duration,
        radius,

        target,
        stack,

        maxStacks,

        adRatio,
        apRatio,
        hpRatio,

        isPeriodic,
        isInstant,
        isChannelled,

        buffType,
        debuffType);
  }

  // =========================================================
  // BASE STATS PARAMS
  // =========================================================

  private readonly record struct BaseStatsParams(
      float? Cooldown,
      float? ManaCost,
      float? Damage,
      float? Healing,
      float? Shield,
      float? CastTime,
      float? ChannelDuration,
      float? CC,
      float? Range);
}