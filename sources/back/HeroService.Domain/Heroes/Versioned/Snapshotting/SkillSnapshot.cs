using System;

namespace HeroService.Domain.Heroes.Versioned.Snapshotting;

/// <summary>
/// Represents the fully resolved, deterministic execution state of a Skill
/// within a specific GameVersion and Hero context.
///
/// Domain Role:
/// SkillSnapshot is the final compiled combat representation of a Skill
/// after applying:
/// - SkillBaseStats (definition layer)
/// - SkillEffects (behavioral layer)
/// - GameVersion modifiers (balance layer)
/// - Hero context scaling (execution layer)
///
/// It is the **runtime-ready artifact used by the combat simulation engine**.
///
/// Matchmaking Relevance:
/// - Direct contributor to Hero power evaluation.
/// - Used to compute:
///   • Burst potential
///   • Sustained DPS windows
///   • Utility impact (CC, mobility, buffs)
///   • Resource efficiency (mana vs impact ratio)
/// - Aggregated into HeroSnapshot for matchmaking scoring.
///
/// Invariants:
/// - SkillId must reference a valid Skill.
/// - GameVersionId must match active simulation context.
/// - Snapshot is immutable once created.
/// - All values are fully resolved (no deferred computation).
///
/// Relationships:
/// - Derived from:
///   • SkillBaseStats
///   • SkillEffects
///   • GameVersion modifiers
///   • Hero scaling context (optional injection via SnapshotResolver)
/// - Contained within:
///   • HeroSnapshot
/// - Consumed by:
///   • Combat simulation engine
///   • Matchmaking system
///   • Replay/determinism validation
///
/// Versioning / Snapshot Impact:
/// - CRITICAL to game balance integrity.
/// - Any change in SkillBaseStats or SkillEffects requires regeneration.
/// - Guarantees deterministic replay of combat outcomes.
///
/// Developer Notes:
/// - This is NOT a DTO.
/// - This is NOT a raw stat container.
/// - This is a compiled simulation artifact.
/// - Behavior must NOT be added here; only resolved results.
///
/// Architectural Insight:
/// - SkillBaseStats = definition
/// - SkillEffects = behavior semantics
/// - GameVersion modifiers = balance tuning
/// - SkillSnapshot = final deterministic execution form
/// </summary>
public class SkillSnapshot
{
  public Guid SkillId { get; }
  public Guid GameVersionId { get; }

  // =========================
  // CORE EXECUTION STATS
  // =========================
  public float Cooldown { get; }
  public float ManaCost { get; }
  public float Damage { get; }
  public float Healing { get; }
  public float ShieldValue { get; }

  // =========================
  // EXECUTION BEHAVIOR
  // =========================
  public float CastTime { get; }
  public float ChannelDuration { get; }
  public float Range { get; }

  // =========================
  // SCALING RESULT (RESOLVED)
  // =========================
  public float AttackDamageScalingContribution { get; }
  public float AbilityPowerScalingContribution { get; }
  public float MaxHealthScalingContribution { get; }

  // =========================
  // EFFECT SEMANTICS (FROM SkillEffects)
  // =========================
  public bool HasDamage { get; }
  public bool HasHealing { get; }
  public bool HasShielding { get; }
  public bool HasCrowdControl { get; }
  public bool HasMobility { get; }
  public bool IsBuff { get; }
  public bool IsDebuff { get; }
  public bool IsUltimate { get; }

  // =========================
  // CONTROL PARAMETERS (SIMULATION RELEVANT)
  // =========================
  public float CrowdControlDuration { get; }

  public SkillSnapshot(
    Guid skillId,
    Guid gameVersionId,
    float cooldown,
    float manaCost,
    float damage,
    float healing,
    float shieldValue,
    float castTime,
    float channelDuration,
    float range,
    float attackDamageScalingContribution,
    float abilityPowerScalingContribution,
    float maxHealthScalingContribution,
    bool hasDamage,
    bool hasHealing,
    bool hasShielding,
    bool hasCrowdControl,
    bool hasMobility,
    bool isBuff,
    bool isDebuff,
    bool isUltimate,
    float crowdControlDuration)
  {
    SkillId = skillId;
    GameVersionId = gameVersionId;

    Cooldown = cooldown;
    ManaCost = manaCost;

    Damage = damage;
    Healing = healing;
    ShieldValue = shieldValue;

    CastTime = castTime;
    ChannelDuration = channelDuration;
    Range = range;

    AttackDamageScalingContribution = attackDamageScalingContribution;
    AbilityPowerScalingContribution = abilityPowerScalingContribution;
    MaxHealthScalingContribution = maxHealthScalingContribution;

    HasDamage = hasDamage;
    HasHealing = hasHealing;
    HasShielding = hasShielding;
    HasCrowdControl = hasCrowdControl;
    HasMobility = hasMobility;

    IsBuff = isBuff;
    IsDebuff = isDebuff;
    IsUltimate = isUltimate;

    CrowdControlDuration = crowdControlDuration;
  }
}