

using HeroService.Domain.Heroes.Core.Skills;

namespace HeroService.Domain.Heroes.Core
{
  /// <summary>
  /// Represents a Hero as the central aggregate root of the domain.
  ///
  /// Domain Role:
  /// The Hero is the primary aggregate that encapsulates identity, classification,
  /// affiliation, and combat capabilities. It serves as the authoritative source
  /// of truth for anything related to a playable or matchable unit in the system.
  ///
  /// This aggregate composes:
  /// - Classification (HeroClass)
  /// - Cultural/ontological identity (HeroAffiliation)
  /// - Combat behavior (HeroSkillKit)
  ///
  /// Matchmaking Relevance:
  /// - Acts as the fundamental unit of matchmaking.
  /// - Used to:
  ///   • Build team compositions.
  ///   • Evaluate role distribution via HeroClass.
  ///   • Evaluate compatibility and constraints via HeroAffiliation.
  ///   • Simulate combat potential via SkillKit.
  /// - A Hero’s matchmaking profile is effectively the combination of:
  ///   (HeroClass + HeroAffiliation + SkillKit).
  ///
  /// Invariants:
  /// - Name must be non-empty and uniquely identifiable within the system context.
  /// - HeroClassId must reference a valid HeroClass.
  /// - HeroAffiliationId must reference a valid HeroAffiliation.
  /// - SkillKit must be fully defined and valid (no missing or invalid skill references).
  /// - A Hero must always have exactly one class, one affiliation, and one skill kit.
  /// - Aggregate consistency must be preserved; partial updates are not allowed.
  ///
  /// Relationships:
  /// - Aggregate root for the Heroes domain.
  /// - References:
  ///   • HeroClass (role/archetype definition)
  ///   • HeroAffiliation (origin + mythology classification)
  ///   • HeroSkillKit (combat definition)
  /// - Indirectly connected to:
  ///   • Skills (via SkillKit)
  ///   • Mythology and Origin classifications (via HeroAffiliation)
  ///
  /// Versioning / Snapshot Impact:
  /// - Any change to HeroClassId, HeroAffiliationId, or SkillKit is considered high impact
  ///   and must trigger a new version/snapshot.
  /// - The Hero aggregate is a primary input for snapshot generation and matchmaking caching.
  /// - Even minor changes can alter matchmaking outcomes and must be tracked explicitly.
  ///
  /// Controlled Mutation:
  /// - Affiliation changes are explicitly controlled via SetAffiliation.
  /// - Such changes are expected to be rare and must be treated as significant domain events,
  ///   as they directly affect matchmaking compatibility and rule evaluation.
  ///
  /// Developer Notes:
  /// - This is the entry point for all matchmaking logic—do not bypass the aggregate.
  /// - Avoid exposing setters that allow uncontrolled mutation of core attributes.
  /// - Navigation properties (e.g., HeroClass) should not be relied on for domain logic;
  ///   use identifiers and domain services for evaluation.
  /// - Ensure referential integrity is validated at creation time (fail fast).
  /// - Keep behavior out of the entity unless it enforces invariants; orchestration belongs
  ///   in domain services or application layers.
  ///
  /// Architectural Insight:
  /// - The Hero aggregate is intentionally compositional:
  ///   • HeroClass → "What role does it play?"
  ///   • HeroAffiliation → "Where does it come from?"
  ///   • HeroSkillKit → "How does it behave?"
  /// - This separation enables deterministic, explainable matchmaking without ambiguity.
  /// </summary>
  public class Hero : Entity<Guid>
  {
    public string Name { get; private set; }

    public Guid HeroClassId { get; private set; }
    public HeroClass HeroClass { get; private set; } = null!;

    public Guid HeroAffiliationId { get; private set; }

    public HeroSkillKit SkillKit { get; private set; }

    private Hero() { } // EF

 

    // optional controlled mutation
    public void SetAffiliation(Guid affiliationId)
    {
      HeroAffiliationId = affiliationId;
    }

    public void Initialize(string name, Guid heroClassId, Guid affiliationId, string createdBy)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Hero name is mandatory.");

      if (heroClassId == Guid.Empty)
        throw new ArgumentException("A valid Hero Class must be assigned.");

      Name = name;
      HeroClassId = heroClassId;
      HeroAffiliationId = affiliationId;

      MarkCreated(createdBy);
    }
  }
 }