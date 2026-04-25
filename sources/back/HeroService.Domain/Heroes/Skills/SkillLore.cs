using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Domain.Heroes.Skills;

public sealed class SkillLore : Entity<Guid>
{
  public Guid SkillId { get; private set; }

  public string Name { get; private set; }
  public string Description { get; private set; } // “what it is”
  public string VisualExplanation { get; private set; } // optional

  private SkillLore() { }

  public SkillLore(Guid skillId, string name, string description, string visualExplanation)
  {
    SkillId = skillId;
    Name = name;
    Description = description;
    VisualExplanation = visualExplanation;

    MarkCreated("system");
  }
}