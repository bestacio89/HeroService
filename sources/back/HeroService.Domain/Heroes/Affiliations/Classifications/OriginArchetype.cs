namespace HeroService.Domain.Heroes.Affiliations.Classifications;


/// <summary>
/// Represents the fundamental existential classification of a Hero.
/// 
/// Domain Role:
/// OriginArchetype defines the intrinsic nature or "race-like essence"
/// of a Hero within the combat system (e.g., Human, Divine, Titan, Hybrid).
/// 
/// This is a core gameplay axis used across matchmaking, balance evaluation,
/// and rule-based systems to determine structural compatibility between Heroes.
/// 
/// Matchmaking Relevance:
/// - Primary classification axis in matchmaking systems.
/// - Used to:
///   • Determine baseline compatibility between Heroes.
///   • Enable archetype-based synergies (e.g., Divine + Divine bonuses).
///   • Apply constraints or penalties in team composition rules.
///   • Drive conditional logic in skills, effects, and modifiers.
/// - Evaluated before any narrative or cultural classification.
/// 
/// Invariants:
/// - Each Hero must have exactly one OriginArchetype.
/// - Archetype values are stable gameplay definitions (not cosmetic).
/// - New archetypes must be explicitly introduced and validated across:
///   • matchmaking rules
///   • snapshot evaluation systems
///   • balance tuning systems
/// 
/// Relationships:
/// - Used by HeroAffiliation as a core gameplay axis.
/// - Referenced by:
///   • Matchmaking evaluators
///   • Combat simulation systems
///   • Skill condition engines
/// 
/// Versioning / Snapshot Impact:
/// - HIGH impact attribute.
/// - Any change affects:
///   • combat power evaluation
///   • team composition logic
///   • deterministic snapshot outcomes
/// - Must trigger snapshot regeneration when modified.
/// 
/// Developer Notes:
/// - This is a deterministic gameplay classification, NOT lore metadata.
/// - Avoid embedding business logic directly in enum values.
/// - All rule evaluations should be centralized in domain services.
/// </summary>
public class OriginArchetype : Entity<Guid>
{
  public string Name { get; private set; }

  private OriginArchetype() { }

  public OriginArchetype(string name, string createdBy)
  {
    Name = name;
    MarkCreated(createdBy);
  }

  public void Define(string name, string createdBy)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("HeroClass name is required.");

    Name = name;

    MarkCreated(createdBy);
  }

  public void Rename(string name, string modifiedBy)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Name cannot be empty.");

    if (Name == name)
      return;

    Name = name;
    MarkUpdated(modifiedBy);
  }
}

