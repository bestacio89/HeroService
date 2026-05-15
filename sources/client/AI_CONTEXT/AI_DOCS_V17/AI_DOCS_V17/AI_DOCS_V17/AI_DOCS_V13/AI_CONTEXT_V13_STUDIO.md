# AI_CONTEXT_V13_STUDIO

## Goal of this context file
Help a future AI continue the project from the current real state instead of re-debugging already solved issues.

## What changed recently
1. V10 stabilized local combat runtime.
2. V11 introduced `NpcHeroAI` and extended `HeroSpawner` so the 9 non-local heroes now behave as a simple 5v5 prototype.
3. V12 introduced `NpcHeroRespawn` and validated NPC death/respawn in Play Mode.
4. V13 introduced and consolidated `GameManager`.
5. V13 added a clean objective extension path:
   - `RequestMatchEnd(...)`
   - `IMatchObjectiveSource`
   - `MatchObjectiveSourceBase`
   - `DebugMatchObjective`
6. V13 added a first UI bridge through `MatchHUDPresenter`.
7. The white capsule bug remains historically linked to the old scene object `Enemies/Enemy_01`, not to the 10-hero runtime.

## Practical debugging checklist

### If GameManager does not initialize
- confirm a `GameManager` GameObject exists in the scene
- confirm `HeroSpawner` references or finds `GameManager`
- confirm `HeroSpawner` calls `InitializeMatch(_spawnedHeroes, _localHeroInstance)` after spawn
- confirm logs show:
  - `Match initialisé`
  - `Match démarré`

### If RequestMatchEnd does nothing
- confirm `GameManager.Instance` exists
- confirm `MatchResult` is not `None`
- confirm `currentState != Ended`
- confirm the source objective calls `TriggerObjectiveResolved()`

### If future objectives fail
- confirm the source implements `IMatchObjectiveSource`
- confirm it inherits from `MatchObjectiveSourceBase` when appropriate
- confirm it passes a valid `VictoryResult`

### If HUD does not update
- confirm `MatchHUDPresenter` is attached
- confirm references to `GameManager`, `MatchTimerText`, `MatchStateText`, `MatchResultText`
- confirm the Canvas is active
- remove temporary debug logs after validation

### If PlayerTargeting becomes noisy again
- confirm `enableDebugLogs` is false on the prefab, not just in code
- remember Unity preserves serialized prefab values
- if needed, edit the prefab field directly

### If NPC death/respawn breaks
- confirm `NpcHeroRespawn` is present on non-local heroes
- confirm `SetRespawnPoint(...)` is still called from `HeroSpawner`
- confirm `HealthSystem.OnDied` fires
- confirm HP is restored with `ReviveFull()`
- confirm renderers/canvases are hidden instead of disabling whole objects

### If a white capsule follows the player again
- inspect the hierarchy for legacy objects under `Enemies`
- confirm `Enemy_01` or similar old scene enemies are inactive or removed
- do not misdiagnose this as an 11th hero without checking the hierarchy first
