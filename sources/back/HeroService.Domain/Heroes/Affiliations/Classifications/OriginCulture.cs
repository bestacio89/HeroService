namespace HeroService.Domain.Heroes.Affiliations.Classifications;

/// <summary>
/// Represents the cultural or geographical origin of a Hero within the world.
/// 
/// Domain Role:
/// OriginCulture defines the narrative and world-building origin of a Hero,
/// such as a city, civilization, or mythological location (e.g., Sparta, Athens, Asgard).
/// 
/// This entity belongs to the lore and identity layer of the system and
/// does NOT inherently affect combat mechanics unless explicitly mapped.
/// 
/// Matchmaking Relevance:
/// - Secondary or optional classification axis.
/// - Used for:
///   • Narrative grouping of Heroes
///   • Thematic or faction-based systems (if enabled)
///   • Event-driven or seasonal content mechanics
/// - Must NOT be used as a primary balance axis unless explicitly designed.
/// 
/// Invariants:
/// - Each Hero may have exactly one OriginCulture.
/// - Culture entries are extensible and content-driven.
/// - Names must be unique within the system to avoid ambiguity.
/// 
/// Relationships:
/// - Used by HeroAffiliation as a narrative identity axis.
/// - Referenced by:
///   • Hero selection UI
///   • Lore systems
///   • Event and storytelling systems
/// 
/// Versioning / Snapshot Impact:
/// - LOW impact by default.
/// - Does not affect combat snapshots unless explicitly integrated into rules.
/// - Can evolve without triggering balance recalculation.
/// 
/// Developer Notes:
/// - This is a world-building construct, not a gameplay mechanic.
/// - Keep it isolated from snapshot and balance logic unless explicitly extended.
/// - Treat as reference data supporting narrative systems.
/// </summary>
public class OriginCulture : Entity<Guid>
{
  public string Name { get; private set; }

  private OriginCulture() { }

  public OriginCulture(string name, string createdBy)
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
