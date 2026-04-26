namespace HeroService.Domain.Heroes.Affiliations.Classifications;

/// <summary>
/// Represents a mythology or cultural system to which a Hero belongs.
///
/// Domain Role:
/// Defines the cultural, narrative, and systemic grouping of a Hero (e.g., Greek, Norse, Japanese).
/// Unlike OriginType (which is a closed classification), MythologyType is an extensible
/// reference entity that allows the system to introduce new mythological domains without
/// modifying core enums or breaking existing logic.
///
/// Matchmaking Relevance:
/// - Acts as a secondary but highly influential classification axis.
/// - Used to:
///   • Enable intra-mythology synergies (e.g., Greek + Greek).
///   • Introduce cross-mythology interactions (alliances, rivalries, counters).
///   • Refine matchmaking decisions beyond coarse OriginType grouping.
/// - Often combined with OriginType to form a composite compatibility profile.
///
/// Invariants:
/// - Name must be non-empty and uniquely identify a mythology within the system.
/// - MythologyType instances are expected to be stable once introduced (no frequent mutation).
/// - Must be registered and available before being referenced by HeroAffiliation.
///
/// Relationships:
/// - Referenced by HeroAffiliation via MythologyTypeId.
/// - Acts as reference data (lookup entity) rather than a behavioral aggregate.
/// - Can be used by:
///   • Matchmaking evaluators
///   • Skill conditions (e.g., "applies only to Norse mythology")
///   • Rule engines defining cross-mythology interactions
///
/// Versioning / Snapshot Impact:
/// - Changes to the MythologyTypeId of a Hero (not the entity itself) are high-impact
///   and must trigger a new version/snapshot.
/// - The MythologyType entity itself is typically static and not versioned frequently,
///   but its identity is critical in snapshot comparisons.
///
/// Developer Notes:
/// - This is an extensibility point: prefer adding new MythologyType entries over expanding enums.
/// - Do NOT hardcode logic against Name values; always rely on identifiers and centralized rules.
/// - Ensure uniqueness of Name at the persistence level to avoid ambiguous classifications.
/// - Treat this as reference data: lightweight, stable, and widely reused across the domain.
/// </summary>
public class MythologyType : Entity<Guid>
{
  public string Name { get; private set; } = string.Empty;

  private MythologyType() { }

  public MythologyType(string name, string createdBy)
  {
    Name = name;
    MarkCreated(createdBy);
  }
}