using System;

namespace HeroService.Domain.Heroes.Skills;

/// <summary>
/// Represents the narrative and descriptive identity of a Skill.
///
/// Domain Role:
/// Encapsulates all non-mechanical, player-facing information about a Skill,
/// including its name, description, and visual representation guidance.
///
/// This entity exists purely to support UI presentation, tooltips,
/// animation interpretation layers, and player immersion.
///
/// It has NO influence on gameplay simulation or combat resolution.
///
/// Matchmaking Relevance:
/// - No direct impact on matchmaking, balance, or combat simulation.
/// - May be used indirectly for:
///   • UI clarity during hero/skill selection.
///   • Training/tutorial systems.
///   • Visual effect mapping in client rendering systems.
/// - Must NEVER be used in rule engines or simulation logic.
///
/// Invariants:
/// - SkillId must reference a valid Skill aggregate.
/// - Name must be non-empty and represent a readable skill identity.
/// - Description must explain player-facing behavior in natural language.
/// - VisualExplanation is optional and used for rendering/animation guidance only.
/// - This entity is strictly 1-to-1 with a Skill.
///
/// Relationships:
/// - Directly associated with a single Skill (SkillId).
/// - Consumed by:
///   • UI systems (tooltips, skill panels)
///   • Client rendering / VFX interpretation layers
///   • Localization systems
/// - Explicitly excluded from:
///   • SnapshotResolver
///   • Combat simulation
///   • Matchmaking evaluation
///   • Balance systems
///
/// Versioning / Snapshot Impact:
/// - No impact on gameplay snapshots.
/// - Can evolve independently without triggering combat rebalancing.
/// - Changes are purely cosmetic or UX-driven.
///
/// Developer Notes:
/// - This is a presentation-only entity.
/// - Do NOT introduce gameplay-relevant fields here.
/// - Do NOT derive effects, scaling, or mechanics from this data.
/// - Any gameplay-relevant concept must be modeled in SkillBaseStats or SkillEffect instead.
/// - Keep this entity stable for UI compatibility and localization consistency.
///
/// Architectural Insight:
/// - SkillLore defines "how the skill is perceived."
/// - SkillEffect defines "what the skill does."
/// - SkillBaseStats defines "how strong the skill is."
/// - This separation ensures that narrative and mechanics remain fully decoupled,
///   preserving deterministic simulation integrity.
/// </summary>
public sealed class SkillLore : Entity<Guid>
{
  public Guid SkillId { get; private set; }

  public string Name { get; private set; }
  public string Description { get; private set; }
  public string VisualExplanation { get; private set; }

  private SkillLore() { }

  public SkillLore(Guid skillId, string name, string description, string visualExplanation)
  {
    SkillId = skillId;
    Name = name;
    Description = description;
    VisualExplanation = visualExplanation;

    MarkCreated("system");
  }
}