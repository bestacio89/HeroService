using System;
using System.Collections.Generic;

namespace HeroService.Domain.Heroes.Skills;

/// <summary>
/// Represents the aggregate root definition of a Skill.
///
/// Domain Role:
/// Skill is the primary structural entity that defines an ability in the game.
/// It acts as a container for all mechanical and semantic components required
/// to fully describe a combat ability in a deterministic simulation system.
///
/// A Skill is composed of:
/// - SkillBaseStats (numerical baseline)
/// - SkillEffects (behavioral outcomes)
/// - SkillType (high-level classification)
///
/// This entity does NOT execute logic. It only defines structure.
///
/// Matchmaking Relevance:
/// - Indirectly influences matchmaking through simulation of Hero kits.
/// - Used to evaluate:
///   • Damage profile distribution (burst vs sustain)
///   • Utility vs combat contribution
///   • Crowd control density
///   • Team synergy potential
///
/// - Combined with HeroSkillKit to assess:
///   • Hero combat role effectiveness
///   • Ability kit balance within team compositions
///
/// Invariants:
/// - Name must be non-empty and unique within design context.
/// - SkillType must be a valid predefined classification.
/// - A Skill may contain multiple SkillEffects (compositional model).
/// - Skill is immutable in structure after creation except for controlled effect additions.
///
/// Relationships:
/// - Aggregate root for:
///   • SkillBaseStats
///   • SkillEffect
///   • SkillLore
/// - Consumed by:
///   • SnapshotResolver (combat simulation engine)
///   • HeroSkillKit (Hero ability composition)
///   • Matchmaking evaluation systems
///
/// Versioning / Snapshot Impact:
/// - Highly sensitive to changes in:
///   • Effects
///   • Base stats
///   • SkillType classification
/// - Any modification affects:
///   • Combat outcomes
///   • Balance calculations
///   • Meta stability
/// - Must always be included in snapshot generation pipelines.
///
/// Developer Notes:
/// - This is a STRUCTURAL aggregate, not a behavioral system.
/// - DO NOT implement combat logic inside this class.
/// - DO NOT infer runtime behavior from SkillType.
/// - All execution logic must be handled in SnapshotResolver.
/// - Effects are compositional; order should not imply execution logic unless explicitly defined in resolver.
///
/// Architectural Insight:
/// - Skill is the “definition container” of an ability.
/// - SkillEffect defines behavior pieces.
/// - SkillBaseStats defines numeric strength.
/// - SkillLore defines narrative identity.
/// - Together, they form a fully deterministic ability specification.
/// </summary>
public class Skill : Entity<Guid>
{
  public string Name { get; private set; } = string.Empty;

  public SkillType SkillType { get; private set; }

  private readonly List<SkillEffect> _effects = new();
  public IReadOnlyCollection<SkillEffect> Effects => _effects;

  private Skill() { }

  public Skill(string name, SkillType skillType, string createdBy)
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