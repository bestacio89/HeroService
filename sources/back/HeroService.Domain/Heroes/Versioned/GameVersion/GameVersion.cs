using System;

namespace HeroService.Domain.Heroes.Versioned.GameVersion;

/// <summary>
/// Represents a logical version of the game used for deterministic simulation and balance control.
///
/// Domain Role:
/// GameVersion defines a **snapshot boundary of the entire gameplay balance state**.
/// It acts as the top-level container for all versioned modifiers, ensuring that
/// Hero stats, Skill behavior, and system balance can evolve over time without
/// breaking historical consistency or replay determinism.
///
/// Each GameVersion represents a complete "balance reality" in which:
/// - HeroBaseStats are interpreted
/// - HeroModifiers are applied
/// - Skill balance assumptions are evaluated
/// - SnapshotResolver produces deterministic combat outcomes
///
/// Matchmaking Relevance:
/// - Directly determines the balance context used in all matchmaking simulations.
/// - Ensures fairness by guaranteeing that all players in a match operate under
///   the same versioned balance rules.
/// - Enables:
///   • Seasonal balance shifts
///   • Meta resets
///   • Controlled hero tuning over time
/// - Prevents cross-version imbalance contamination in matchmaking.
///
/// Invariants:
/// - Only one GameVersion can be marked as IsActive at a time.
/// - VersionName must uniquely identify a balance state (e.g., "1.0.0", "Season2026_S1").
/// - GameVersion is immutable once published; changes require a new version.
/// - All HeroModifier entities must reference a valid GameVersion.
///
/// Relationships:
/// - Parent container for:
///   • HeroModifier (balance overrides per hero)
///   • Future system modifiers (items, skills, global balance rules)
/// - Consumed by:
///   • SnapshotResolver (selects active balance context)
///   • Matchmaking system (ensures consistent simulation rules)
///   • Balance tooling / analytics pipelines
///
/// Versioning / Snapshot Impact:
/// - CRITICAL system boundary entity.
/// - Changing or activating a GameVersion effectively:
///   • Re-defines the entire meta state of the game
///   • Alters all simulation outputs
///   • Resets balance assumptions for matchmaking
/// - Must be treated as a controlled deployment artifact, not a runtime flag.
///
/// Developer Notes:
/// - IsActive should NOT be toggled casually in production environments.
/// - GameVersion should be treated as an immutable release artifact once published.
/// - All balancing logic must be attached via external modifier systems, not embedded here.
/// - This entity is intentionally minimal to enforce strict version boundary control.
///
/// Architectural Insight:
/// - GameVersion is the "time axis" of your entire combat simulation system.
/// - HeroBaseStats defines identity.
/// - HeroModifier defines version distortion.
/// - GameVersion defines which distortion set is active.
/// - SnapshotResolver defines the resulting truth.
///
/// Together, they create a fully deterministic, version-controlled combat simulation system.
/// </summary>
public class GameVersion : Entity<Guid>
{
  public string VersionName { get; private set; }
  public bool IsActive { get; private set; }

  private GameVersion() { }

  public GameVersion(string versionName, string createdBy)
  {
    VersionName = versionName;
    MarkCreated(createdBy);
  }
}