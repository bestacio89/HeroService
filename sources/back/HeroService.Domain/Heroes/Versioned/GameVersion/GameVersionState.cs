using System;

namespace HeroService.Domain.Heroes.Versioned.GameVersion;

/// <summary>
/// Represents the runtime context holder for the currently active GameVersion.
///
/// Domain Role:
/// GameVersionState is NOT a domain entity in the strict sense.
/// It is a **runtime configuration holder** used to define which GameVersion
/// is currently active for snapshot resolution and simulation execution.
///
/// It acts as a global reference point for systems that require knowledge of
/// the active balance state (GameVersion) at runtime.
///
/// Matchmaking Relevance:
/// - Indirect but critical role in ensuring consistency of simulation context.
/// - Used by:
///   • SnapshotResolver to determine which HeroModifier set to apply
///   • Matchmaking systems to ensure all evaluations use the same version context
/// - Guarantees that all combat simulations are executed under a single
///   consistent balance version.
///
/// Invariants:
/// - ActiveGameVersionId must always reference a valid GameVersion.
/// - There must be exactly one active GameVersion at runtime per simulation context.
/// - State changes must be controlled and deterministic (no random or implicit updates).
///
/// Relationships:
/// - References GameVersion (logical balance container)
/// - Consumed by:
///   • SnapshotResolver (core dependency for resolution context)
///   • Matchmaking evaluation pipeline
///   • Simulation orchestration layer
///
/// Versioning / Snapshot Impact:
/// - CRITICAL runtime dependency for all snapshot computations.
/// - Changing ActiveGameVersionId affects:
///   • All subsequent combat simulations
///   • Matchmaking fairness evaluation
///   • Meta interpretation of all Hero and Skill data
/// - Must never change mid-simulation.
///
/// Developer Notes:
/// - This class MUST be treated as a scoped runtime context, not persistent domain state.
/// - In multi-match systems, this should ideally be scoped per match instance,
///   not globally shared across all sessions.
/// - Avoid introducing business logic here.
/// - Do NOT use this as a feature flag system or configuration store.
/// - In distributed systems, this should be injected per request or per match context.
///
/// Architectural Insight:
/// - GameVersion is the "definition of balance reality."
/// - GameVersionState is the "currently selected reality window."
/// - SnapshotResolver uses both to compute deterministic outcomes.
///
/// This class acts as the bridge between versioned balance data and runtime execution.
/// </summary>
public class GameVersionState
{
  public Guid ActiveGameVersionId { get; private set; }

  public void SetActive(Guid gameVersionId)
  {
    ActiveGameVersionId = gameVersionId;
  }
}