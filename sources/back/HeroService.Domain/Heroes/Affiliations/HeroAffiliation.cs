using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Domain.Heroes.Affiliations;

/// <summary>
/// Represents the full structural identity of a Hero across gameplay and world systems.
/// 
/// Domain Role:
/// HeroAffiliation composes all identity axes that define a Hero's position in both:
/// - Combat systems (gameplay balance)
/// - World systems (lore and cultural context)
/// 
/// This includes:
/// - OriginArchetype (core gameplay classification)
/// - MythologyType (mythological framework / pantheon system)
/// - OriginCulture (geographical or civilizational origin)
/// 
/// Matchmaking Relevance:
/// - OriginArchetype → PRIMARY axis (combat + balance)
/// - MythologyType → SECONDARY axis (synergy + conditional logic)
/// - OriginCulture → TERTIARY axis (mostly narrative, optional gameplay hooks)
/// 
/// Invariants:
/// - Each Hero must have exactly one value for each axis.
/// - All referenced IDs must exist and be valid reference data.
/// - Archetype changes are snapshot-critical.
/// - Mythology/Culture are optional-impact depending on rule configuration.
/// 
/// Relationships:
/// - Part of the Hero aggregate.
/// - Consumed by:
///   • Matchmaking system
///   • Hero catalog (UI + filtering)
///   • Rule engines (conditional effects)
///   • Snapshot resolver (only Archetype is required)
/// 
/// Versioning / Snapshot Impact:
/// - OriginArchetype → HIGH impact (mandatory snapshot regeneration)
/// - MythologyType → MEDIUM impact (depends on rule usage)
/// - OriginCulture → LOW impact (usually non-snapshot affecting)
/// 
/// Developer Notes:
/// - Do NOT assume all axes have equal gameplay weight.
/// - Archetype is the ONLY guaranteed gameplay axis.
/// - Culture must remain safe for world expansion without balance impact.
/// - Keep evaluation logic outside this entity.
/// </summary>
public sealed class HeroAffiliation
{
  /// <summary>
  /// Core gameplay identity axis.
  /// Drives balance, matchmaking and rule systems.
  /// </summary>
  public OriginArchetype Archetype { get; private set; }

  /// <summary>
  /// Mythological framework affiliation.
  /// Used for synergy and conditional systems.
  /// </summary>
  public MythologyType Mythology { get; private set; }

  /// <summary>
  /// Narrative/cultural origin.
  /// Primarily used for lore and thematic systems.
  /// </summary>
  public OriginCulture OriginCulture{ get; private set; }

  private HeroAffiliation() { }

  public HeroAffiliation(
      OriginArchetype archetype,
      MythologyType mythology,
      OriginCulture originCulture)
  {
    Archetype = archetype ?? throw new ArgumentNullException(nameof(archetype));
    Mythology = mythology ?? throw new ArgumentNullException(nameof(mythology));
    OriginCulture = originCulture ?? throw new ArgumentNullException(nameof(originCulture));
  }
}
