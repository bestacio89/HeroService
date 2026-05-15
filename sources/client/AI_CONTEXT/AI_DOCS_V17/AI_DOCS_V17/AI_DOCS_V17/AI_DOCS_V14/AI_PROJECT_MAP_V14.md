# AI_PROJECT_MAP_V14

## Main folders
- `Scripts/Managers`
  - `HeroSpawner.cs` — spawn orchestration, local player setup, NPC setup, GameManager initialization
  - `TeamManager.cs` — team registration utilities
  - `GameManager.cs` — match state, timer, flow, end-match ownership
  - `IMatchObjectiveSource.cs` — objective contract
  - `MatchObjectiveSourceBase.cs` — reusable base for future objectives
  - `DebugMatchObjective.cs` — concrete stub for validation
- `Scripts/AI`
  - `NpcHeroAI.cs` — NPC AI baseline
  - `NpcHeroRespawn.cs` — NPC death/respawn lifecycle
- `Scripts/Player`
  - `PlayerMovement.cs`
  - `PlayerTargeting.cs`
  - `PlayerRespawn.cs`
  - `PlayerBasicAttack.cs`
  - `HeroLoader.cs`
  - `HeroSelector.cs`
  - `HeroSkillSlot.cs`
- `Scripts/UI`
  - `PlayerCombatInputUI.cs`
  - `MatchHUDPresenter.cs`
  - world and player HUD scripts
- `Scripts/Heroes/Byakuya`
  - Byakuya runtime combat scripts
- `Scripts/Systems/Health`
  - health/death runtime systems
- `Scenes`
  - `Prototype_Map.unity`
- `Resources/Heroes`
  - runtime hero definitions

## Most important current flow
1. Scene loads `Prototype_Map`
2. `HeroSpawner.Start()` calls `SpawnAllHeroes()`
3. Each `HeroSpawnConfig` spawns one hero
4. `HeroRuntime` receives hero definition + team
5. Local hero gets input/UI stack
6. Non-local hero gets `NpcHeroAI`
7. Non-local hero gets `NpcHeroRespawn`
8. `TeamManager` registers heroes
9. `HeroSpawner` initializes `GameManager`
10. `GameManager` starts the match
11. `MatchHUDPresenter` reflects timer/state in UI
12. `DebugMatchObjective` or a future objective can request match end
13. `GameManager.EndMatch(...)` updates state/result
14. `MatchHUDPresenter` shows `Ended` and the player-facing result

## Important design split
### Match ownership
- `GameManager`
- match state
- timer
- result
- start/pause/resume/end
- `RequestMatchEnd(...)`

### External objective ownership
- future `WonderObjective` / `CoreObjective`
- objective-specific rule logic
- must not replace GameManager ownership of end flow

### UI ownership
- `MatchHUDPresenter`
- read-only presentation of timer/state/result
- no gameplay decision logic

### Legacy scene content
- old `Enemies/Enemy_01`
- should not be part of the active runtime anymore
- can create the historical white capsule bug if left visible

## Next files likely to change in V15
- future minimap scripts and UI assets
- potentially `HeroSpawner` only for minimap registration hooks if needed
- `GameManager` only if minimap needs read-only access to canonical hero lists
- scene UI layout for minimap integration
- possibly a new document package for minimap design decisions
