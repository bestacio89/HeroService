# AI_PROJECT_MAP_V17

## Main folders
- `Scripts/Managers`
  - `HeroSpawner.cs` — spawn orchestration and runtime application of confirmed hero context
  - `GameManager.cs` — match state, timer, true start trigger, end-match ownership
  - `TeamManager.cs` — ally/enemy team utilities
  - `DebugMatchObjective.cs` — debug end-match trigger stub
- `Scripts/Runtime/Flow`
  - `MatchStartFlowController.cs` — runtime start-flow states and orchestration
  - `CountdownController.cs` — generic countdown executor
- `Scripts/Runtime/Input`
  - `PlayerInputGate.cs` — gameplay input authorization gate for local hero
- `PreMatch/Data`
  - `GameModeType.cs`
  - `LanePreferenceType.cs`
  - `HeroSelectionSessionData.cs`
  - `MatchLaunchContext.cs`
- `PreMatch/Service`
  - `HeroAvailabilityService.cs`
  - `HeroSelectionSessionService.cs`
- `PreMatch/UI`
  - `ModeSelectionPresenter.cs`
  - `HeroSelectionItemView.cs`
  - `HeroSelectionPanelPresenter.cs`
- `PreMatch/Bootstrap`
  - `MatchBootstrapper.cs`
  - `PreMatchRuntimeTransitionController.cs`
- `Scripts/Camera`
  - `CameraFollowTopDown.cs`
- `Scripts/Player`
  - `PlayerMovement.cs`
  - `PlayerBasicAttack.cs`
  - `PlayerTargeting.cs`
  - `PlayerUltimateAbility.cs`
  - `HeroLoader.cs`, `HeroRuntime.cs`, `HeroSelector.cs`, `HeroSkillSlot.cs`, `PlayerRespawn.cs`, `TargetHighlight.cs`
- `Scripts/UI/Minimap`
  - minimap V1.1 scripts

## Most important current flow
1. scene loads
2. `HeroSpawner` spawns runtime heroes
3. mode is selected in UI
4. hero is selected in UI
5. `HeroSelectionSessionService` stores the choice
6. confirm sets `IsConfirmed`
7. `MatchBootstrapper` builds and applies `MatchLaunchContext`
8. `PreMatchRuntimeTransitionController` closes pre-match and reveals runtime
9. `MatchStartFlowController` enters `PreGame`
10. `CountdownController` runs `3, 2, 1, GO`
11. `GameManager.StartMatch()` enters active gameplay
12. `PlayerInputGate` reopens gameplay input

## Important design split
### Match ownership
- `GameManager`

### Runtime ownership
- `HeroSpawner`

### Pre-match truth
- `HeroSelectionSessionService`

### Bridge pre-match -> runtime
- `MatchBootstrapper`

### UX/runtime transition
- `PreMatchRuntimeTransitionController`

### Runtime start flow
- `MatchStartFlowController` + `CountdownController`

### Input authorization
- `PlayerInputGate`

### UI ownership
- presenters remain orchestration-only
