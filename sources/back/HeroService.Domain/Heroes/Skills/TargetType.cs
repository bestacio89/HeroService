namespace HeroService.Domain.Heroes.Skills;

public enum TargetType
{
  /// <summary>
  /// No explicit target is selected.
  /// The effect is resolved based on positional or self-based logic.
  /// Example: self-buffs, aura effects, ground AoE triggers.
  /// </summary>
  Self = 0,

  /// <summary>
  /// The effect targets a single allied unit.
  /// Used for heals, shields, buffs.
  /// </summary>
  Ally = 1,

  /// <summary>
  /// The effect targets a single enemy unit.
  /// Used for single-target damage, debuffs, executes.
  /// </summary>
  Enemy = 2,

  /// <summary>
  /// The effect targets multiple allied units in an area.
  /// Used for team buffs, AoE heals.
  /// </summary>
  AreaAllies = 3,

  /// <summary>
  /// The effect targets multiple enemy units in an area.
  /// Used for AoE damage, AoE crowd control.
  /// </summary>
  AreaEnemies = 4,

  /// <summary>
  /// The effect applies to all units within a radius regardless of allegiance.
  /// Used for global AoE fields, neutral zones, environmental effects.
  /// </summary>
  AreaAll = 5,

  /// <summary>
  /// The effect is bound to terrain or world position rather than a unit.
  /// Example: traps, zones, persistent ground effects.
  /// </summary>
  Ground = 6,

  /// <summary>
  /// The effect propagates through a chain of targets.
  /// Example: chain lightning, chained heals.
  /// </summary>
  Chain = 7,

  /// <summary>
  /// The effect is global and affects all entities in the match.
  /// Example: global buffs, map-wide ultimates.
  /// </summary>
  Global = 8
}