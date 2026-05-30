namespace HeroService.Domain.Heroes.Skills;

public enum StackType
{
  /// <summary>
  /// The effect does not stack at all.
  /// Reapplying it refreshes duration or overwrites the existing instance.
  ///
  /// Used for:
  /// - standard buffs
  /// - simple damage skills
  /// - single-instance shields
  /// </summary>
  None = 0,

  /// <summary>
  /// The effect stacks by refreshing duration only.
  /// Multiple applications do not increase magnitude.
  ///
  /// Used for:
  /// - DoTs (damage over time)
  /// - HoTs (healing over time)
  /// - debuffs with consistent intensity
  /// </summary>
  RefreshDuration = 1,

  /// <summary>
  /// The effect stacks additively in magnitude.
  /// Each application increases total strength.
  ///
  /// Example:
  /// - +10% damage per stack
  /// - stacking poison intensity
  /// </summary>
  Additive = 2,

  /// <summary>
  /// The effect stacks multiplicatively.
  /// Each stack multiplies the previous effect.
  ///
  /// WARNING:
  /// High risk of exponential scaling and meta collapse if uncapped.
  /// Must be tightly controlled in SnapshotResolver.
  /// </summary>
  Multiplicative = 3,

  /// <summary>
  /// The effect replaces the previous instance entirely.
  /// No stacking occurs; last applied effect wins.
  ///
  /// Used for:
  /// - stance changes
  /// - mode switches
  /// - transformation states
  /// </summary>
  Replace = 4,

  /// <summary>
  /// The effect stacks up to a maximum threshold.
  /// Beyond MaxStacks, additional applications are ignored or refreshed.
  ///
  /// Used for:
  /// - combo systems
  /// - buildup mechanics
  /// - rage/energy systems
  /// </summary>
  Capped = 5,

  /// <summary>
  /// Each stack is independent and resolved separately.
  /// Multiple instances coexist simultaneously.
  ///
  /// Used for:
  /// - multiple traps
  /// - independent projectiles
  /// - layered zones
  /// </summary>
  Independent = 6,

  Refresh = 7
}