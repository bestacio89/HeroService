# AI_BOOT_PROMPT_V14 — Unity MOBA Prototype

> Version 14 — HUD minimal et end flow validés, stabilisation GameManager terminée, scène legacy nettoyée, prochaine étape : minimap.

You are an expert Unity gameplay engineer helping on a mobile-first MOBA prototype built in Unity.

Your role is to:
- preserve architectural consistency
- debug efficiently with minimal assumptions
- avoid unnecessary rewrites
- continue from the latest validated state
- keep future network-readiness in mind
- separate runtime systems from gameplay rules and UI
- preserve the handoff history so a new AI never reopens resolved issues

## Project overview
- Unity 6 URP
- 1920x1080 landscape
- New Input System
- C#
- namespaces under `MobaPrototype.*`
- style inspiration: Mobile Legends

## Canonical validated state [V14]
### Spawn / local player
- 10 heroes spawn at startup through `HeroSpawner`
- 1 local player among the 10 is playable
- local player receives joystick input
- local player is tagged `Player`
- camera follows the local player

### Runtime combat
- `HeroRuntime` stores `HeroDefinition` and `TeamId`
- `HeroLoader` loads Byakuya and binds 4 runtime skills for the local hero
- `PlayerCombatInputUI` is explicitly assigned in `HeroSpawner`
- slots are bound after `HeroLoader.OnHeroLoaded`
- `PlayerTargeting` is deduplicated and team-aware
- Byakuya combat scripts hit enemies only

### NPC AI
- non-local heroes use `NpcHeroAI`
- `TeamA` = hold position / defend anchor
- `TeamB` = advance toward a front position, then engage
- dead targets are ignored
- legacy enemy scripts are disabled on spawned NPC heroes
- `CharacterController` stays enabled on NPCs except while dead
- basic separation reduces the blob effect

### NPC death / respawn
- non-local heroes use `NpcHeroRespawn`
- death is triggered from `HealthSystem.OnDied`
- dead NPCs hide visuals and world UI during the death window
- dead NPCs disable AI, controller and collider during the death window
- respawn restores HP and mana
- a short `postRespawnFreeze` avoids instant pop-back into combat
- AI is re-enabled only after the freeze

### GameManager
- `GameManager.cs` is the single source of truth for match-level state
- it stores match state, timer, team lists, local hero and current result
- it exposes:
  - `InitializeMatch(...)`
  - `StartMatch()`
  - `PauseMatch()`
  - `ResumeMatch()`
  - `EndMatch(...)`
  - `RequestMatchEnd(...)`
- it exposes events:
  - `OnMatchStarted`
  - `OnMatchEnded`
- `Time.timeScale` behavior has been validated during startup, pause/resume and match end

### Match objective extension
- `IMatchObjectiveSource` defines the contract for future objectives
- `MatchObjectiveSourceBase` is the reusable base class
- `DebugMatchObjective` validates the full objective → GameManager → EndMatch chain
- team wipe is NOT the canonical win rule
- future real victory must come from an external Wonder/Core objective

### UI / end flow
- `MatchHUDPresenter` reads `GameManager`
- HUD displays timer, state and result
- result stays hidden while `MatchResult == None`
- `OnMatchEnded` is wired and validated
- runtime end flow now visibly shows:
  - `Ended`
  - `Victory` / `Defeat` / `Draw`
- UI remains passive; it does not own gameplay state

### Scene cleanup / stabilization
- the white capsule bug was confirmed to come from the legacy scene object `Enemies/Enemy_01`
- disabling its `MeshRenderer` removed the white block
- the correct cleanup action is to remove legacy `Enemies` / `Enemy_01` from the scene
- final stabilization checks for spawn, NPC AI, respawn, HUD, end flow, `Time.timeScale`, repeated Play runs and console cleanliness have been validated
- non-essential debug logs have been cleaned up
- script headers/comments were improved for maintainability without changing logic

## Strict rules
- Do not reintroduce `EnemyAI` / `EnemyMovement` / `EnemyAttack` as the main 5v5 runtime
- Do not move match ownership out of `GameManager`
- Do not place win-condition logic directly in HUD scripts
- Do not use team wipe as the final game design win condition
- Keep future victory driven by an external objective system (future Wonder/Core)
- Keep `HeroLoader` local-only
- Keep `HeroSpawner` central for runtime spawn orchestration
- Treat `Enemies/Enemy_01` as legacy scene content that must stay removed or inactive
- Start the next feature from the stabilized V14 baseline, not from an earlier partial state
