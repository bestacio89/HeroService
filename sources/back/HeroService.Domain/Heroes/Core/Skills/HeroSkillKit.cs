namespace HeroService.Domain.Heroes.Core.Skills;
/// <summary>
/// Represents the complete set of skills defining a Hero's combat capabilities.
///
/// Domain Role:
/// Encapsulates the fixed composition of a Hero's abilities as a structured kit,
/// defining how the Hero behaves in combat scenarios. Each slot represents a
/// distinct tactical role within the kit (passive, active abilities, and ultimate).
///
/// This is not a collection but a **contractual loadout**—every Hero is expected
/// to have a fully defined and valid skill kit.
///
/// Matchmaking Relevance:
/// - Acts as a primary driver of gameplay behavior in matchmaking simulations.
/// - Used to:
///   • Evaluate team composition (damage, support, control, burst potential).
///   • Detect synergies and combos between Heroes.
///   • Identify gaps or redundancies in a team (e.g., no ultimate impact, no sustain).
/// - Each slot has implicit weight:
///   • Passive → constant influence / baseline modifiers
///   • Primary/Secondary/Tertiary → core rotation and tactical flexibility
///   • Ultimate → high-impact, often decisive ability
///
/// Invariants:
/// - All skill IDs must be valid and reference existing Skill definitions.
/// - No slot can be empty (no partial kits).
/// - Each slot must represent a distinct role; duplicate skill usage across slots
///   is considered invalid unless explicitly supported by design.
/// - The structure (5 slots) is fixed and must remain consistent across all Heroes.
///
/// Relationships:
/// - Part of the Hero aggregate (core combat definition).
/// - Each SkillId references a Skill entity from the Skills domain.
/// - Consumed by:
///   • Matchmaking evaluators (team balance and synergy)
///   • Combat simulation systems
///   • Rule engines evaluating skill interactions
///
/// Versioning / Snapshot Impact:
/// - Considered a high-impact component.
/// - Any change to any SkillId must trigger a new version/snapshot,
///   as it directly alters the Hero's behavior and matchmaking evaluation.
/// - Frequently included in snapshot comparison and caching strategies.
///
/// Developer Notes:
/// - This is intentionally modeled as a fixed structure, not a collection,
///   to enforce consistency and simplify evaluation logic.
/// - Avoid introducing optional slots; matchmaking logic depends on predictability.
/// - Do NOT embed behavior here—this is a structural definition only.
/// - Ensure referential integrity with Skill entities at creation time (fail fast).
/// </summary>
public class HeroSkillKit
{
  public Guid PassiveSkillId { get; }

  public Guid PrimarySkillId { get; }
  public Guid SecondarySkillId { get; }
  public Guid TertiarySkillId { get; }
  public Guid UltimateSkillId { get; }

  private HeroSkillKit() { }
  public HeroSkillKit(
    Guid passiveSkillId,
    Guid primarySkillId,
    Guid secondarySkillId,
    Guid tertiarySkillId,
    Guid ultimateSkillId)
  {
    PassiveSkillId = passiveSkillId;
    PrimarySkillId = primarySkillId;
    SecondarySkillId = secondarySkillId;
    TertiarySkillId = tertiarySkillId;
    UltimateSkillId = ultimateSkillId;
  }
}