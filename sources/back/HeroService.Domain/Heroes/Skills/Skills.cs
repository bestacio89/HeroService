namespace HeroService.Domain.Heroes.Skills;

public class Skill : Entity<Guid>
{
   public string Name { get; private set; } = string.Empty;

  public SkillType SkillType { get; private set; }

  private readonly List<SkillEffect> _effects = new();
  public IReadOnlyCollection<SkillEffect> Effects => _effects;

  private Skill() { }

  public Skill( string name, SkillType skillType, string createdBy)
  {
    
    Name = name;
    SkillType = skillType;
    MarkCreated(createdBy);
  }

  public void AddEffect(SkillEffect effect)
  {
    _effects.Add(effect);
  }
}