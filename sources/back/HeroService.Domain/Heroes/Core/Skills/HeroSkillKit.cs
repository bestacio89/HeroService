namespace HeroService.Domain.Heroes.Core.Skills;

public class HeroSkillKit
{
  public Guid PassiveSkillId { get; }

  public Guid PrimarySkillId { get; }
  public Guid SecondarySkillId { get; }
  public Guid TertiarySkillId { get; }
  public Guid UltimateSkillId { get; }

  public HeroSkillKit(
    Guid passiveSkillId,
    Guid primarySkillId,
    Guid secondarySkillId,
    Guid tertiarySkillId,
    Guid ultimateSkillId)
  {
    PassiveSkillId = passiveSkillId;
    PrimarySkillId = primarySkillId;
    SecondarySkillId = secondarySkillId;
    TertiarySkillId = tertiarySkillId;
    UltimateSkillId = ultimateSkillId;
  }
}