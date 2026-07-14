namespace HeroService.Domain.Heroes.Skills;

public class SkillEffect : Entity<Guid>
{
  public Guid SkillId { get; private set; }

  public EffectType EffectType { get; private set; }

  // =========================================================
  // STATE MODIFIER SPECIALIZATION
  // =========================================================
  public BuffType? BuffType { get; private set; }

  public DebuffType? DebuffType { get; private set; }


  public float Magnitude { get; private set; }
  public float Duration { get; private set; }
  public float Radius { get; private set; }


  public float? AttackDamageRatio { get; private set; }
  public float? MagicDamageRatio { get; private set; }
  public float? MaxHealthRatio { get; private set; }


  public bool IsPeriodic { get; private set; }
  public bool IsInstant { get; private set; }
  public bool IsChannelled { get; private set; }


  public TargetType TargetType { get; private set; }
  public StackType StackType { get; private set; }
  public int MaxStacks { get; private set; }


  public int Revision { get; private set; }


  protected SkillEffect(Guid id) : base(id)
  {
  }


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
    float? mdRatio,
    float? hpRatio,
    bool isPeriodic,
    bool isInstant,
    bool isChannelled,
    BuffType? buffType,
    DebuffType? debuffType,
    string createdBy)
  {
    if (SkillId != Guid.Empty)
      throw new InvalidOperationException(
        "SkillEffect already defined. Use Redefine for balance changes.");


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
      mdRatio,
      hpRatio,
      isPeriodic,
      isInstant,
      isChannelled,
      buffType,
      debuffType,
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
    float? mdRatio,
    float? hpRatio,
    bool isPeriodic,
    bool isInstant,
    bool isChannelled,
    BuffType? buffType,
    DebuffType? debuffType,
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
      mdRatio,
      hpRatio,
      isPeriodic,
      isInstant,
      isChannelled,
      buffType,
      debuffType,
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
    float? mdRatio,
    float? hpRatio,
    bool isPeriodic,
    bool isInstant,
    bool isChannelled,
    BuffType? buffType,
    DebuffType? debuffType,
    string actor)
  {
    if (skillId == Guid.Empty)
      throw new ArgumentException(
        "SkillId cannot be empty.");


    ValidateModifierTypes(
      effectType,
      buffType,
      debuffType);


    SkillId = skillId;

    EffectType = effectType;

    BuffType = buffType;
    DebuffType = debuffType;


    Magnitude = magnitude;
    Duration = duration;
    Radius = radius;


    TargetType = targetType;
    StackType = stackType;
    MaxStacks = maxStacks;


    AttackDamageRatio = adRatio;
    MagicDamageRatio = mdRatio;
    MaxHealthRatio = hpRatio;


    IsPeriodic = isPeriodic;
    IsInstant = isInstant;
    IsChannelled = isChannelled;


    MarkCreated(actor);
  }


  private static void ValidateModifierTypes(
    EffectType effectType,
    BuffType? buffType,
    DebuffType? debuffType)
  {
    // A buff type can only exist on a buff effect.
    if (buffType is not null &&
        effectType != EffectType.Buff)
    {
      throw new InvalidOperationException(
          "Only Buff effects can define a BuffType.");
    }


    // A debuff type can only exist on a debuff effect.
    if (debuffType is not null &&
        effectType != EffectType.Debuff)
    {
      throw new InvalidOperationException(
          "Only Debuff effects can define a DebuffType.");
    }


    // Buff and Debuff cannot coexist.
    if (buffType is not null &&
        debuffType is not null)
    {
      throw new InvalidOperationException(
          "An effect cannot define both BuffType and DebuffType.");
    }
  }
}