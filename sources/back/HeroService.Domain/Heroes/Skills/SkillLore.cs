using System;

namespace HeroService.Domain.Heroes.Skills;

/// <summary>
/// Represents the narrative and descriptive identity of a Skill.
/// </summary>
public sealed class SkillLore : Entity<Guid>
{
  public Guid SkillId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string Description { get; private set; } = string.Empty;
  public string VisualExplanation { get; private set; } = string.Empty;

  private SkillLore() { }

  /// <summary>
  /// Initial creation of lore (single source of truth for instantiation).
  /// </summary>
  public void Define(
     Guid skillId,
     string name,
     string description,
     string visualExplanation,
     string createdBy)
  {
    if (SkillId != Guid.Empty)
      throw new InvalidOperationException("SkillLore already defined.");

    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Skill name cannot be empty.");

    SkillId = skillId;
    Name = name;
    Description = description;
    VisualExplanation = visualExplanation;

    MarkCreated(createdBy);
  }

  /// <summary>
  /// Controlled update (UI-safe mutation, no gameplay impact).
  /// </summary>
  public void Update(
      string name,
      string description,
      string visualExplanation,
      string updatedBy)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Skill name cannot be empty.");

    Name = name;
    Description = description;
    VisualExplanation = visualExplanation;

    MarkUpdated(updatedBy);
  }
}