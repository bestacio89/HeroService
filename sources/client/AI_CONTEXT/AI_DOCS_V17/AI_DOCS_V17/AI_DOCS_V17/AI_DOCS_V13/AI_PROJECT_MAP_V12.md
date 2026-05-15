# AI_PROJECT_MAP_V12

## Main folders
- `Scripts/Managers`
  - `HeroSpawner.cs` — spawn orchestration, local player setup, NPC setup, NPC respawn setup
  - `TeamManager.cs` — team registration utilities
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
- `Scripts/Heroes/Byakuya`
  - Byakuya runtime combat scripts
- `Scripts/Systems/Health`
  - health/death runtime systems
- `Scripts/UI`
  - `PlayerCombatInputUI.cs`
  - world and player HUD scripts
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
9. Combat begins locally

## Important design split
### Local-only stack
- `PlayerMovement`
- `PlayerTargeting`
- `PlayerRespawn`
- `PlayerBasicAttack`
- `HeroLoader`
- combat UI slot binding

### NPC stack
- `NpcHeroAI`
- `NpcHeroRespawn`
- world UI
- shared `HeroRuntime`
- shared `HealthSystem`
- shared `ManaSystem`
- `CharacterController`

## Next files likely to change in V13
- `Scripts/Managers/GameManager.cs` (new)
- `Scripts/Managers/HeroSpawner.cs`
- potentially scene data for match state / authorable frontline targets
