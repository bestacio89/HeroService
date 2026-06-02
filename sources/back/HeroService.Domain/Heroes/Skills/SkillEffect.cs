using HeroService.Domain.Heroes.Skills;

/// <summary>
/// Represents a behavioral effect attached to a Skill.
///
/// Domain Role:
/// Defines the *functional outcome category* of a Skill, describing what the ability
/// actually does in gameplay terms (e.g., Damage, Heal, Shield, Crowd Control, Mobility).
///
/// Unlike SkillBaseStats, which defines numerical properties,
/// SkillEffect defines **behavioral intent and resolution type**.
///
/// This entity is the primary mechanism through which Skills gain expressive diversity.
///
/// Matchmaking Relevance:
/// - Indirect but structurally important for simulation and role evaluation.
/// - Used to:
///   • Classify Skill behavior during combat simulation.
///   • Identify team composition capabilities (damage, sustain, control, mobility).
///   • Support synergy detection between Heroes and Skills.
/// - Enables abstraction for matchmaking heuristics such as:
///   • Crowd control density
///   • Mobility availability
///   • Burst vs sustain balance
///
/// Invariants:
/// - SkillId must reference a valid Skill aggregate.
/// - EffectType must be a valid, predefined classification (closed set).
/// - A Skill may contain multiple SkillEffects (compositional behavior).
/// - Each effect must remain stateless and deterministic in definition.
///
/// Relationships:
/// - Belongs to a Skill aggregate.
/// - Works in conjunction with:
///   • SkillBaseStats (numerical definition)
///   • SnapshotResolver (effect resolution engine)
///   • HeroSkillKit (execution context on Hero)
///
/// Versioning / Snapshot Impact:
/// - Moderate to high impact depending on EffectType.
/// - Changes to effect composition can alter:
///   • Combat flow
///   • Hero role classification
///   • Matchmaking evaluation results
/// - Must be included in snapshot resolution outputs to ensure deterministic simulation.
///
/// Developer Notes:
/// - This is a **composition point, not a behavior implementation**.
/// - Do NOT encode logic inside EffectType itself; it must remain declarative.
/// - Actual effect execution (damage, CC, mobility resolution) belongs in the
///   SnapshotResolver or dedicated effect processors.
/// - EffectType should be treated as a contract for downstream simulation engines.
///
/// Architectural Insight:
/// - SkillEffect is where your system gains flexibility without losing determinism.
/// - It acts as a bridge between:
///   • Static numerical definitions (SkillBaseStats)
///   • Dynamic gameplay behavior (SnapshotResolver execution layer)
/// - This is the primary extensibility mechanism for introducing new gameplay mechanics
///   without modifying core combat systems.
/// </summary>
 namespace HeroService.Domain.Heroes.Skills;

public class SkillEffect : Entity<Guid>
{
  public Guid SkillId { get; private set; }
  public EffectType EffectType { get; private set; }

  public float Magnitude { get; private set; }
  public float Duration { get; private set; }
  public float Radius { get; private set; }

  public float? AttackDamageRatio { get; private set; }
  public float? AbilityPowerRatio { get; private set; }
  public float? MaxHealthRatio { get; private set; }

  public bool IsPeriodic { get; private set; }
  public bool IsInstant { get; private set; }
  public bool IsChannelled { get; private set; }

  public TargetType TargetType { get; private set; }
  public StackType StackType { get; private set; }
  public int MaxStacks { get; private set; }

  public int Revision { get; private set; }   // 👈 IMPORTANT for balancing

  private SkillEffect() { }

  public void Define(
    Guid skillId,
    EffectType effectType,
    float magnitude,
    float duration,
    float radius,
    TargetType targetType,
    StackType stackType,
    int maxStacks,
    float? adRatio,
    float? apRatio,
    float? hpRatio,
    bool isPeriodic,
    bool isInstant,
    bool isChannelled,
    string createdBy)
  {
    if (SkillId != Guid.Empty)
      throw new InvalidOperationException("SkillEffect already defined. Use Redefine for balance changes.");

    ApplyDefinition(
      skillId,
      effectType,
      magnitude,
      duration,
      radius,
      targetType,
      stackType,
      maxStacks,
      adRatio,
      apRatio,
      hpRatio,
      isPeriodic,
      isInstant,
      isChannelled,
      createdBy
    );

    Revision = 1;
  }

  public void Redefine(
    EffectType effectType,
    float magnitude,
    float duration,
    float radius,
    TargetType targetType,
    StackType stackType,
    int maxStacks,
    float? adRatio,
    float? apRatio,
    float? hpRatio,
    bool isPeriodic,
    bool isInstant,
    bool isChannelled,
    string updatedBy)
  {
    ApplyDefinition(
      SkillId,
      effectType,
      magnitude,
      duration,
      radius,
      targetType,
      stackType,
      maxStacks,
      adRatio,
      apRatio,
      hpRatio,
      isPeriodic,
      isInstant,
      isChannelled,
      updatedBy
    );

    Revision++;
  }

  private void ApplyDefinition(
    Guid skillId,
    EffectType effectType,
    float magnitude,
    float duration,
    float radius,
    TargetType targetType,
    StackType stackType,
    int maxStacks,
    float? adRatio,
    float? apRatio,
    float? hpRatio,
    bool isPeriodic,
    bool isInstant,
    bool isChannelled,
    string actor)
  {
    if (skillId == Guid.Empty)
      throw new ArgumentException("SkillId cannot be empty.");

    SkillId = skillId;
    EffectType = effectType;

    Magnitude = magnitude;
    Duration = duration;
    Radius = radius;

    TargetType = targetType;
    StackType = stackType;
    MaxStacks = maxStacks;

    AttackDamageRatio = adRatio;
    AbilityPowerRatio = apRatio;
    MaxHealthRatio = hpRatio;

    IsPeriodic = isPeriodic;
    IsInstant = isInstant;
    IsChannelled = isChannelled;

    MarkCreated(actor);
  }
}