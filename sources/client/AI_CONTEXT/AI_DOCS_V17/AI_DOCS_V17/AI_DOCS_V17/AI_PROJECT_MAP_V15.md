# AI_PROJECT_MAP_V15

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
- `Scripts/UI/Minimap`
  - `MinimapBounds.cs` — world bounds used for minimap projection
  - `MinimapMarkerView.cs` — one marker view, position/visibility/rotation
  - `MinimapPresenter.cs` — marker creation/update from GameManager data
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
12. `MinimapPresenter` reads canonical hero data and builds minimap markers
13. `DebugMatchObjective` or a future objective can request match end
14. `GameManager.EndMatch(...)` updates state/result
15. `MatchHUDPresenter` shows `Ended` and the player-facing result

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

### Minimap ownership
- `MinimapBounds`
- `MinimapPresenter`
- `MinimapMarkerView`
- read-only projection and display logic
- no gameplay authority

### Legacy scene content
- old `Enemies/Enemy_01`
- should not be part of the active runtime anymore
- can create the historical white capsule bug if left visible

## Scene-side minimap expectations in V15
Expected scene objects:
- `Canvas/MinimapPanel`
- `Canvas/MinimapPanel/MarkersRoot`
- `MinimapSystem`

Expected minimap prefabs:
- `MinimapMarker_Local`
- `MinimapMarker_Ally`
- `MinimapMarker_Enemy`

Expected validated minimap behavior:
- top-right placement
- background visible
- local player triangle rotates correctly
- ally/enemy markers remain readable
- markers are clamped inside the panel

## Next files likely to change after V15
Short-term likely changes:
- hero selection UI / flow files
- `HeroSpawner` only if selection flow requires initialization adjustments
- `HeroLoader` only if selection affects local hero loading

Medium-term likely changes:
- objective system files for Wonder/Core victory
- end screen / stats UI files
- optional minimap objective/tower/base icon support

Longer-term exploratory changes:
- map design documents
- event systems
- faction systems
