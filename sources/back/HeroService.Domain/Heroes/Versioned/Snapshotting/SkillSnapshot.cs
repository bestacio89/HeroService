using System;

namespace HeroService.Domain.Heroes.Versioned.Snapshotting;

public class SkillSnapshot
{
  public Guid SkillId { get; }
  public Guid GameVersionId { get; }

  public float Cooldown { get; }
  public float ManaCost { get; }
  public float Damage { get; }

  public SkillSnapshot(
    Guid skillId,
    Guid gameVersionId,
    float cooldown,
    float manaCost,
    float damage)
  {
    SkillId = skillId;
    GameVersionId = gameVersionId;
    Cooldown = cooldown;
    ManaCost = manaCost;
    Damage = damage;
  }
}