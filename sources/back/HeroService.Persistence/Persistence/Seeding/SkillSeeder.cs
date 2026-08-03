using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

/// <summary>
/// Seeds the full 30-hero Skill roster (5 skills per Hero: Passive, Primary,
/// Secondary, Tertiary, Ultimate), translated and mapped from
/// "codex_heros_v17" (Mickael's revised codex, 5 heroes per class).
///
/// NOTE: the codex's 5 kit slots are "Attaque de base / Passive / Primary /
/// Secondary / Ultimate" -- no slot is literally named "Tertiary". Since the
/// domain's HeroSkillKit needs exactly 5 named slots and "Attaque de base"
/// is the one left over, it is mapped to TertiarySkillId here. Worth realigning
/// naming with Mickael for the next revision, not blocking on it now.
///
/// EffectType tags are taken directly from the codex -- all 25 distinct tags
/// used verified as real EffectType enum members (no generic categories this
/// time). AD/MD ratios are not given by the codex; assigned per hero's
/// dominant stat (AD-dominant vs MD-dominant/mixed), scaled by slot.
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
        name: "Static Charge",
        type: SkillType.Buff,
        description: "After casting a spell, his next basic attack electrifies the target and nearby enemies.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 50f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 4f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
            Effect(EffectType.Damage, 50f, 0f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.4f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Thunder Strike",
        type: SkillType.Damage,
        description: "Hurls a bolt of lightning in a straight line; the first enemy struck is stunned.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 220f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: 1f,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 220f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.1f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Stun, 0f, 1f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Hammer's Call",
        type: SkillType.Mobility,
        description: "Throws Mjolnir at a point; reactivating teleports Thor to the hammer.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: 130f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 130f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.8f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Mjolnir Blows",
        type: SkillType.Damage,
        description: "Heavy, slow strikes; each hit crackles a spark onto the target.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Asgard's Wrath",
        type: SkillType.Ultimate,
        description: "Calls lightning down on a wide area, launching enemies into the air.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 88f,
            ManaCost: 120f,
            Damage: 400f,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Damage, 400f, 0f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.8f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.KnockUp, 0f, 1.5f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // ARES (WARRIOR)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Bloodlust",
        type: SkillType.Buff,
        description: "Ares recovers part of the damage he deals as life.",
        visualExplanation: "No aim.",
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
        description: "Sweeps his spear in a cone, opening wounds that bleed over time.",
        visualExplanation: "Cone.",
        baseStats: new BaseStatsParams(
            Cooldown: 8f,
            ManaCost: 55f,
            Damage: 180f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: 3f,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 180f, 0f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.9f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.DamageOverTime, 90f, 3f, 3.5f, TargetType.AreaEnemies, StackType.RefreshDuration, 1, adRatio: 0.4f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Butcher's Charge",
        type: SkillType.Mobility,
        description: "Rushes forward and knocks back the first enemy struck.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 11f,
            ManaCost: 60f,
            Damage: 150f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 0.5f,
            Range: 7f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 150f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.8f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Knockback, 0f, 0.5f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Spear Blows",
        type: SkillType.Damage,
        description: "Fast, furious strikes; a struck enemy bleeds briefly.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 62f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 62f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "War's Fury",
        type: SkillType.Ultimate,
        description: "Enters a rage: increased damage and speed, briefly terrifying nearby enemies on activation.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: 85f,
            ManaCost: 140f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: 6f,
            CrowdControlDuration: 1f,
            Range: 4f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 6f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
            Effect(EffectType.Fear, 0f, 1f, 4f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // GUAN YU (WARRIOR)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Unshakeable Loyalty",
        type: SkillType.Buff,
        description: "Below a health threshold, Guan Yu gains damage reduction that grows as he weakens.",
        visualExplanation: "No aim.",
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
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: 0.15f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.DamageReduction, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Green Dragon Blade",
        type: SkillType.Damage,
        description: "Cleaves the air in a line, slicing every enemy aligned.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 50f,
            Damage: 210f,
            Healing: null,
            Shield: null,
            CastTime: 0.35f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 7.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 210f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.0f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Red Hare Charge",
        type: SkillType.Mobility,
        description: "Rides forward and topples enemies struck.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: 140f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 0.6f,
            Range: 7f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 140f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.7f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Knockback, 0f, 0.6f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Halberd Sweep",
        type: SkillType.Damage,
        description: "Wide, sweeping strikes hitting a short line in front of him.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 58f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.9f),
        effects: new[]
        {
            Effect(EffectType.Damage, 58f, 0f, 1.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.55f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Oath of the Three Brothers",
        type: SkillType.Ultimate,
        description: "A devastating charge along a long line, stunning every enemy crossed.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 90f,
            ManaCost: 130f,
            Damage: 360f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: 1.2f,
            Range: 9f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 360f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Stun, 0f, 1.2f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // OGUN (WARRIOR)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Living Metal",
        type: SkillType.Debuff,
        description: "Ogun's attacks corrode the target's armor, stacking.",
        visualExplanation: "No aim.",
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
            Effect(EffectType.Debuff, 8f, 4f, 0f, TargetType.Enemy, StackType.Additive, 4, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: DebuffType.ArmorReduction),
        });

    await CreateAsync(ct, system,
        name: "Iron Edge",
        type: SkillType.Damage,
        description: "A cone strike that leaves a burning gash over time.",
        visualExplanation: "Cone.",
        baseStats: new BaseStatsParams(
            Cooldown: 8f,
            ManaCost: 55f,
            Damage: 170f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: 3f,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 170f, 0f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.85f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.DamageOverTime, 80f, 3f, 3.5f, TargetType.AreaEnemies, StackType.RefreshDuration, 1, adRatio: 0.35f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Blacksmith's Stride",
        type: SkillType.Mobility,
        description: "Advances while striking the ground, slowing enemies in his path.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 11f,
            ManaCost: 55f,
            Damage: 110f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 1.2f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 30f, 1.2f, 4f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 110f, 0f, 4f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.5f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Machete Blows",
        type: SkillType.Damage,
        description: "Iron strikes that gradually wear down the enemy's guard.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Forge's Wrath",
        type: SkillType.Ultimate,
        description: "Slams his weapon down: a seismic wave in an area stuns enemies.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 88f,
            ManaCost: 125f,
            Damage: 380f,
            Healing: null,
            Shield: null,
            CastTime: 0.5f,
            ChannelDuration: null,
            CrowdControlDuration: 1.3f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Damage, 380f, 0f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.7f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Stun, 0f, 1.3f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // SUSANOO (WARRIOR)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Breath of Battle",
        type: SkillType.Buff,
        description: "Every third strike releases a cutting gust around him.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 55f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Capped, 3, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackSpeed, debuffType: null),
            Effect(EffectType.Damage, 55f, 0f, 3f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.5f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Gale Blade",
        type: SkillType.Damage,
        description: "Hurls a blade of wind that passes through enemies in a line.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 6f,
            ManaCost: 50f,
            Damage: 200f,
            Healing: null,
            Shield: null,
            CastTime: 0.25f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 7f),
        effects: new[]
        {
            Effect(EffectType.Damage, 200f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.95f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Typhoon Step",
        type: SkillType.Mobility,
        description: "A cutting dash that wounds enemies crossed.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 9f,
            ManaCost: 55f,
            Damage: 150f,
            Healing: null,
            Shield: null,
            CastTime: 0.15f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 150f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.75f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Kusanagi Strikes",
        type: SkillType.Damage,
        description: "Swift strikes in pairs; the wind hisses with every pass.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.9f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Orochi",
        type: SkillType.Ultimate,
        description: "Summons the eight-headed serpent, which strikes nearby enemies for several seconds.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: 85f,
            ManaCost: 130f,
            Damage: 340f,
            Healing: null,
            Shield: null,
            CastTime: 0.5f,
            ChannelDuration: 4f,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Summon, 0f, 4f, 5f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: true, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 340f, 4f, 5f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: 1.5f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    // =====================================================
    // LOKI (ASSASSIN)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Deceit",
        type: SkillType.Buff,
        description: "After casting a spell, Loki briefly becomes invisible.",
        visualExplanation: "No aim.",
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
            Effect(EffectType.Buff, 0f, 1.5f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.MovementSpeed, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Twin Blades",
        type: SkillType.Damage,
        description: "Throws two poisoned daggers in a line that sap life over time.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 230f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Damage, 230f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.1f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.DamageOverTime, 60f, 3f, 0f, TargetType.AreaEnemies, StackType.RefreshDuration, 1, adRatio: 0.3f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Shadow Flee",
        type: SkillType.Mobility,
        description: "Teleports away, leaving a decoy in his place.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
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
        });

    await CreateAsync(ct, system,
        name: "Sneaking Daggers",
        type: SkillType.Damage,
        description: "Quick strikes; a hit to the back deals bonus damage.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 64f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 64f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.65f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Trickster's Verdict",
        type: SkillType.Ultimate,
        description: "Teleports behind a designated target and delivers an execution strike amplified by their missing health.",
        visualExplanation: "Locked.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: 400f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Execute, 400f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.9f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // SET (ASSASSIN)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Desert's Blood",
        type: SkillType.Debuff,
        description: "Set's damage leaves a poison that lingers over time.",
        visualExplanation: "No aim.",
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
            Effect(EffectType.DamageOverTime, 30f, 3f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: 0.2f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Sandstorm",
        type: SkillType.Damage,
        description: "A cone gust that blinds enemies struck.",
        visualExplanation: "Cone.",
        baseStats: new BaseStatsParams(
            Cooldown: 8f,
            ManaCost: 55f,
            Damage: 190f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: 1f,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 190f, 0f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.9f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Blind, 0f, 1f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Chaos Mist",
        type: SkillType.Mobility,
        description: "Dissolves into sand and reappears a short distance away.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 11f,
            ManaCost: 55f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.1f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Khopesh Blows",
        type: SkillType.Damage,
        description: "Strikes charged with abrasive sand.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 62f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.9f),
        effects: new[]
        {
            Effect(EffectType.Damage, 62f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.62f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Set's Judgment",
        type: SkillType.Ultimate,
        description: "Descends on a designated target and executes it, harder the more wounded they are.",
        visualExplanation: "Locked.",
        baseStats: new BaseStatsParams(
            Cooldown: 78f,
            ManaCost: 120f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 7f),
        effects: new[]
        {
            Effect(EffectType.Execute, 380f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.8f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // KALI (ASSASSIN)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Blood Rapture",
        type: SkillType.Buff,
        description: "Every takedown briefly increases her attack speed.",
        visualExplanation: "No aim.",
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
            Effect(EffectType.Buff, 0f, 5f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackSpeed, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Dance of Blades",
        type: SkillType.Damage,
        description: "Whirls in place, slashing every enemy around her; while whirling, she takes 25% less damage.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: 8f,
            ManaCost: 55f,
            Damage: 200f,
            Healing: null,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: 1.5f,
            CrowdControlDuration: null,
            Range: 3f),
        effects: new[]
        {
            Effect(EffectType.Damage, 200f, 1.5f, 3f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: 1.0f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 25f, 1.5f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: true, buffType: BuffType.DamageReduction, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Goddess's Leap",
        type: SkillType.Mobility,
        description: "Leaps to a targeted area, striking on landing.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 10f,
            ManaCost: 55f,
            Damage: 160f,
            Healing: null,
            Shield: null,
            CastTime: 0.15f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 160f, 0f, 3f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.8f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Many Blades",
        type: SkillType.Damage,
        description: "Her many arms strike in a cascade.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 63f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 63f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.63f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Destructive Fury",
        type: SkillType.Ultimate,
        description: "Locks onto a target and delivers a flurry of blows that finishes it off if weakened.",
        visualExplanation: "Locked.",
        baseStats: new BaseStatsParams(
            Cooldown: 75f,
            ManaCost: 120f,
            Damage: 370f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 7f),
        effects: new[]
        {
            Effect(EffectType.Damage, 370f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.7f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Execute, 120f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.5f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // CAMAZOTZ (ASSASSIN)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Nocturnal Flight",
        type: SkillType.Buff,
        description: "Camazotz recovers 22% of damage dealt as health — a hunter of attrition, not a burst reset.",
        visualExplanation: "No aim.",
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
        name: "Blood Swarm",
        type: SkillType.Damage,
        description: "Sends a swarm of bats through enemies in a line.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 210f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 210f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.0f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Raptor Dive",
        type: SkillType.Mobility,
        description: "Takes flight and dives on a targeted point.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 11f,
            ManaCost: 55f,
            Damage: 150f,
            Healing: null,
            Shield: null,
            CastTime: 0.15f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 150f, 0f, 3f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.75f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Claws and Fangs",
        type: SkillType.Damage,
        description: "Quick lacerations followed by a bite.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 61f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 61f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.61f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Feast of Xibalba",
        type: SkillType.Ultimate,
        description: "Locks onto a weakened target, drains it, and heals as it executes.",
        visualExplanation: "Locked.",
        baseStats: new BaseStatsParams(
            Cooldown: 78f,
            ManaCost: 120f,
            Damage: null,
            Healing: 150f,
            Shield: null,
            CastTime: 0.3f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 7f),
        effects: new[]
        {
            Effect(EffectType.Execute, 380f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.8f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Heal, 150f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // NYX (ASSASSIN)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Nightfall Veil",
        type: SkillType.Buff,
        description: "In darkness, Nyx gains speed and expanded vision.",
        visualExplanation: "No aim.",
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
            Effect(EffectType.Vision, 0f, 0f, 4f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Blade of Darkness",
        type: SkillType.Damage,
        description: "Throws a blade of shadow that streaks in a straight line.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 220f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 220f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.05f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Shadow Step",
        type: SkillType.Mobility,
        description: "Melts into darkness and resurfaces elsewhere.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 9f,
            ManaCost: 50f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.1f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Shadow Blades",
        type: SkillType.Damage,
        description: "Silent strikes that seem to leap from the dark.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 62f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 62f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.62f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Everlasting Night",
        type: SkillType.Ultimate,
        description: "Plunges an area into absolute darkness, blinding enemies caught inside.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 82f,
            ManaCost: 125f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: 4f,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Blind, 0f, 4f, 6f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
            Effect(EffectType.ZoneControl, 0f, 4f, 6f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    // =====================================================
    // ZEUS (MAGE)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Celestial Charge",
        type: SkillType.Debuff,
        description: "Zeus's spells mark the target; at three marks, they take an extra discharge.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 55f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Debuff, 0f, 4f, 0f, TargetType.Enemy, StackType.Additive, 3, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: DebuffType.MagicResistanceReduction),
            Effect(EffectType.Damage, 55f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.5f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Targeted Bolt",
        type: SkillType.Damage,
        description: "Calls down lightning on a chosen area.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 242f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Damage, 242f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 1.1f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Olympus Gale",
        type: SkillType.Mobility,
        description: "Knocks back nearby enemies with a backhand and steps away.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 1f,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Knockback, 0f, 1f, 4f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Sparks",
        type: SkillType.Damage,
        description: "Medium-range electric discharges.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 66f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 66f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Sky's Wrath",
        type: SkillType.Ultimate,
        description: "Unleashes a rain of lightning that jumps between enemies across a wide area.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: 440f,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Damage, 440f, 0f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 1.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // RA (MAGE)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Solar Fire",
        type: SkillType.Damage,
        description: "Ra's spells set the target ablaze, dealing damage over time.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.DamageOverTime, 50f, 3f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: 0.3f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Solar Ray",
        type: SkillType.Damage,
        description: "Fires a long beam of fire in a straight line.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 242f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 242f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 1.05f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Ascension",
        type: SkillType.Mobility,
        description: "Rises briefly, untouchable, and lands further away.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Rays",
        type: SkillType.Damage,
        description: "Beams of burning light.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 66f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 66f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Noon Judgment",
        type: SkillType.Ultimate,
        description: "Converges light on an area, burning enemies and healing allies present.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: 440f,
            Healing: 320f,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Damage, 440f, 0f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 1.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Heal, 320f, 0f, 6f, TargetType.AreaAllies, StackType.None, 1, adRatio: null, mdRatio: 1.1f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // AGNI (MAGE)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Combustion",
        type: SkillType.Buff,
        description: "Four consecutive spells trigger an explosion on the next target struck.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 55f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Capped, 4, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AbilityPower, debuffType: null),
            Effect(EffectType.Damage, 55f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.5f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Flame Javelin",
        type: SkillType.Damage,
        description: "Throws a spear of fire in a line.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 242f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 242f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 1.05f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Blazing Trail",
        type: SkillType.Mobility,
        description: "Leaps away, leaving a trail of flame on the ground.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: 3f,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.ZoneControl, 0f, 3f, 4f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Ember Spit",
        type: SkillType.Damage,
        description: "Throws small flames.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 66f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 66f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Pillar of Fire",
        type: SkillType.Ultimate,
        description: "A column of fire erupts on an area, launching enemies into the air.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 87f,
            ManaCost: 120f,
            Damage: 440f,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Damage, 440f, 0f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 1.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.KnockUp, 0f, 1.5f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // RAIJIN (MAGE)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Storm's Rhythm",
        type: SkillType.Buff,
        description: "Every spell briefly accelerates the casting of the next one.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 3f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.CooldownReduction, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Thunderclap",
        type: SkillType.Damage,
        description: "Sends a shockwave in a cone in front of him.",
        visualExplanation: "Cone.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 242f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 242f, 0f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 1.05f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Thunder Step",
        type: SkillType.Mobility,
        description: "Moves in a flash of lightning toward a direction.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Drum Rolls",
        type: SkillType.Damage,
        description: "Strikes the air with his drumsticks, sending sonic waves.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 66f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 66f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Drum Fury",
        type: SkillType.Ultimate,
        description: "Calls lightning onto an area, stunning enemies struck.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: 440f,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Damage, 440f, 0f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: 1.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Stun, 0f, 1.5f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // MARDUK (MAGE)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Tablets of Destiny",
        type: SkillType.Buff,
        description: "Marduk's spells against a restrained target deal bonus damage; he trades raw power for longer, more frequent restraints.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AbilityPower, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Net of the Winds",
        type: SkillType.Damage,
        description: "Throws a net in a line that roots the first enemy caught.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 6f,
            ManaCost: 55f,
            Damage: 218f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: 1.1f,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Root, 0f, 1.1f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 218f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 1.0f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Primordial Breath",
        type: SkillType.Mobility,
        description: "Retreats while blowing a gust that slows pursuers.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 10f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 1.1f,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 30f, 1.1f, 4f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Shards of Power",
        type: SkillType.Damage,
        description: "Throws fragments of primordial energy.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 66f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 66f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Seal of Tiamat",
        type: SkillType.Ultimate,
        description: "Seals an area: enemies inside are slowed and take increasing damage.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 65f,
            ManaCost: 85f,
            Damage: 396f,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.7f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.ZoneControl, 0f, 1.7f, 6f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 35f, 1.7f, 6f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: true, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 396f, 1.7f, 6f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: 1.5f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    // =====================================================
    // HERAKLES (TANK)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Hero's Endurance",
        type: SkillType.Buff,
        description: "Every hit taken briefly hardens Herakles, reducing subsequent damage.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 2f, 0f, TargetType.Self, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.DamageReduction, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Nemean Grip",
        type: SkillType.Control,
        description: "Throws his grip in a straight line: the first enemy struck is pulled to him.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 220f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: 1f,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Pull, 0f, 1f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: 0.05f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 220f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.5f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Lion's Charge",
        type: SkillType.Mobility,
        description: "Charges forward, bowling over and stunning the first enemy struck.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: 130f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 1f,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Stun, 0f, 1f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: 0.04f, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 130f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.4f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Club Blows",
        type: SkillType.Damage,
        description: "Crushing, slow strikes.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.55f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Twelve Labors",
        type: SkillType.Ultimate,
        description: "Anchors himself: for a few seconds, deals heavy area damage and resists enormously.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: 400f,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: 4f,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 4f, 0f, TargetType.Self, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: true, buffType: BuffType.DamageReduction, debuffType: null),
            Effect(EffectType.Damage, 400f, 4f, 3.5f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    // =====================================================
    // YMIR (TANK)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Flesh of Ice",
        type: SkillType.Shield,
        description: "While standing still, Ymir regenerates a shield of frost.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: 60f,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Shield, 60f, 3f, 0f, TargetType.Self, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: 0.02f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Glacial Shard",
        type: SkillType.Damage,
        description: "Throws an ice spike in a line that slows the target.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 220f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: 1f,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 220f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.5f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 30f, 1f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Wall of Frost",
        type: SkillType.Control,
        description: "Raises a wall of ice on the ground to block passage.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: 6f,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.ZoneControl, 0f, 6f, 3f, TargetType.Ground, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: true, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Frost Fists",
        type: SkillType.Damage,
        description: "Heavy blows that numb with cold.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.55f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Grip of Frost",
        type: SkillType.Ultimate,
        description: "Freezes enemies in a nearby area in place for a short time.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Freeze, 0f, 1.5f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // GEB (TANK)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Stone Skin",
        type: SkillType.Buff,
        description: "Geb's telluric mass reduces the crowd control effects he suffers.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.Tenacity, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Telluric Shard",
        type: SkillType.Damage,
        description: "Throws a rock in a line that stuns the first target.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 220f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: 1f,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 220f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.5f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Stun, 0f, 1f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Earthen Aegis",
        type: SkillType.Shield,
        description: "Wraps a targeted ally in a protective casing of stone.",
        visualExplanation: "Ally.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: 150f,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Shield, 150f, 0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: 0.06f, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Stone Fists",
        type: SkillType.Damage,
        description: "Massive blows of rock.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.55f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Cataclysm",
        type: SkillType.Ultimate,
        description: "Shakes a wide area, launching enemies into the air.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 87f,
            ManaCost: 120f,
            Damage: 400f,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Damage, 400f, 0f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.KnockUp, 0f, 1.5f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // KUMBHAKARNA (TANK)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Giant's Slumber",
        type: SkillType.Buff,
        description: "Standing still, Kumbhakarna builds power; his next strike is amplified.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Great Sweep",
        type: SkillType.Damage,
        description: "Sweeps a cone in front of him, knocking enemies back.",
        visualExplanation: "Cone.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 220f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: 1f,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Damage, 220f, 0f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Knockback, 0f, 1f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Ponderous Stride",
        type: SkillType.Mobility,
        description: "Advances heavily, unaffected by restraints while moving.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Massive Backhand",
        type: SkillType.Damage,
        description: "Slow but devastating blows.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.55f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Terrible Awakening",
        type: SkillType.Ultimate,
        description: "Wakes with a crash: stuns and puts to sleep the enemies of a wide area.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 90f,
            ManaCost: 120f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Stun, 0f, 1.5f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Sleep, 0f, 1.5f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // GILGAMESH (TANK)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Two-Thirds Divine",
        type: SkillType.Buff,
        description: "Gilgamesh regenerates health when surrounded by several enemies.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: 40f,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.Lifesteal, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Celestial Slash",
        type: SkillType.Damage,
        description: "Brings his axe down in a line, wounding enemies aligned.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 220f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 220f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Charge of Uruk",
        type: SkillType.Mobility,
        description: "Rushes to a point and taunts nearby enemies on arrival.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 1f,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Taunt, 0f, 1f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Royal Blows",
        type: SkillType.Damage,
        description: "Authoritative axe strikes.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 1.8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.55f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "King's Judgment",
        type: SkillType.Ultimate,
        description: "Challenges an area: enemies are forced to attack him and see their damage reduced.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Taunt, 0f, 1.5f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Debuff, 25f, 4f, 3.5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: DebuffType.AttackDamageReduction),
        });

    // =====================================================
    // FREYJA (SUPPORT)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Vanir's Favor",
        type: SkillType.Shield,
        description: "Freyja's heals also grant a light shield to the target.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: 60f,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Shield, 60f, 3f, 0f, TargetType.Ally, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Valkyries' Blessing",
        type: SkillType.Buff,
        description: "Strengthens a targeted ally, increasing their damage briefly.",
        visualExplanation: "Ally.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: 3f,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 15f, 3f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Falcon Flight",
        type: SkillType.Mobility,
        description: "Dons her feathered cloak and streaks toward a direction.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Golden Shards",
        type: SkillType.Damage,
        description: "Throws shards of warm light.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.5f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Dawn of Folkvangr",
        type: SkillType.Ultimate,
        description: "Heals allies in an area and slows enemies present.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: null,
            Healing: 320f,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Heal, 320f, 0f, 6f, TargetType.AreaAllies, StackType.None, 1, adRatio: null, mdRatio: 1.4f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 30f, 1.5f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // ISIS (SUPPORT)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Ancestral Magic",
        type: SkillType.Buff,
        description: "Isis's spells slightly reduce their own cooldown when they hit an ally.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.CooldownReduction, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Healing Wing",
        type: SkillType.Heal,
        description: "Spreads a wing that heals a targeted ally.",
        visualExplanation: "Ally.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: null,
            Healing: 180f,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Heal, 180f, 0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: 0.8f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Veil of Isis",
        type: SkillType.Mobility,
        description: "Wraps herself in a veil and moves, protected by a shield.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: 150f,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Shield, 150f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: 0.6f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Breath of Life",
        type: SkillType.Damage,
        description: "Sends an offensive curative breeze.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.5f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Osiris's Resurrection",
        type: SkillType.Ultimate,
        description: "Marks an ally: if they fall within the next few seconds, they are reborn with part of their health.",
        visualExplanation: "Ally.",
        baseStats: new BaseStatsParams(
            Cooldown: 115f,
            ManaCost: 130f,
            Damage: null,
            Healing: 320f,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.HealOverTime, 320f, 4f, 0f, TargetType.Ally, StackType.Refresh, 1, adRatio: null, mdRatio: 1.3f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 0f, 8f, 0f, TargetType.Ally, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.MaxHealth, debuffType: null),
        });

    // =====================================================
    // APHRODITE (SUPPORT)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Grace",
        type: SkillType.Heal,
        description: "Aphrodite and the ally she is bonded to slowly regenerate health.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: 40f,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.HealOverTime, 40f, 3f, 0f, TargetType.Self, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Embrace",
        type: SkillType.Heal,
        description: "Bonds to a targeted ally: while the bond holds, she heals them from a distance.",
        visualExplanation: "Ally.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: null,
            Healing: 180f,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Heal, 180f, 0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: 0.8f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Buff, 0f, 4f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Flight of Doves",
        type: SkillType.Mobility,
        description: "Rises on a flight of doves toward a direction.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Ardent Kisses",
        type: SkillType.Damage,
        description: "Sends darts of passion.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.5f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Intoxicating Charm",
        type: SkillType.Ultimate,
        description: "Enthralls enemies in a cone, charming them toward her briefly.",
        visualExplanation: "Cone.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Charm, 0f, 1.5f, 5f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // GUANYIN (SUPPORT)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Compassion",
        type: SkillType.Buff,
        description: "Healing an ally below a health threshold amplifies the heal.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.HealingPower, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Willow Water",
        type: SkillType.Heal,
        description: "Pours a curative stream on a targeted ally, cleansing a weakening effect.",
        visualExplanation: "Ally.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: null,
            Healing: 180f,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Heal, 180f, 0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: 0.8f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Utility, 0f, 0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Lotus Step",
        type: SkillType.Mobility,
        description: "Glides on a lotus toward a direction.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Jade Droplets",
        type: SkillType.Damage,
        description: "Throws drops of blessed water.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.5f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Ocean of Mercy",
        type: SkillType.Ultimate,
        description: "Rains soothing water on an area, healing allies and slowing enemies.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: null,
            Healing: 320f,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Heal, 320f, 0f, 6f, TargetType.AreaAllies, StackType.None, 1, adRatio: null, mdRatio: 1.4f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 30f, 1.5f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // BRIGID (SUPPORT)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Eternal Flame",
        type: SkillType.Shield,
        description: "Brigid's shields burn enemies who strike the protected ally.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 50f,
            Healing: null,
            Shield: 60f,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Shield, 60f, 3f, 0f, TargetType.Ally, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 50f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.4f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Forge Shield",
        type: SkillType.Shield,
        description: "Places a burning shield on a targeted ally.",
        visualExplanation: "Ally.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: null,
            Healing: null,
            Shield: 200f,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Shield, 200f, 0f, 0f, TargetType.Ally, StackType.None, 1, adRatio: null, mdRatio: 0.8f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Ember Breath",
        type: SkillType.Mobility,
        description: "Moves in a burst of sparks.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Sacred Embers",
        type: SkillType.Damage,
        description: "Throws sparks from the forge.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 60f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 60f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: null, mdRatio: 0.5f, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Inspiring Blaze",
        type: SkillType.Ultimate,
        description: "Sets an area ablaze: strengthens allies present and burns enemies over time.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: 6f,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Buff, 20f, 6f, 6f, TargetType.AreaAllies, StackType.Refresh, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: BuffType.AttackDamage, debuffType: null),
            Effect(EffectType.DamageOverTime, 180f, 6f, 6f, TargetType.AreaEnemies, StackType.Refresh, 1, adRatio: null, mdRatio: 0.9f, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: true, buffType: null, debuffType: null),
        });

    // =====================================================
    // ARTEMIS (RANGER)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Huntress's Eye",
        type: SkillType.Debuff,
        description: "Repeated shots on the same target mark it, revealing its position.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Debuff, 0f, 3f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: DebuffType.VisionReduction),
            Effect(EffectType.Vision, 0f, 3f, 0f, TargetType.Enemy, StackType.RefreshDuration, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Piercing Arrow",
        type: SkillType.Damage,
        description: "Looses a shaft that pierces enemies in a line.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 253f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 253f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.2f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Doe's Leap",
        type: SkillType.Mobility,
        description: "A light leap to reposition.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Lunar Shots",
        type: SkillType.Damage,
        description: "Fast, precise shots at long range.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 69f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 69f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.65f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Arrow of the Moon",
        type: SkillType.Ultimate,
        description: "Aims at a designated target and fells it with a shot that executes if weakened.",
        visualExplanation: "Locked.",
        baseStats: new BaseStatsParams(
            Cooldown: 72f,
            ManaCost: 110f,
            Damage: 460f,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Execute, 460f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 2.0f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // RAMA (RANGER)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Prince's Precision",
        type: SkillType.Buff,
        description: "Rama's shots on a distant target deal bonus damage.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackDamage, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Blazing Shaft",
        type: SkillType.Damage,
        description: "Looses a fire arrow that crosses a line and burns over time.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 253f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 253f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.2f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.DamageOverTime, 60f, 3f, 0f, TargetType.AreaEnemies, StackType.RefreshDuration, 1, adRatio: 0.3f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Prince's Step",
        type: SkillType.Mobility,
        description: "Hops back lightly while still firing.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: 150f,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Damage, 150f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.7f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Arrows of Kodanda",
        type: SkillType.Damage,
        description: "Powerful shots at very long range.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 69f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 69f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.65f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Brahmastra",
        type: SkillType.Ultimate,
        description: "Draws his bow and releases the divine arrow, crossing the whole map in a line and executing the weak.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 72f,
            ManaCost: 110f,
            Damage: 460f,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 460f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 2.0f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Execute, 150f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 0.6f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // HOUYI (RANGER)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Nine Suns",
        type: SkillType.Damage,
        description: "Every arrow builds heat which, at a threshold, ignites the target.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.DamageOverTime, 40f, 3f, 0f, TargetType.Enemy, StackType.Additive, 3, adRatio: 0.2f, mdRatio: null, hpRatio: null, isPeriodic: true, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Solar Arrow",
        type: SkillType.Damage,
        description: "Looses a piercing shaft of fire in a line.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 253f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 253f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.2f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Hunter's Roll",
        type: SkillType.Mobility,
        description: "A quick roll to dodge.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Burning Arrows",
        type: SkillType.Damage,
        description: "Scorching shots at long range.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 69f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 69f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.65f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Volley of Ten Suns",
        type: SkillType.Ultimate,
        description: "Fires a rain of blazing arrows onto a targeted area.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 80f,
            ManaCost: 120f,
            Damage: 460f,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Damage, 460f, 0f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 2.0f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // ULLR (RANGER)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Winter's Favor",
        type: SkillType.Buff,
        description: "Alternating bow and blades strengthens Ullr's next form.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Buff, 0f, 0f, 0f, TargetType.Self, StackType.Additive, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: BuffType.AttackSpeed, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Piercing Shaft",
        type: SkillType.Damage,
        description: "Looses a shaft that pierces a line of enemies.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 253f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 253f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.2f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Icy Glide",
        type: SkillType.Mobility,
        description: "Glides on his skis toward a direction.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Yew Arrows",
        type: SkillType.Damage,
        description: "Precise shots at long range.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 69f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 69f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.65f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Duelist's Challenge",
        type: SkillType.Ultimate,
        description: "Draws his bow for a charged shot at very long range: high damage that ignores part of the target's resistances. A one-sided duel, not an execute on a weakened target.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: 75f,
            ManaCost: 110f,
            Damage: 380f,
            Healing: null,
            Shield: null,
            CastTime: 0.8f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 10f),
        effects: new[]
        {
            Effect(EffectType.Damage, 380f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 1.7f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    // =====================================================
    // NEITH (RANGER)
    // =====================================================

    await CreateAsync(ct, system,
        name: "Thread of Fate",
        type: SkillType.Debuff,
        description: "Neith's shots leave a thread on the target; striking an already-tethered target restrains it further.",
        visualExplanation: "No aim.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 3.5f),
        effects: new[]
        {
            Effect(EffectType.Debuff, 0f, 3f, 0f, TargetType.Enemy, StackType.Additive, 2, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: DebuffType.MovementSpeedReduction),
        });

    await CreateAsync(ct, system,
        name: "Hunting Shaft",
        type: SkillType.Damage,
        description: "Looses a piercing arrow in a line.",
        visualExplanation: "Line.",
        baseStats: new BaseStatsParams(
            Cooldown: 7f,
            ManaCost: 55f,
            Damage: 253f,
            Healing: null,
            Shield: null,
            CastTime: 0.4f,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 8f),
        effects: new[]
        {
            Effect(EffectType.Damage, 253f, 0f, 0f, TargetType.AreaEnemies, StackType.None, 1, adRatio: 1.2f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Weaver's Step",
        type: SkillType.Mobility,
        description: "Retreats while weaving a thread that slows pursuers.",
        visualExplanation: "Movement.",
        baseStats: new BaseStatsParams(
            Cooldown: 12f,
            ManaCost: 60f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.2f,
            ChannelDuration: null,
            CrowdControlDuration: 1f,
            Range: 5f),
        effects: new[]
        {
            Effect(EffectType.Mobility, 0f, 0f, 0f, TargetType.Self, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.Slow, 30f, 1f, 4f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Woven Arrows",
        type: SkillType.Damage,
        description: "Long-range shots fletched with thread.",
        visualExplanation: "Auto-target.",
        baseStats: new BaseStatsParams(
            Cooldown: null,
            ManaCost: null,
            Damage: 69f,
            Healing: null,
            Shield: null,
            CastTime: null,
            ChannelDuration: null,
            CrowdControlDuration: null,
            Range: 6.0f),
        effects: new[]
        {
            Effect(EffectType.Damage, 69f, 0f, 0f, TargetType.Enemy, StackType.None, 1, adRatio: 0.65f, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
        });

    await CreateAsync(ct, system,
        name: "Web of the World",
        type: SkillType.Ultimate,
        description: "Weaves a web on the ground over an area: enemies caught are rooted.",
        visualExplanation: "Zone.",
        baseStats: new BaseStatsParams(
            Cooldown: 62f,
            ManaCost: 85f,
            Damage: null,
            Healing: null,
            Shield: null,
            CastTime: 0.6f,
            ChannelDuration: null,
            CrowdControlDuration: 1.5f,
            Range: 6f),
        effects: new[]
        {
            Effect(EffectType.Root, 0f, 1.5f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: false, isChannelled: false, buffType: null, debuffType: null),
            Effect(EffectType.ZoneControl, 0f, 1.5f, 6f, TargetType.AreaEnemies, StackType.None, 1, adRatio: null, mdRatio: null, hpRatio: null, isPeriodic: false, isInstant: true, isChannelled: false, buffType: null, debuffType: null),
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

    await _skills.AddAsync(skill, ct);

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
      await _effects.AddAsync(effect, ct);
    }

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