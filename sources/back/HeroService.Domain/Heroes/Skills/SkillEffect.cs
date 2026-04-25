namespace HeroService.Domain.Heroes.Skills;

public class SkillEffect : Entity<Guid>
{
  public Guid SkillId { get; private set; }
  public EffectType EffectType { get; private set; }

  private SkillEffect() { }

  public SkillEffect(Guid skillId, EffectType effectType, string createdBy)
  {
    SkillId = skillId;
    EffectType = effectType;
    MarkCreated(createdBy);
  }
}