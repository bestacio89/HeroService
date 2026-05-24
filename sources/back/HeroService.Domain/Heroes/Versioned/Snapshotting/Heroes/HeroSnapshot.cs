using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable
namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;

/// <summary>
/// Represents a fully resolved, immutable snapshot of a Hero at a specific GameVersion.
///
/// Domain Role:
/// HeroSnapshot is the **final deterministic representation of a Hero’s combat state**
/// after applying:
/// - HeroBaseStats
/// - HeroModifier (GameVersion scaling layer)
/// - SkillBaseStats
/// - SkillEffects (resolved via SnapshotResolver)
///
/// This entity is the **core input for all combat simulation systems**.
///
/// Matchmaking Relevance:
/// - Primary unit used for matchmaking evaluation.
/// - Used to compute:
///   • Effective survivability (HP, shields, mitigation)
///   • Damage output potential (AD/AP + skill integration)
///   • Resource economy efficiency (mana + cooldown + regen)
///   • Skill kit power distribution
/// - Enables deterministic comparison between Heroes for balance and fairness.
///
/// Invariants:
/// - HeroId must reference a valid Hero.
/// - GameVersionId must reference the active simulation version.
/// - Snapshot is immutable once created.
/// - Skills must already be fully resolved SkillSnapshots (no lazy computation).
///
/// Relationships:
/// - Composed of:
///   • SkillSnapshot (resolved ability state)
/// - Derived from:
///   • HeroBaseStats
///   • HeroModifier
///   • SkillBaseStats
///   • SkillEffects
/// - Consumed by:
///   • Combat simulation engine
///   • Matchmaking evaluation system
///   • Replay / deterministic validation systems
///
/// Versioning / Snapshot Impact:
/// - CRITICAL system artifact.
/// - Fully version-bound (GameVersionId defines interpretation context).
/// - Any change in underlying systems requires regeneration of snapshots.
/// - Guarantees deterministic replayability of matches.
///
/// Developer Notes:
/// - This class must remain immutable.
/// - DO NOT add business logic that mutates state.
/// - Only allow aggregation/helper queries that do NOT alter meaning.
/// - Avoid reducing stats too early — keep as close as possible to full simulation model.
/// - This is NOT a DTO; it is a simulation artifact.
///
/// Architectural Insight:
/// - HeroSnapshot is the “truth state” of a Hero inside a match.
/// - SkillSnapshot is the “truth state” of abilities.
/// - SnapshotResolver is the system that produces this truth.
/// </summary>
public sealed class HeroSnapshot
{
  public Guid HeroId { get; }
  public Guid GameVersionId { get; }

  public HeroStatSnapshot Stats { get; }
  public HeroSkillKitSnapshot SkillKit { get; }

  public HeroSnapshot(
      Guid heroId,
      Guid gameVersionId,
      HeroStatSnapshot stats,
      HeroSkillKitSnapshot skillKit)
  {
    HeroId = heroId;
    GameVersionId = gameVersionId;
    Stats = stats;
    SkillKit = skillKit;
  }
}