# AI_CONTEXT_V12_STUDIO

## Goal of this context file
Help a future AI continue the project from the current real state instead of re-debugging already solved issues.

## What changed recently
1. V10 stabilized local combat runtime.
2. V11 introduced `NpcHeroAI` and extended `HeroSpawner` so the 9 non-local heroes now behave as a simple 5v5 prototype.
3. V12 introduced `NpcHeroRespawn` and validated NPC death/respawn in Play Mode.
4. A white capsule bug was traced to an old scene object `Enemies/Enemy_01`, not to `HeroSpawner`.
5. The packaged V12 scene deactivates `Enemies` to keep only the new 5v5 flow active.

## Practical debugging checklist
### If NPCs do not move
- confirm `CharacterController` is enabled on NPCs after spawn / after respawn
- confirm `NpcHeroAI` is added on non-local heroes
- confirm `HealthSystem.IsDead == false`
- confirm spawn points are not stacked on top of each other

### If NPC death/respawn breaks
- confirm `NpcHeroRespawn` is present on non-local heroes
- confirm `SetRespawnPoint(...)` is called from `HeroSpawner`
- confirm `HealthSystem.OnDied` fires
- confirm HP is restored with `ReviveFull()`
- confirm renderers/canvases are hidden instead of disabling whole objects
- confirm `postRespawnFreeze` is not excessively large

### If TeamB leaves the visible map
- confirm `SetAdvanceTargetPosition(...)` is called in `HeroSpawner`
- confirm `NpcHeroAI.UpdateAdvance()` no longer uses `transform.forward * 2f`

### If everyone blobs together again
- inspect `separationRadius` and `separationStrength`
- verify scene spawn points are sufficiently spaced
- check if `OverlapSphere` is detecting multiple colliders per hero

### If a white capsule follows the player again
- inspect the scene hierarchy for legacy objects under `Enemies`
- confirm `Enemy_01` or similar old scene enemies are inactive or removed
- do not misdiagnose this as an 11th hero without checking the hierarchy first

### If local combat breaks
- verify `PlayerCombatInputUI` reference in `HeroSpawner`
- verify slots are assigned after `HeroLoader.OnHeroLoaded`
- verify `HeroLoader` is enabled only on the local player

## Editor reminders
- `SpawnPoints` are children under `HeroSpawner`
- moving `HeroSpawner` moves every spawn point
- all hero roots should remain on layer `Hero`
- local player still uses tag `Player`
- the scene may still contain legacy helper objects; validate them before assuming a runtime bug
