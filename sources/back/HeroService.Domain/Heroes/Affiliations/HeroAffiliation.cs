using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Domain.Heroes.Affiliations;

/// <summary>
/// Represents the intrinsic origin and mythological classification of a Hero.
///
/// Domain Role:
/// Encapsulates the fundamental background of a Hero by defining:
/// - The high-level origin category (e.g., divine, human, demonic, artificial).
/// - The specific mythology or cultural system the Hero belongs to.
/// This is not cosmetic metadata; it defines the narrative and systemic identity of the Hero.
///
/// Matchmaking Relevance:
/// - Acts as both a constraint and a synergy signal in matchmaking.
/// - Used to:
///   • Enable or restrict team compositions based on origin compatibility.
///   • Apply bonuses/penalties for shared or conflicting mythological backgrounds.
///   • Drive faction-like logic without being a strict "faction" system.
/// - Can be leveraged by skills, effects, or rules that target specific origins or mythologies.
///
/// Invariants:
/// - OriginType must always be a valid, defined classification.
/// - MythologyTypeId must reference an existing, valid mythology definition.
/// - A Hero must have exactly one primary HeroAffiliation at any given time.
/// - The combination of OriginType and MythologyTypeId must be semantically consistent
///   (e.g., no invalid pairings like "Artificial" with a purely divine mythology unless explicitly supported).
///
/// Relationships:
/// - Part of the Hero aggregate.
/// - MythologyTypeId links to an external Mythology definition (reference data / lookup aggregate).
/// - OriginType is a controlled enum that drives rule-based logic across the system.
/// - Frequently referenced by:
///   • Skills (conditional effects)
///   • Affiliation-based rules
///   • Matchmaking evaluators
///
/// Versioning / Snapshot Impact:
/// - Considered a high-impact attribute for matchmaking.
/// - Any change in OriginType or MythologyTypeId must trigger a new version/snapshot,
///   as it can significantly alter compatibility, synergies, and constraints.
/// - Must be included in snapshot comparisons and matchmaking cache keys.
///
/// Developer Notes:
/// - Do NOT treat this as descriptive lore only—this is a core gameplay/mechanics driver.
/// - Avoid hardcoding logic against specific MythologyTypeId values; rely on classification rules instead.
/// - Ensure new mythology entries are properly registered and compatible with existing matchmaking logic.
/// - This entity is intentionally minimal; behavior should live in domain services or rule evaluators.
/// </summary>
/// 


public class HeroAffiliation : Entity<Guid>
{
  public OriginType OriginType { get; private set; }

  public Guid MythologyTypeId { get; private set; }

  private HeroAffiliation() { }

  public HeroAffiliation(OriginType originType, Guid mythologyTypeId)
  {
    OriginType = originType;
    MythologyTypeId = mythologyTypeId;

    MarkCreated("system");
  }
}