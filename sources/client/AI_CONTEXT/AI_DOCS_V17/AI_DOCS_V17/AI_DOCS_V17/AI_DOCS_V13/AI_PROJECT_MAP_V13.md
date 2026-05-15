# AI_PROJECT_MAP_V13

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
11. `MatchHUDPresenter` reflects match state in UI

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

## Next files likely to change in V14
- `Scripts/UI/MatchHUDPresenter.cs` (UI refinement)
- future end screen UI scripts
- future objective scripts:
  - `WonderObjective.cs`
  - or `CoreObjective.cs`
- potentially scene UI layout and minimap setup
