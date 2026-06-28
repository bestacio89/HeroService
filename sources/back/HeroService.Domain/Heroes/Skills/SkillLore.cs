using System;

namespace HeroService.Domain.Heroes.Skills;

/// <summary>
/// Represents the narrative and descriptive identity of a Skill.
/// </summary>
public sealed class SkillLore : Entity<Guid>
{
  public Guid SkillId { get; private set; }

  public string Description { get; private set; } = string.Empty;
  public string VisualExplanation { get; private set; } = string.Empty;

  protected SkillLore(Guid id) : base(id) { }


  /// <summary>
  /// Initial creation of lore (single source of truth for instantiation).
  /// </summary>
  public void Define(
     Guid skillId,
     string description,
     string visualExplanation,
     string createdBy)
  {
    if (SkillId != Guid.Empty)
      throw new InvalidOperationException("SkillLore already defined.");

    if (string.IsNullOrWhiteSpace(description))
      throw new ArgumentException("Skill description cannot be empty.");

    SkillId = skillId;
    Description = description;
    VisualExplanation = visualExplanation;

    MarkCreated(createdBy);
  }

  /// <summary>
  /// Controlled update (UI-safe mutation, no gameplay impact).
  /// </summary>
  public void Update(
      string description,
      string visualExplanation,
      string updatedBy)
  {
    if (string.IsNullOrWhiteSpace(description))
      throw new ArgumentException("Skill description cannot be empty.");

  
    Description = description;
    VisualExplanation = visualExplanation;

    MarkUpdated(updatedBy);
  }
}