namespace HeroService.Domain.Heroes.Skills;

public class SkillBaseStats : Entity<Guid>
{
  public Guid SkillId { get; private set; }

  public float BaseCooldown { get; private set; }
  public float BaseManaCost { get; private set; }
  public float BaseDamage { get; private set; }

  private SkillBaseStats() { }

  public SkillBaseStats(
    Guid skillId,
    float cooldown,
    float manaCost,
    float damage)
  {
    SkillId = skillId;
    BaseCooldown = cooldown;
    BaseManaCost = manaCost;
    BaseDamage = damage;
  }
}