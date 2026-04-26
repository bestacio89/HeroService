namespace HeroService.Domain.Heroes.Affiliations.Classifications;

/// <summary>
/// Defines the fundamental existential classification of a Hero.
///
/// Domain Role:
/// Represents the primary nature of a Hero's existence (e.g., mortal, god-like, primordial).
/// This classification abstracts away individual lore into a standardized axis used
/// across the system for rule evaluation and compatibility logic.
///
/// Matchmaking Relevance:
/// - Acts as a primary classification axis in matchmaking.
/// - Used to:
///   • Determine baseline compatibility between Heroes.
///   • Enable origin-based synergies (e.g., Divine + Divine).
///   • Apply penalties or constraints (e.g., Human vs Titan imbalance scenarios).
///   • Drive rule evaluation in skills and effects targeting specific origins.
/// - Often evaluated before more granular attributes (e.g., mythology).
///
/// Invariants:
/// - Must always be one of the predefined values (closed set).
/// - Each HeroAffiliation must have exactly one OriginType.
/// - New values must be explicitly introduced and supported across matchmaking logic,
///   rule engines, and balancing systems before use.
///
/// Relationships:
/// - Used by HeroAffiliation as a core classification component.
/// - Referenced by:
///   • Matchmaking evaluators
///   • Skill conditions and effects
///   • Affiliation-based rules and constraints
///
/// Versioning / Snapshot Impact:
/// - Considered a high-impact attribute.
/// - Any change to a Hero's OriginType must trigger a new version/snapshot,
///   as it can significantly affect matchmaking outcomes and rule evaluations.
///
/// Developer Notes:
/// - This enum is NOT extensible by default; adding new values requires:
///   • Updating matchmaking rules
///   • Validating compatibility matrices
///   • Reviewing balance implications across the system
/// - Avoid embedding business logic directly against enum values in multiple places;
///   prefer centralized evaluators or rule engines.
/// - Treat this as a stable classification axis similar to a "type system" for Heroes.
/// </summary>
public enum OriginType
{
  /// <summary>
  /// Represents mortal beings with no inherent divine or primordial nature.
  /// Typically balanced, adaptable, and often used as baseline references in matchmaking.
  /// </summary>
  Human = 0,

  /// <summary>
  /// Represents god-like or celestial beings with inherent supernatural authority or power.
  /// Often associated with strong synergies and high-impact abilities.
  /// </summary>
  Divine = 1,

  /// <summary>
  /// Represents primordial or ancient entities, often predating traditional divine systems.
  /// Typically associated with raw power, instability, or unique rule interactions.
  /// </summary>
  Titan = 2,

  /// <summary>
  /// Represents entities composed of multiple origins (e.g., Human-Divine, Divine-Titan).
  /// Introduces flexible but complex interactions in matchmaking and rule evaluation.
  /// </summary>
  Hybrid = 3
}