# AI_CONTEXT_V14_STUDIO

## Goal of this context file
Help a future AI continue the project from the current real state instead of re-debugging already solved issues.

## What changed recently
1. V10 stabilized local combat runtime.
2. V11 introduced `NpcHeroAI` and extended `HeroSpawner` so the 9 non-local heroes behave as a simple 5v5 prototype.
3. V12 introduced `NpcHeroRespawn` and validated NPC death/respawn in Play Mode.
4. V13 introduced and consolidated `GameManager`.
5. V13 added a clean objective extension path:
   - `RequestMatchEnd(...)`
   - `IMatchObjectiveSource`
   - `MatchObjectiveSourceBase`
   - `DebugMatchObjective`
6. V13 added a first UI bridge through `MatchHUDPresenter`.
7. V13.1 validated the readable HUD baseline and hidden result state.
8. V13.2 validated the end flow through `OnMatchEnded` with visible `Ended` and `Victory` in runtime.
9. V14 finalized `1.5.D` stabilization:
   - redundant logs cleaned
   - repeated Play Mode validation passed
   - no regression on spawn / AI / respawn / HUD / end flow
   - `Time.timeScale` behavior confirmed coherent
   - legacy `Enemy_01` white capsule issue re-confirmed and cleanup path documented

## Practical debugging checklist

### If GameManager does not initialize
- confirm a `GameManager` GameObject exists in the scene
- confirm `HeroSpawner` references or finds `GameManager`
- confirm `HeroSpawner` calls `InitializeMatch(_spawnedHeroes, _localHeroInstance)` after spawn
- confirm runtime actually reaches `StartMatch()`

### If RequestMatchEnd does nothing
- confirm `GameManager.Instance` exists
- confirm `MatchResult` is not `None`
- confirm `currentState != Ended`
- confirm the source objective calls `TriggerObjectiveResolved()`
- confirm `DebugMatchObjective` has a valid `VictoryResult` (not `None`) when used for tests

### If future objectives fail
- confirm the source implements `IMatchObjectiveSource`
- confirm it inherits from `MatchObjectiveSourceBase` when appropriate
- confirm it passes a valid `VictoryResult`
- confirm the goal logic stays outside `GameManager`

### If HUD does not update
- confirm `MatchHUDPresenter` is attached
- confirm references to `GameManager`, `MatchTimerText`, `MatchStateText`, `MatchResultText`
- confirm the Canvas is active
- confirm `MatchResultText` is hidden while `CurrentResult == None`
- confirm `OnMatchEnded` reaches the presenter

### If the result text is wrong
- remember `GameManager.LocalHero` is a `GameObject`
- `MatchHUDPresenter` must fetch `HeroRuntime` from that object
- team comparison uses `TeamId.TeamA` / `TeamId.TeamB`, not a generic `Team` enum

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
- confirm `Enemy_01` or similar old scene enemies are removed or inactive
- if the white block disappears when `MeshRenderer` is disabled, the cause is legacy scene content, not an 11th hero
- do not misdiagnose this as a `HeroSpawner` bug without checking the hierarchy first

### If repeated Play tests diverge
- verify no duplicate `GameManager` exists
- verify `Time.timeScale` returns to a sane state after exiting play mode and after match end
- confirm no old scene object survives as a hidden dependency

## Editor reminders
- `SpawnPoints` are children under `HeroSpawner`
- moving `HeroSpawner` moves every spawn point
- all hero roots should remain on layer `Hero`
- local player still uses tag `Player`
- treat legacy helper objects with suspicion before assuming a runtime-system regression
