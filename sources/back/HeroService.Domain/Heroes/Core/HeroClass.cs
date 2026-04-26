
namespace HeroService.Domain.Heroes.Core;
/// <summary>
/// Represents the archetypal combat role of a Hero.
///
/// Domain Role:
/// Defines the structural gameplay identity of a Hero (e.g., Tank, Assassin, Mage, Support).
/// Unlike raw stats or abilities, HeroClass expresses *intent*—how the Hero is expected
/// to function within combat systems and team composition.
///
/// HeroClass acts as a high-level behavioral archetype that influences scaling,
/// item interactions, and matchmaking evaluation.
///
/// Matchmaking Relevance:
/// - Serves as a primary team composition axis.
/// - Used to:
///   • Ensure balanced distribution of roles within teams.
///   • Evaluate role redundancy or deficiency (e.g., no frontline, no sustain).
///   • Apply class-based item scaling modifiers.
///   • Influence synergy evaluation between Heroes.
///
/// - Works in conjunction with:
///   • HeroBaseStats (numerical strength profile)
///   • HeroSkillKit (behavioral execution)
///   • HeroAffiliation (identity constraints)
///
/// Invariants:
/// - Name must be non-empty and uniquely identify a valid class archetype.
/// - Each Hero must have exactly one HeroClass.
/// - HeroClass values are expected to remain stable once introduced,
///   as changes may impact balance, matchmaking, and item scaling systems.
///
/// Relationships:
/// - Part of the Hero aggregate.
/// - Consumed by:
///   • Matchmaking system (role distribution logic)
///   • Item scaling system (class-based bonuses)
///   • Combat simulation systems
///   • Hero creation and validation workflows
///
/// Versioning / Snapshot Impact:
/// - High-impact classification axis.
/// - Changes to a Hero's HeroClass require snapshot/version updates,
///   as they directly affect:
///   • Team composition evaluation
///   • Item effectiveness
///   • Combat role expectations
/// - Class definitions themselves should be treated as semi-static balance data.
///
/// Developer Notes:
/// - This is not descriptive metadata; it is a functional gameplay axis.
/// - Avoid using free-form strings in logic—use stable identifiers or resolved entities.
/// - Do not overload HeroClass with behavioral logic; it should remain a classification,
///   not a rule engine.
/// - Balance changes here ripple through the entire ecosystem (items, matchmaking, skills).
///
/// Architectural Insight:
/// - HeroClass defines *how the Hero is interpreted by the system*.
/// - Combined with HeroAffiliation (who they are) and HeroBaseStats (how strong they are),
///   it forms the backbone of matchmaking classification.
/// </summary>
public class HeroClass : Entity<Guid>
{
  public string Name { get; private set; } = string.Empty;

  private HeroClass() { }

  public HeroClass(string name, string createdBy)
  {
    Name = name;
    MarkCreated(createdBy);
  }
}