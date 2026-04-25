using HeroService.Domain.Heroes.Versioned.Snapshotting;

public class HeroSnapshot
{
  public Guid HeroId { get; }
  public Guid GameVersionId { get; }

  public float Health { get; }
  public float Mana { get; }
  public float AttackDamage { get; }

  public IReadOnlyList<SkillSnapshot> Skills { get; }

  public HeroSnapshot(
    Guid heroId,
    Guid gameVersionId,
    float health,
    float mana,
    float attackDamage,
    IReadOnlyList<SkillSnapshot> skills)
  {
    HeroId = heroId;
    GameVersionId = gameVersionId;
    Health = health;
    Mana = mana;
    AttackDamage = attackDamage;
    Skills = skills;
  }

  // ---- DOMAIN BEHAVIOR (CORRECT PLACE) ----

  public float TotalSkillDamage()
    => Skills.Sum(s => s.Damage);

  public float TotalManaCost()
    => Skills.Sum(s => s.ManaCost);

  public bool CanCastAnySkill(float currentMana)
    => Skills.Any(s => s.ManaCost <= currentMana);

  public SkillSnapshot? GetSkill(Guid skillId)
    => Skills.FirstOrDefault(s => s.SkillId == skillId);
}