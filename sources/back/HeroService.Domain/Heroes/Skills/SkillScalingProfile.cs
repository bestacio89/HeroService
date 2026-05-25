namespace HeroService.Domain.Heroes.Skills;

/// <summary>
/// Defines how a Skill scales with Hero stats.
/// This is NOT base data — it's interaction logic.
/// </summary>
public class SkillScalingProfile : Entity<Guid>
{
  public Guid SkillId { get; private set; }

  // =========================
  // HERO STAT SCALING
  // =========================
  public float AttackDamageRatio { get; private set; }
  public float AbilityPowerRatio { get; private set; }
  public float MaxHealthRatio { get; private set; }

  private SkillScalingProfile() { }

  public void Define(
    Guid skillId,
    float attackDamageRatio,
    float abilityPowerRatio,
    float maxHealthRatio,
    string createdBy)
  {
    if (SkillId != Guid.Empty)
      throw new InvalidOperationException("Already defined.");

    if (skillId == Guid.Empty)
      throw new ArgumentException("SkillId required.");

    SkillId = skillId;

    AttackDamageRatio = attackDamageRatio;
    AbilityPowerRatio = abilityPowerRatio;
    MaxHealthRatio = maxHealthRatio;

    MarkCreated(createdBy);
  }
}