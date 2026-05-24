using System;
using System.Collections.Generic;

namespace HeroService.Domain.Heroes.Skills;

public class Skill : Entity<Guid>
{
  public string Name { get; private set; } = string.Empty;

  public SkillType SkillType { get; private set; }

  private readonly List<SkillEffect> _effects = new();
  public IReadOnlyCollection<SkillEffect> Effects => _effects;

  private Skill() { }

  public void Define(
      string name,
      SkillType skillType,
      string createdBy)
  {
    // =========================
    // Guard: prevent re-definition
    // =========================
    if (!string.IsNullOrWhiteSpace(Name))
      throw new InvalidOperationException(
          "Skill is already defined.");

    // =========================
    // Validation
    // =========================
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException(
          "Skill name cannot be empty.",
          nameof(name));

    // =========================
    // Assignment
    // =========================
    Name = name;
    SkillType = skillType;

    // =========================
    // Audit
    // =========================
    MarkCreated(createdBy);
  }

  public void Redefine(
      string name,
      SkillType skillType,
      string updatedBy)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException(
          "Skill name cannot be empty.",
          nameof(name));

    Name = name;
    SkillType = skillType;

    MarkUpdated(updatedBy);
  }

  public void AddEffect(SkillEffect effect)
  {
    ArgumentNullException.ThrowIfNull(effect);

    if (effect.SkillId != Id)
      throw new InvalidOperationException(
          "Cannot attach an effect to a different Skill.");

    _effects.Add(effect);
  }

  public void RemoveEffect(Guid effectId, string updatedBy)
  {
    var effect = _effects.FirstOrDefault(e => e.Id == effectId);

    if (effect is null)
      throw new InvalidOperationException(
          $"Effect '{effectId}' was not found.");

    _effects.Remove(effect);

    MarkUpdated(updatedBy);
  }
}