using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

/// <summary>
/// Seeds the full 18-hero Skill roster (5 skills per Hero: Passive, Primary,
/// Secondary, Tertiary, Ultimate), translated and mapped from the
/// "codex_complet_heros_items_v10" design document. Numeric values (cooldown,
/// mana cost, damage, healing, shield, cast time, channel duration, crowd
/// control duration, range, AD/MD/HP ratios) are taken verbatim from the codex.
/// EffectType granularity for "CrowdControl"-tagged skills (not a real enum
/// member) was resolved to the specific mechanic named in the codex's flavor
/// text; skills with no flavor text (Tertiary/S3 slots) are marked INFERRED.
/// </summary>
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
    // THOR (WARRIOR)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Lightning Charge",
        type: SkillType.Buff,
        description: "Heavy, slow but devastating hammer blows. Each strike charges a small amount of lightning.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Thunder Strike",
        type: SkillType.Damage,
        description: "Slams Mjolnir into the ground: area damage and a brief stun around the point of impact.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 8.0f,
            ManaCost: 60f,
            Damage: 240f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: 0.8f,
            Range: 3.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 240f, 0f, 3.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.0f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Stun, 0f, 0.8f, 3.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Hammer's Call",
        type: SkillType.Mobility,
        description: "Hurls Mjolnir at the target, then launches himself to retrieve it (offensive gap-closer).",
        visualExplanation: "Single-target line skillshot.",
        baseStats: new BaseStatsParams(
            Cooldown: 12.0f,
            ManaCost: 70f,
            Damage: 180f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 180f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.8f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Asgard's Guard",
        type: SkillType.Shield,
        description: "Braces behind a shield of storm energy, absorbing incoming damage.",
        visualExplanation: "Self-cast, no target.",
        baseStats: new BaseStatsParams(
            Cooldown: 14.0f,
            ManaCost: 80f,
            Damage: null,
            Healing: null,
            Shield: 300f,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 0.0f),
        effects: new[]
        {
            Effect(EffectType.Shield, 300f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: 0.15f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Asgard's Wrath",
        type: SkillType.Ultimate,
        description: "Channels the storm: his attack speed explodes and every strike hurls lightning at nearby enemies.",
        visualExplanation: "Self-cast, no target.",
        baseStats: new BaseStatsParams(
            Cooldown: 60.0f,
            ManaCost: 120f,
            Damage: 150f,
            Healing: null,
            Shield: null,
            CastTime: 0.5f,
            ChannelDuration: 4.0f,
            CrowdControlDuration: null,
            Range: 4.0f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 4.0f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: BuffType.AttackSpeed, debuffType: null),
            Effect(EffectType.Damage, 150f, 4.0f, 4.0f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: 1.2f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // ARES (WARRIOR)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Bloodlust",
        type: SkillType.Buff,
        description: "Fast, furious spear strikes. A struck enemy bleeds briefly.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.Lifesteal, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Blood Harvest",
        type: SkillType.Damage,
        description: "Sweeps his spear in a wide arc: damage to all nearby enemies, healing him for each target struck.",
        visualExplanation: "Cone-shaped area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 7.0f,
            ManaCost: 55f,
            Damage: 220f,
            Healing: 120f,
            Shield: null,
            CastTime: 0.25f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 220f, 0f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.1f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Heal, 120f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Butcher's Charge",
        type: SkillType.Mobility,
        description: "Rushes forward in a straight line, impaling enemies in his path and knocking them back slightly.",
        visualExplanation: "Movement, straight line.",
        baseStats: new BaseStatsParams(
            Cooldown: 11.0f,
            ManaCost: 65f,
            Damage: 160f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 0.5f,
            Range: 7.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 160f, 0f, 1.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.7f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Knockback, 0f, 0.5f, 1.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "War Cry",
        type: SkillType.Debuff,
        description: "Lets out a terrifying war cry, unsettling nearby enemies.",
        visualExplanation: "Self-cast, radius around Ares.",
        baseStats: new BaseStatsParams(
            Cooldown: 13.0f,
            ManaCost: 70f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 4.0f),
        effects: new[]
        {
            Effect(EffectType.Fear, 0f, 1.5f, 4.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "War's Fury",
        type: SkillType.Ultimate,
        description: "Enters a killing rage: damage reduction, immunity to slows, and amplified strikes for the duration.",
        visualExplanation: "Self-cast, no target.",
        baseStats: new BaseStatsParams(
            Cooldown: 65.0f,
            ManaCost: 130f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: 6.0f,
            CrowdControlDuration: null,
            Range: 0.0f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 6.0f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: 0.1f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
            Effect(EffectType.Buff, 0f, 6.0f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.DamageReduction, debuffType: null),
            Effect(EffectType.Buff, 0f, 6.0f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.Tenacity, debuffType: null),
        });

    // =====================================================
    // SUSANOO (WARRIOR)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Typhoon's Breath",
        type: SkillType.Buff,
        description: "Swift katana strikes in pairs. The wind hisses with every cut.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackSpeed, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Gale Blade",
        type: SkillType.Damage,
        description: "Hurls a blade of wind that pierces through enemies in a line.",
        visualExplanation: "Single-target line skillshot, pierces.",
        baseStats: new BaseStatsParams(
            Cooldown: 6.0f,
            ManaCost: 50f,
            Damage: 200f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 7.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 200f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.9f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Typhoon Step",
        type: SkillType.Mobility,
        description: "Surges forward in a whirl, passing through the target and striking everything in his path.",
        visualExplanation: "Movement, straight line.",
        baseStats: new BaseStatsParams(
            Cooldown: 10.0f,
            ManaCost: 60f,
            Damage: 170f,
            Healing: null,
            Shield: null,
            CastTime: 0.15f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 170f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.85f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Cutting Wind",
        type: SkillType.Damage,
        description: "A sharp gust that slices and briefly staggers anyone caught in it.",
        visualExplanation: "Cone-shaped area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 9.0f,
            ManaCost: 55f,
            Damage: 190f,
            Healing: null,
            Shield: null,
            CastTime: 0.25f,
            ChannelDuration: null,
            CrowdControlDuration: 0.4f,
            Range: 4.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 190f, 0f, 4.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.8f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 30f, 0.4f, 4.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Orochi",
        type: SkillType.Ultimate,
        description: "Summons the eight-headed serpent: each head strikes a different nearby target, stacking damage.",
        visualExplanation: "Self-cast, radius around Susanoo.",
        baseStats: new BaseStatsParams(
            Cooldown: 70.0f,
            ManaCost: 140f,
            Damage: 320f,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 320f, 0f, 5.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.4f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Summon, 0f, 0f, 5.0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // LOKI (ASSASSIN)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Deceit",
        type: SkillType.Buff,
        description: "Quick, treacherous daggers. Striking from behind deals bonus damage.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.CriticalDamage, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Twin Blades",
        type: SkillType.Damage,
        description: "Throws two daggers that boomerang back, marking any enemies struck.",
        visualExplanation: "Single-target line skillshot, returns.",
        baseStats: new BaseStatsParams(
            Cooldown: 7.0f,
            ManaCost: 55f,
            Damage: 230f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 230f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.1f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 15f, 3.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: DebuffType.ArmorReduction),
        });

    await CreateAsync(ct, system,
        name: "Shadow Flee",
        type: SkillType.Mobility,
        description: "Turns invisible for a moment and moves quickly in a direction.",
        visualExplanation: "Movement, direction-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 12.0f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.1f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 0f, 1.5f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.MovementSpeed, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Poisoned Dagger",
        type: SkillType.Debuff,
        description: "A dagger coated in venom: damage over time and reduced healing on the target.",
        visualExplanation: "Single-target line skillshot.",
        baseStats: new BaseStatsParams(
            Cooldown: 9.0f,
            ManaCost: 50f,
            Damage: 140f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.DamageOverTime, 140f, 3.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 30f, 3.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: DebuffType.HealingReduction),
        });

    await CreateAsync(ct, system,
        name: "Trickster's Verdict",
        type: SkillType.Ultimate,
        description: "Teleports behind a designated target and delivers an execution strike amplified by their missing health.",
        visualExplanation: "Single-target, locked selection.",
        baseStats: new BaseStatsParams(
            Cooldown: 75.0f,
            ManaCost: 120f,
            Damage: 400f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Execute, 400f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.8f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // NYX (ASSASSIN)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Child of Night",
        type: SkillType.Buff,
        description: "Short-range shadow claws, utterly silent.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.MovementSpeed, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Nightfall Veil",
        type: SkillType.Damage,
        description: "Throws a sheet of shadow that briefly blinds and damages enemies within.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 8.0f,
            ManaCost: 60f,
            Damage: 210f,
            Healing: null,
            Shield: null,
            CastTime: 0.25f,
            ChannelDuration: null,
            CrowdControlDuration: 1.0f,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 210f, 0f, 5.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.0f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Blind, 0f, 1.0f, 5.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Starstep",
        type: SkillType.Mobility,
        description: "Dissolves and reappears further away; can be chained several times in a row.",
        visualExplanation: "Movement, direction-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 6.0f,
            ManaCost: 45f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.05f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.Independent, 3, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Shadow Claws",
        type: SkillType.Damage,
        description: "A rapid slash with claws of shadow.",
        visualExplanation: "Single-target, short range.",
        baseStats: new BaseStatsParams(
            Cooldown: 7.0f,
            ManaCost: 50f,
            Damage: 180f,
            Healing: null,
            Shield: null,
            CastTime: 0.15f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 2.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 180f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.9f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Everlasting Night",
        type: SkillType.Ultimate,
        description: "Plunges the area into darkness: enemies inside see their vision reduced, and every strike Nyx lands is amplified.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 80.0f,
            ManaCost: 130f,
            Damage: 260f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: 5.0f,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Debuff, 0f, 5.0f, 6.0f, TargetType.AreaEnemies, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: DebuffType.VisionReduction),
            Effect(EffectType.Damage, 260f, 5.0f, 6.0f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: 1.3f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // SET (ASSASSIN)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Desert Venom",
        type: SkillType.Debuff,
        description: "Sand-charged khopesh strikes, inflicting a light poison.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Debuff, 0f, 0f, 0f, TargetType.Enemy, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: DebuffType.HealingReduction),
        });

    await CreateAsync(ct, system,
        name: "Desert's Breath",
        type: SkillType.Debuff,
        description: "A gust of stinging sand that weakens the armor of every enemy struck.",
        visualExplanation: "Cone-shaped area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 8.0f,
            ManaCost: 60f,
            Damage: 170f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 170f, 0f, 5.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.7f, mdRatio: 0.2f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 20f, 4.0f, 5.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: DebuffType.ArmorReduction),
        });

    await CreateAsync(ct, system,
        name: "Jackal's Hunt",
        type: SkillType.Mobility,
        description: "Pounces on the weakened target, passing through them and repositioning behind.",
        visualExplanation: "Movement, locked selection.",
        baseStats: new BaseStatsParams(
            Cooldown: 11.0f,
            ManaCost: 65f,
            Damage: 200f,
            Healing: null,
            Shield: null,
            CastTime: 0.15f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 200f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.0f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Corrosive Sands",
        type: SkillType.Damage,
        description: "A patch of corrosive sand that lingers, burning anyone who stands in it.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 9.0f,
            ManaCost: 55f,
            Damage: 120f,
            Healing: null,
            Shield: null,
            CastTime: 0.25f,
            ChannelDuration: 3.0f,
            CrowdControlDuration: null,
            Range: 4.5f),
        effects: new[]
        {
            Effect(EffectType.DamageOverTime, 120f, 3.0f, 4.5f, TargetType.AreaEnemies, StackType.RefreshDuration, 1, adRatio: 0.5f, mdRatio: 0.3f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Curse of Chaos",
        type: SkillType.Ultimate,
        description: "Marks a target: for the duration, all damage they take is amplified and their healing reduced (never fully negated).",
        visualExplanation: "Single-target, locked selection.",
        baseStats: new BaseStatsParams(
            Cooldown: 70.0f,
            ManaCost: 140f,
            Damage: 280f,
            Healing: null,
            Shield: null,
            CastTime: 0.5f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 7.0f),
        effects: new[]
        {
            Effect(EffectType.Execute, 280f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.5f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 25f, 6.0f, 0f, TargetType.Enemy, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: DebuffType.HealingReduction),
        });

    // =====================================================
    // ZEUS (MAGE)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Static",
        type: SkillType.Buff,
        description: "Regular, steady lightning bolts at range.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackSpeed, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Targeted Bolt",
        type: SkillType.Damage,
        description: "Calls down lightning on a designated area: heavy magic damage.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 6.0f,
            ManaCost: 80f,
            Damage: 280f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 280f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 1.1f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Olympus Gale",
        type: SkillType.Mobility,
        description: "Propels himself backward in a thunderclap, knocking back and briefly stunning anyone standing too close.",
        visualExplanation: "Self-cast, radius around Zeus.",
        baseStats: new BaseStatsParams(
            Cooldown: 14.0f,
            ManaCost: 90f,
            Damage: 150f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 0.5f,
            Range: 4.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 150f, 0f, 4.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 0.5f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Knockback, 0f, 0.5f, 4.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Electric Arc",
        type: SkillType.Damage,
        description: "A crackling arc of electricity that chains between enemies and weakens them.",
        visualExplanation: "Single-target line skillshot.",
        baseStats: new BaseStatsParams(
            Cooldown: 8.0f,
            ManaCost: 85f,
            Damage: 200f,
            Healing: null,
            Shield: null,
            CastTime: 0.5f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 7.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 200f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 0.8f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 15f, 3.0f, 0f, TargetType.AreaEnemies, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: DebuffType.MagicResistanceReduction),
        });

    await CreateAsync(ct, system,
        name: "Sky's Fury",
        type: SkillType.Ultimate,
        description: "A storm of lightning rains down across a wide area for several seconds.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 85.0f,
            ManaCost: 180f,
            Damage: 180f,
            Healing: null,
            Shield: null,
            CastTime: 0.8f,
            ChannelDuration: 5.0f,
            CrowdControlDuration: null,
            Range: 8.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 180f, 5.0f, 8.0f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: 0.9f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    // =====================================================
    // AMUN-RA (MAGE)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Radiance",
        type: SkillType.Buff,
        description: "Precise, burning bolts of solar light.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AbilityPower, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Solar Disc",
        type: SkillType.Damage,
        description: "Throws a disc of fire that pierces through enemies and returns to him.",
        visualExplanation: "Single-target line skillshot, returns.",
        baseStats: new BaseStatsParams(
            Cooldown: 7.0f,
            ManaCost: 75f,
            Damage: 240f,
            Healing: null,
            Shield: null,
            CastTime: 0.35f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 240f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 1.0f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Celestial Barque",
        type: SkillType.Mobility,
        description: "Glides swiftly along a beam of light, gaining a small shield.",
        visualExplanation: "Movement, direction-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 13.0f,
            ManaCost: 85f,
            Damage: null,
            Healing: null,
            Shield: 120f,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Shield, 120f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: 0.4f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Sacred Burn",
        type: SkillType.Damage,
        description: "A burning mark that sears the target over time and weakens them.",
        visualExplanation: "Single-target, locked selection.",
        baseStats: new BaseStatsParams(
            Cooldown: 9.0f,
            ManaCost: 80f,
            Damage: 90f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: 4.0f,
            CrowdControlDuration: null,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.DamageOverTime, 90f, 4.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: 0.5f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 15f, 4.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: DebuffType.MagicResistanceReduction),
        });

    await CreateAsync(ct, system,
        name: "Eternal Noon",
        type: SkillType.Ultimate,
        description: "Summons a second sun above the zone: continuous burn on enemies within, restored energy for allies within.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 90.0f,
            ManaCost: 190f,
            Damage: 140f,
            Healing: 80f,
            Shield: null,
            CastTime: 0.7f,
            ChannelDuration: 6.0f,
            CrowdControlDuration: null,
            Range: 7.5f),
        effects: new[]
        {
            Effect(EffectType.DamageOverTime, 140f, 6.0f, 7.5f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: 0.85f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
            Effect(EffectType.Heal, 80f, 6.0f, 7.5f, TargetType.AreaAllies, StackType.Refresh, 1, adRatio: null, mdRatio: 0.85f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    // =====================================================
    // CHRONOS (MAGE)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Time Slip",
        type: SkillType.Debuff,
        description: "Temporal fragments that slightly slow whatever they strike.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Debuff, 10f, 1.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: DebuffType.MovementSpeedReduction),
        });

    await CreateAsync(ct, system,
        name: "Hour Fracture",
        type: SkillType.Control,
        description: "Briefly freezes an area of enemies within a bubble of slowed time.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 10.0f,
            ManaCost: 90f,
            Damage: 180f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Freeze, 0f, 1.5f, 6.0f, TargetType.AreaEnemies, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 180f, 0f, 6.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 0.7f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Temporal Leap",
        type: SkillType.Mobility,
        description: "Vanishes and reappears where he stood a few seconds earlier (defensive repositioning).",
        visualExplanation: "Self-cast, no target.",
        baseStats: new BaseStatsParams(
            Cooldown: 16.0f,
            ManaCost: 80f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.1f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 0f, 2.0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.MovementSpeed, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Temporal Shard",
        type: SkillType.Damage,
        description: "A shard of fractured time that damages and briefly weakens the target.",
        visualExplanation: "Single-target line skillshot.",
        baseStats: new BaseStatsParams(
            Cooldown: 7.0f,
            ManaCost: 75f,
            Damage: 220f,
            Healing: null,
            Shield: null,
            CastTime: 0.35f,
            ChannelDuration: null,
            CrowdControlDuration: 0.6f,
            Range: 6.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 220f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.95f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 30f, 0.6f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Hourglass Reversal",
        type: SkillType.Ultimate,
        description: "Radically accelerates his own spellcasting while slowing every enemy around him.",
        visualExplanation: "Self-cast, radius around Chronos.",
        baseStats: new BaseStatsParams(
            Cooldown: 95.0f,
            ManaCost: 200f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: 6.0f,
            CrowdControlDuration: 2.0f,
            Range: 7.0f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 6.0f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.CooldownReduction, debuffType: null),
            Effect(EffectType.Debuff, 0f, 6.0f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: DebuffType.AttackSpeedReduction),
            Effect(EffectType.Slow, 35f, 2.0f, 7.0f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // HERAKLES (TANK)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Titanic Strength",
        type: SkillType.Buff,
        description: "Bare-fisted blows, crushing but slow.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Nemean Grip",
        type: SkillType.Control,
        description: "Hurls his grip in a straight line: the first enemy struck is pulled behind him.",
        visualExplanation: "Single-target line skillshot.",
        baseStats: new BaseStatsParams(
            Cooldown: 10.0f,
            ManaCost: 70f,
            Damage: 180f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: 1.2f,
            Range: 3.0f),
        effects: new[]
        {
            Effect(EffectType.Pull, 0f, 1.2f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: 0.05f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 180f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.5f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Lion's Charge",
        type: SkillType.Mobility,
        description: "Charges forward, bowling over and stunning the first enemy struck.",
        visualExplanation: "Movement, straight line.",
        baseStats: new BaseStatsParams(
            Cooldown: 12.0f,
            ManaCost: 80f,
            Damage: 150f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 0.8f,
            Range: 7.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Stun, 0f, 0.8f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: 0.04f, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 150f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.4f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Lion's Skin",
        type: SkillType.Shield,
        description: "Wraps himself in the Nemean lion's hide, absorbing incoming damage.",
        visualExplanation: "Self-cast, no target.",
        baseStats: new BaseStatsParams(
            Cooldown: 14.0f,
            ManaCost: 75f,
            Damage: null,
            Healing: null,
            Shield: 350f,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 0.0f),
        effects: new[]
        {
            Effect(EffectType.Shield, 350f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: 0.12f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Twelve Labors",
        type: SkillType.Ultimate,
        description: "Surpasses himself: massive health gain, reduced damage taken, and his strikes knock enemies back.",
        visualExplanation: "Self-cast, no target.",
        baseStats: new BaseStatsParams(
            Cooldown: 80.0f,
            ManaCost: 150f,
            Damage: null,
            Healing: null,
            Shield: 500f,
            CastTime: 0.5f,
            ChannelDuration: 8.0f,
            CrowdControlDuration: null,
            Range: 0.0f),
        effects: new[]
        {
            Effect(EffectType.Shield, 500f, 8.0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: 0.2f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 0f, 8.0f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.DamageReduction, debuffType: null),
            Effect(EffectType.Knockback, 0f, 8.0f, 1.8f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // ANUBIS (TANK)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Guardian of the Threshold",
        type: SkillType.Shield,
        description: "Slow scepter strikes, tinged with funerary energy.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Shield, 25f, 2.0f, 0f, TargetType.Self, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: 0.08f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Judgment's Veil",
        type: SkillType.Shield,
        description: "Casts a shield of energy on the nearest ally and reinforces himself too.",
        visualExplanation: "Single-target, nearest ally.",
        baseStats: new BaseStatsParams(
            Cooldown: 11.0f,
            ManaCost: 80f,
            Damage: null,
            Healing: null,
            Shield: 400f,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.Shield, 400f, 0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: 0.4f, hpRatio: 0.1f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 0f, 4.0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.MaxHealth, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Passage of Shadows",
        type: SkillType.Mobility,
        description: "Instantly moves to the side of a targeted ally, surrounding them with a protective halo.",
        visualExplanation: "Single-target, ally-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 15.0f,
            ManaCost: 85f,
            Damage: null,
            Healing: null,
            Shield: 200f,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Shield, 200f, 0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: 0.2f, hpRatio: 0.05f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Funerary Scepter",
        type: SkillType.Control,
        description: "A slow, heavy scepter blow charged with funerary energy that saps the target's strength.",
        visualExplanation: "Single-target, short range.",
        baseStats: new BaseStatsParams(
            Cooldown: 10.0f,
            ManaCost: 70f,
            Damage: 140f,
            Healing: null,
            Shield: null,
            CastTime: 0.35f,
            ChannelDuration: null,
            CrowdControlDuration: 1.0f,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Slow, 25f, 1.0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.3f, mdRatio: 0.3f, hpRatio: 0.03f, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 140f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.3f, mdRatio: 0.3f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Weighing of Souls",
        type: SkillType.Ultimate,
        description: "Raises a barrier around himself: enemies who cross it are slowed and weakened.",
        visualExplanation: "Self-cast, radius around Anubis.",
        baseStats: new BaseStatsParams(
            Cooldown: 85.0f,
            ManaCost: 170f,
            Damage: null,
            Healing: null,
            Shield: 300f,
            CastTime: 0.6f,
            ChannelDuration: 7.0f,
            CrowdControlDuration: 1.5f,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Shield, 300f, 7.0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: 0.5f, hpRatio: 0.15f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 30f, 1.5f, 6.0f, TargetType.AreaEnemies, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 20f, 7.0f, 6.0f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: DebuffType.DamageAmplification),
        });

    // =====================================================
    // HEL (TANK)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Chill of the Dead",
        type: SkillType.Debuff,
        description: "Icy claws that briefly slow whatever they touch.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Debuff, 12f, 1.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: DebuffType.MovementSpeedReduction),
        });

    await CreateAsync(ct, system,
        name: "Grasp of the Fallen",
        type: SkillType.Control,
        description: "Hands burst from the ground and bind enemies within the zone.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 12.0f,
            ManaCost: 85f,
            Damage: 160f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.Root, 0f, 1.5f, 5.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 160f, 0f, 5.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.3f, mdRatio: 0.3f, hpRatio: 0.06f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Walk of the Dead",
        type: SkillType.Mobility,
        description: "Slips through the ground for a short distance, untargetable while crossing.",
        visualExplanation: "Movement, direction-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 16.0f,
            ManaCost: 80f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.15f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 0f, 1.0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.MovementSpeed, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Icy Breath",
        type: SkillType.Debuff,
        description: "A breath of glacial cold that damages over time and weakens the target.",
        visualExplanation: "Single-target, short range.",
        baseStats: new BaseStatsParams(
            Cooldown: 9.0f,
            ManaCost: 70f,
            Damage: 120f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 4.5f),
        effects: new[]
        {
            Effect(EffectType.DamageOverTime, 120f, 3.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: 0.2f, mdRatio: 0.4f, hpRatio: 0.04f, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 15f, 3.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: DebuffType.AttackSpeedReduction),
        });

    await CreateAsync(ct, system,
        name: "Domain of Helheim",
        type: SkillType.Ultimate,
        description: "A zone of her own realm: movement speed drops sharply and damage increases the longer enemies remain inside.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 90.0f,
            ManaCost: 180f,
            Damage: 150f,
            Healing: null,
            Shield: null,
            CastTime: 0.7f,
            ChannelDuration: 8.0f,
            CrowdControlDuration: null,
            Range: 7.0f),
        effects: new[]
        {
            Effect(EffectType.ZoneControl, 0f, 8.0f, 7.0f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: 0.1f, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 40f, 8.0f, 7.0f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: true, buffType: null, debuffType: null),
            Effect(EffectType.DamageOverTime, 150f, 8.0f, 7.0f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: 0.5f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    // =====================================================
    // ISIS (SUPPORT)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Ancient Magic",
        type: SkillType.Heal,
        description: "Soft bolts of magic at short range.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.HealOverTime, 5f, 3.0f, 0f, TargetType.Self, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Healing Wing",
        type: SkillType.Heal,
        description: "Spreads her wings: heals nearby allies immediately and grants them regeneration afterward.",
        visualExplanation: "Self-cast, radius around Isis.",
        baseStats: new BaseStatsParams(
            Cooldown: 9.0f,
            ManaCost: 90f,
            Damage: null,
            Healing: 280f,
            Shield: null,
            CastTime: 0.35f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Heal, 280f, 0f, 5.5f, TargetType.AreaAllies, StackType.None, 1, adRatio: null, mdRatio: 0.8f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.HealOverTime, 15f, 3.0f, 5.5f, TargetType.AreaAllies, StackType.RefreshDuration, 1, adRatio: null, mdRatio: 0.2f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Isis's Flight",
        type: SkillType.Mobility,
        description: "Takes to the air for a short distance, carrying the nearest ally along and healing them.",
        visualExplanation: "Movement, direction-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 14.0f,
            ManaCost: 85f,
            Damage: null,
            Healing: 120f,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Heal, 120f, 0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: 0.3f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Ray of Light",
        type: SkillType.Damage,
        description: "A beam of pure light that damages and weakens the target.",
        visualExplanation: "Single-target, medium range.",
        baseStats: new BaseStatsParams(
            Cooldown: 7.0f,
            ManaCost: 70f,
            Damage: 160f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: 0.5f,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 160f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Blind, 0f, 0.5f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Osiris's Reprieve",
        type: SkillType.Ultimate,
        description: "Grants a dying ally a reprieve: highly resilient and regenerating heavily for a few seconds (not a resurrection - they can still die).",
        visualExplanation: "Single-target, ally-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 100.0f,
            ManaCost: 200f,
            Damage: null,
            Healing: 400f,
            Shield: 300f,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 7.0f),
        effects: new[]
        {
            Effect(EffectType.HealOverTime, 400f, 4.0f, 0f, TargetType.Ally, StackType.Refresh, 1, adRatio: null, mdRatio: 1.0f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Shield, 300f, 4.0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 0f, 4.0f, 0f, TargetType.Ally, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.DamageReduction, debuffType: null),
        });

    // =====================================================
    // FREYJA (SUPPORT)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Valkyries' Favor",
        type: SkillType.Shield,
        description: "Bursts of golden light at medium range.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Shield, 20f, 2.0f, 0f, TargetType.Self, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Valkyries' Blessing",
        type: SkillType.Shield,
        description: "Calls down a blessing on nearby allies: grants a shield and boosts their power.",
        visualExplanation: "Self-cast, radius around Freyja.",
        baseStats: new BaseStatsParams(
            Cooldown: 10.0f,
            ManaCost: 85f,
            Damage: null,
            Healing: null,
            Shield: 320f,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Shield, 320f, 0f, 5.5f, TargetType.AreaAllies, StackType.None, 1, adRatio: null, mdRatio: 0.7f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 15f, 4.0f, 5.5f, TargetType.AreaAllies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Falcon Flight",
        type: SkillType.Mobility,
        description: "Dons her feathered cloak and streaks toward a chosen point, gaining a small shield.",
        visualExplanation: "Movement, direction-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 13.0f,
            ManaCost: 80f,
            Damage: null,
            Healing: null,
            Shield: 150f,
            CastTime: 0.15f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Shield, 150f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: 0.3f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Golden Blade",
        type: SkillType.Damage,
        description: "A radiant blade strike that briefly staggers the target.",
        visualExplanation: "Single-target, medium range.",
        baseStats: new BaseStatsParams(
            Cooldown: 8.0f,
            ManaCost: 65f,
            Damage: 150f,
            Healing: null,
            Shield: null,
            CastTime: 0.35f,
            ChannelDuration: null,
            CrowdControlDuration: 0.6f,
            Range: 4.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 150f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.3f, mdRatio: 0.5f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 25f, 0.6f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Field of Folkvangr",
        type: SkillType.Ultimate,
        description: "Consecrates a field: allies within receive a continuously regenerating shield for as long as they remain.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 95.0f,
            ManaCost: 190f,
            Damage: null,
            Healing: 150f,
            Shield: 350f,
            CastTime: 0.5f,
            ChannelDuration: 7.0f,
            CrowdControlDuration: null,
            Range: 6.5f),
        effects: new[]
        {
            Effect(EffectType.Shield, 350f, 7.0f, 6.5f, TargetType.AreaAllies, StackType.Refresh, 1, adRatio: null, mdRatio: 0.8f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
            Effect(EffectType.HealOverTime, 150f, 7.0f, 6.5f, TargetType.AreaAllies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    // =====================================================
    // ENKI (SUPPORT)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Water's Wisdom",
        type: SkillType.Buff,
        description: "Jets of flowing water at medium range.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.CooldownReduction, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Stream of Wisdom",
        type: SkillType.Buff,
        description: "A current flows through his allies: heals them and increases their damage output.",
        visualExplanation: "Single-target line skillshot, hits allies.",
        baseStats: new BaseStatsParams(
            Cooldown: 9.0f,
            ManaCost: 85f,
            Damage: null,
            Healing: 200f,
            Shield: null,
            CastTime: 0.35f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Heal, 200f, 0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: 0.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 15f, 4.0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Undercurrent",
        type: SkillType.Mobility,
        description: "Dives into the water and resurfaces further away, dragging nearby enemies in the undertow.",
        visualExplanation: "Movement, direction-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 14.0f,
            ManaCost: 90f,
            Damage: 120f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 0.5f,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Pull, 0f, 0.5f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 0.4f, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 120f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 0.4f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Invigorating Wave",
        type: SkillType.Shield,
        description: "A wave of vital energy that shields an ally.",
        visualExplanation: "Single-target, ally-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 10.0f,
            ManaCost: 75f,
            Damage: null,
            Healing: null,
            Shield: 180f,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Shield, 180f, 0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: 0.5f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Abzu",
        type: SkillType.Ultimate,
        description: "Calls forth the primordial spring: allies within recover resources and see cooldowns accelerate.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 100.0f,
            ManaCost: 200f,
            Damage: null,
            Healing: 180f,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: 6.0f,
            CrowdControlDuration: null,
            Range: 7.0f),
        effects: new[]
        {
            Effect(EffectType.Heal, 180f, 0f, 7.0f, TargetType.AreaAllies, StackType.None, 1, adRatio: null, mdRatio: 0.9f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 0f, 6.0f, 7.0f, TargetType.AreaAllies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.CooldownReduction, debuffType: null),
            Effect(EffectType.Buff, 0f, 6.0f, 7.0f, TargetType.AreaAllies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.ManaRegeneration, debuffType: null),
        });

    // =====================================================
    // ARTEMIS (RANGER)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Huntress's Eye",
        type: SkillType.Buff,
        description: "Precise, rapid bow shots at long range.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.CriticalChance, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Lunar Arrow",
        type: SkillType.Damage,
        description: "Looses a shaft that pierces enemies in a line and marks the first one struck.",
        visualExplanation: "Single-target line skillshot, pierces.",
        baseStats: new BaseStatsParams(
            Cooldown: 8.0f,
            ManaCost: 55f,
            Damage: 240f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 240f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.2f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 20f, 3.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: DebuffType.ArmorReduction),
        });

    await CreateAsync(ct, system,
        name: "Doe's Leap",
        type: SkillType.Mobility,
        description: "A nimble leap backward or sideways, briefly gaining speed.",
        visualExplanation: "Movement, direction-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 11.0f,
            ManaCost: 50f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.1f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 4.5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 0f, 2.0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.MovementSpeed, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Rain of Arrows",
        type: SkillType.Damage,
        description: "A volley of arrows blankets an area in front of her.",
        visualExplanation: "Ground-targeted area effect.",
        baseStats: new BaseStatsParams(
            Cooldown: 10.0f,
            ManaCost: 60f,
            Damage: 180f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 7.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 180f, 0f, 7.0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.9f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.ZoneControl, 0f, 0f, 7.0f, TargetType.Ground, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Sacred Hunt",
        type: SkillType.Ultimate,
        description: "Marks a chosen target: her shots against them are amplified, tracking them wherever they go while isolated.",
        visualExplanation: "Single-target, locked selection.",
        baseStats: new BaseStatsParams(
            Cooldown: 75.0f,
            ManaCost: 130f,
            Damage: 300f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 9.0f),
        effects: new[]
        {
            Effect(EffectType.Execute, 300f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Vision, 0f, 8.0f, 0f, TargetType.Enemy, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // VIDAR (RANGER)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Avenging Silence",
        type: SkillType.Buff,
        description: "Heavy bow shots, slow but piercing.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Silencing Shaft",
        type: SkillType.Damage,
        description: "A massive arrow that punches through armor and reduces the armor of the first enemy struck.",
        visualExplanation: "Single-target line skillshot.",
        baseStats: new BaseStatsParams(
            Cooldown: 9.0f,
            ManaCost: 60f,
            Damage: 260f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 260f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.3f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 15f, 3.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: DebuffType.ArmorReduction),
        });

    await CreateAsync(ct, system,
        name: "Iron Stride",
        type: SkillType.Mobility,
        description: "Steps back and plants himself firmly, briefly gaining precision and range.",
        visualExplanation: "Movement, backward step.",
        baseStats: new BaseStatsParams(
            Cooldown: 12.0f,
            ManaCost: 55f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.15f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 4.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 15f, 3.0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackRange, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Crushing Boot",
        type: SkillType.Control,
        description: "A crushing stomp that damages and briefly stuns the target.",
        visualExplanation: "Single-target, short range.",
        baseStats: new BaseStatsParams(
            Cooldown: 11.0f,
            ManaCost: 65f,
            Damage: 200f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: 0.9f,
            Range: 2.5f),
        effects: new[]
        {
            Effect(EffectType.Stun, 0f, 0.9f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 200f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.0f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Fenrir's Vengeance",
        type: SkillType.Ultimate,
        description: "Plants his feet and unleashes a continuous volley: his rate of fire and range climb the longer he stays still.",
        visualExplanation: "Self-cast, no target.",
        baseStats: new BaseStatsParams(
            Cooldown: 80.0f,
            ManaCost: 140f,
            Damage: 120f,
            Healing: null,
            Shield: null,
            CastTime: 0.5f,
            ChannelDuration: 6.0f,
            CrowdControlDuration: null,
            Range: 9.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 6.0f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: BuffType.AttackSpeed, debuffType: null),
            Effect(EffectType.Damage, 120f, 6.0f, 0f, TargetType.Enemy, StackType.Refresh, 1, adRatio: 1.0f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    // =====================================================
    // RAMA (RANGER)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Discipline",
        type: SkillType.Buff,
        description: "A very regular chain of shots, fired at a high rate.",
        visualExplanation: "Passive - triggers on basic attack, no cast.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: null),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackSpeed, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Arrow of Dharma",
        type: SkillType.Damage,
        description: "A blessed shaft that deals heavy damage and weakens the target.",
        visualExplanation: "Single-target line skillshot.",
        baseStats: new BaseStatsParams(
            Cooldown: 7.0f,
            ManaCost: 55f,
            Damage: 230f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 230f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.15f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 15f, 3.0f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: DebuffType.AttackDamageReduction),
        });

    await CreateAsync(ct, system,
        name: "Prince's Stride",
        type: SkillType.Mobility,
        description: "Slides elegantly to the side while continuing to fire.",
        visualExplanation: "Movement, direction-targeted.",
        baseStats: new BaseStatsParams(
            Cooldown: 10.0f,
            ManaCost: 50f,
            Damage: 140f,
            Healing: null,
            Shield: null,
            CastTime: 0.1f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 140f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Chained Shot",
        type: SkillType.Damage,
        description: "A steady, chained shot dealing reliable damage.",
        visualExplanation: "Single-target line skillshot.",
        baseStats: new BaseStatsParams(
            Cooldown: 9.0f,
            ManaCost: 60f,
            Damage: 170f,
            Healing: null,
            Shield: null,
            CastTime: 0.25f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 7.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 170f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.95f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Brahmastra",
        type: SkillType.Ultimate,
        description: "Draws his divine bow and looses a colossal arrow that pierces the entire line, dealing massive damage.",
        visualExplanation: "Single-target line skillshot, pierces.",
        baseStats: new BaseStatsParams(
            Cooldown: 85.0f,
            ManaCost: 150f,
            Damage: 420f,
            Healing: null,
            Shield: null,
            CastTime: 0.9f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 12.0f),
        effects: new[]
        {
            Effect(EffectType.Execute, 420f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.7f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
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
      string description,
      string visualExplanation,
      EffectParams[] effects,
      BaseStatsParams baseStats)
  {
    var skill = _skillFactory.Create();
    skill.Define(name, type, system);

    var stats = _statsFactory.Create();

    stats.Define(
        skill.Id,
        baseStats.Cooldown,
        baseStats.ManaCost,
        baseStats.Damage,
        baseStats.Healing,
        baseStats.Shield,
        baseStats.CastTime,
        baseStats.ChannelDuration,
        baseStats.CrowdControlDuration,
        baseStats.Range,
        system);

    skill.SetBaseStats(stats);

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
          p.MdRatio,
          p.HpRatio,
          p.IsPeriodic,
          p.IsInstant,
          p.IsChannelled,
          p.BuffType,
          p.DebuffType,
          system);

      skill.AddEffect(effect);
    }

    // Relying on aggregate tracking behaviors. Child navigations map cleanly.
    await _skills.AddAsync(skill, ct);

    var lore = _loreFactory.Create();
    lore.Define(skill.Id, description, visualExplanation, system);
    await _lore.AddAsync(lore, ct);
  }

  // =========================================================
  // PARAM RECORDS + HELPERS
  // =========================================================

  private readonly record struct EffectParams(
      EffectType EffectType,
      float Magnitude,
      float Duration,
      float Radius,
      TargetType TargetType,
      StackType StackType,
      int MaxStacks,
      float? AdRatio,
      float? MdRatio,
      float? HpRatio,
      bool IsPeriodic,
      bool IsInstant,
      bool IsChannelled,
      BuffType? BuffType,
      DebuffType? DebuffType);

  private static EffectParams Effect(
      EffectType effectType,
      float magnitude,
      float duration,
      float radius,
      TargetType target,
      StackType stack,
      int maxStacks = 1,
      float? adRatio = null,
      float? mdRatio = null,
      float? hpRatio = null,
      bool isPeriodic = false,
      bool isInstant = true,
      bool isChannelled = false,
      BuffType? buffType = null,
      DebuffType? debuffType = null)
      => new(effectType, magnitude, duration, radius,
             target, stack, maxStacks,
             adRatio, mdRatio, hpRatio,
             isPeriodic, isInstant, isChannelled,
             buffType, debuffType);

  private readonly record struct BaseStatsParams(
      float? Cooldown,
      float? ManaCost,
      float? Damage,
      float? Healing,
      float? Shield,
      float? CastTime,
      float? ChannelDuration,
      float? CrowdControlDuration,
      float? Range);
}