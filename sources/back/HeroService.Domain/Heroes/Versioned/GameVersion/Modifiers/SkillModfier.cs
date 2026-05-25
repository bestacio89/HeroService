using System;

namespace HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

public class SkillModifier : Entity<Guid>
{
  public Guid GameVersionId { get; private set; }
  public Guid SkillId { get; private set; }

  // COSTS
  public float CooldownMultiplier { get; private set; }
  public float ManaCostMultiplier { get; private set; }

  // OUTPUT
  public float DamageMultiplier { get; private set; }
  public float HealingMultiplier { get; private set; }
  public float ShieldMultiplier { get; private set; }

  // TEMPO
  public float CastTimeMultiplier { get; private set; }
  public float ChannelDurationMultiplier { get; private set; }

  // UTILITY
  public float CrowdControlDurationMultiplier { get; private set; }
  public float RangeMultiplier { get; private set; }

  private SkillModifier() { }

  // =========================================================
  // DEFINE (FACTORY ENTRY POINT)
  // =========================================================
  public void Define(
    Guid gameVersionId,
    Guid skillId,

    float cooldownMultiplier,
    float manaCostMultiplier,

    float damageMultiplier,
    float healingMultiplier,
    float shieldMultiplier,

    float castTimeMultiplier,
    float channelDurationMultiplier,

    float crowdControlDurationMultiplier,
    float rangeMultiplier,

    string createdBy)
  {
    if (GameVersionId != Guid.Empty)
      throw new InvalidOperationException("SkillModifier already defined.");

    if (gameVersionId == Guid.Empty)
      throw new ArgumentException("GameVersionId is required.");

    if (skillId == Guid.Empty)
      throw new ArgumentException("SkillId is required.");

    GameVersionId = gameVersionId;
    SkillId = skillId;

    // =========================
    // VALIDATION RULE: NO ZERO OR NEGATIVE
    // =========================
    CooldownMultiplier = ValidatePositive(cooldownMultiplier, nameof(cooldownMultiplier));
    ManaCostMultiplier = ValidatePositive(manaCostMultiplier, nameof(manaCostMultiplier));

    DamageMultiplier = ValidatePositive(damageMultiplier, nameof(damageMultiplier));
    HealingMultiplier = ValidatePositive(healingMultiplier, nameof(healingMultiplier));
    ShieldMultiplier = ValidatePositive(shieldMultiplier, nameof(shieldMultiplier));

    CastTimeMultiplier = ValidatePositive(castTimeMultiplier, nameof(castTimeMultiplier));
    ChannelDurationMultiplier = ValidatePositive(channelDurationMultiplier, nameof(channelDurationMultiplier));

    CrowdControlDurationMultiplier = ValidatePositive(crowdControlDurationMultiplier, nameof(crowdControlDurationMultiplier));
    RangeMultiplier = ValidatePositive(rangeMultiplier, nameof(rangeMultiplier));

    MarkCreated(createdBy);
  }

  // =========================================================
  // VALIDATION
  // =========================================================
  private static float ValidatePositive(float value, string name)
  {
    if (value <= 0f)
      throw new ArgumentOutOfRangeException(
        name,
        $"{name} must be greater than 0.");

    return value;
  }
}