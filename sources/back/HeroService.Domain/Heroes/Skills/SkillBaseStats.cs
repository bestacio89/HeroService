namespace HeroService.Domain.Heroes.Skills;

public class SkillBaseStats : Entity<Guid>
{
  public Guid SkillId { get; private set; }

  public float? BaseCooldown { get; private set; }
  public float? BaseManaCost { get; private set; }

  public float? BaseDamage { get; private set; }
  public float? BaseHealing { get; private set; }
  public float? BaseShieldValue { get; private set; }

  public float? BaseCastTime { get; private set; }
  public float? BaseChannelDuration { get; private set; }

  public float? BaseCrowdControlDuration { get; private set; }
  public float? BaseRange { get; private set; }

  protected SkillBaseStats(Guid id) : base(id) { }

  public void Define(
    Guid skillId,
    float? baseCooldown,
    float? baseManaCost,
    float? baseDamage,
    float? baseHealing,
    float? baseShieldValue,
    float? baseCastTime,
    float? baseChannelDuration,
    float? baseCrowdControlDuration,
    float? baseRange,
    string createdBy)
  {
    if (SkillId != Guid.Empty)
      throw new InvalidOperationException("Already defined.");

    if (skillId == Guid.Empty)
      throw new ArgumentException("SkillId required.");

    SkillId = skillId;

    BaseCooldown = Validate(baseCooldown);
    BaseManaCost = Validate(baseManaCost);

    BaseDamage = Validate(baseDamage);
    BaseHealing = Validate(baseHealing);
    BaseShieldValue = Validate(baseShieldValue);

    BaseCastTime = Validate(baseCastTime);
    BaseChannelDuration = Validate(baseChannelDuration);

    BaseCrowdControlDuration = Validate(baseCrowdControlDuration);
    BaseRange = Validate(baseRange);

    MarkCreated(createdBy);
  }

  private static float? Validate(float? v)
  {
    if (v is null)
      return null;

    if (v < 0)
      throw new ArgumentOutOfRangeException(nameof(v));

    return v;
  }
}