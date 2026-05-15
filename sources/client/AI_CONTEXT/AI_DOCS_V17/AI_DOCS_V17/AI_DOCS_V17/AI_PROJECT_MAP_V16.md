# AI_PROJECT_MAP_V16

## Main folders
- `Scripts/Managers`
  - `HeroSpawner.cs` — spawn orchestration and runtime application of confirmed hero context
  - `GameManager.cs` — match state, timer, flow, end-match ownership
  - `TeamManager.cs` — ally/enemy team utilities
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
- `Scripts/UI/Minimap`
  - minimap V1.1 scripts

## Most important current flow
1. scene loads
2. `HeroSpawner` spawns runtime heroes
3. mode is selected in UI
4. hero is selected in UI
5. `HeroSelectionSessionService` stores the choice
6. confirm sets `IsConfirmed`
7. `MatchBootstrapper` builds `MatchLaunchContext`
8. `HeroSpawner.TryApplyLaunchContext(...)` applies the confirmed hero to the local runtime hero

## Important design split
### Match ownership
- `GameManager`

### Runtime ownership
- `HeroSpawner`

### Pre-match truth
- `HeroSelectionSessionService`

### Bridge pre-match -> runtime
- `MatchBootstrapper`

### UI ownership
- presenters remain orchestration-only
