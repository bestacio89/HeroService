# MOBA Prototype Unity 6 — Project Context V15

## Executive summary
V15 is the new canonical handoff package after the project completed **Minimap 1.6 in a validated V1.1 form**.

The prototype now includes:
- 10 heroes spawned at runtime through `HeroSpawner`
- 1 local playable hero among the 10
- team-aware combat through `HeroRuntime.Team`
- working Byakuya runtime loading and combat UI binding
- basic NPC AI on the 9 non-local heroes
- full NPC death/respawn loop for NPC heroes
- a real `GameManager` for match-level ownership
- a clean objective extension path for future victory conditions
- a HUD bridge reading the `GameManager`
- a validated minimal end flow (`Ended` + `Victory/Defeat/Draw`)
- a validated **minimap V1.1**:
  - local player marker
  - ally markers
  - enemy markers
  - world-to-UI projection
  - marker clamp inside minimap
  - local player marker rotation

This package is meant to preserve the exact current state of the project so that a future AI can resume work without losing context or reopening resolved issues.

## Product vision reminder
The long-term target is a mobile-first MOBA prototype inspired by Mobile Legends but not copied from it:
- Unity 6 URP
- landscape 1920x1080
- New Input System
- C# namespaces under `MobaPrototype.*`
- solo-first implementation but network-ready architecture
- future room for a distinctive identity (mythic factions, dynamic map ideas, seasonal faction systems)

## Current roadmap status
### Phase 1 — Unity Core
#### 1.1 Spawn system for 10 heroes
Status: done.

#### 1.2 Local player among the 10 heroes
Status: done.

#### 1.3 Basic AI for the 9 other heroes
Status: done as a functional prototype version.

#### 1.4 Hero Selection Screen
Status: not started.
This becomes the next short-term production step after V15.

#### 1.5 GameManager
Status: finalized as a stable baseline.

Validated behavior:
- central match state
- timer
- result
- pause/resume/end
- team lists
- local hero reference
- clean external match-end request path
- HUD consumption
- end flow minimal
- final stabilization completed

#### 1.6 Basic minimap
Status: done in **V1.1 validated form**.

Validated behavior:
- dedicated minimap UI panel
- `MarkersRoot` stretched to panel
- read-only consumption of canonical hero lists from `GameManager`
- local player marker
- ally markers
- enemy markers
- world position projection
- marker size readability pass
- clamp inside minimap
- local player rotation with triangle marker
- runtime visibility confirmed in Play Mode

Out of scope for V15 minimap:
- fog of war
- click-to-move through minimap
- pings
- advanced objective icons
- rotating whole minimap
- zoom system

#### 1.7 Death and respawn for heroes / NPCs
Status: done in prototype form for NPCs.

## Architecture principles validated in V15
- `HeroSpawner` remains the runtime orchestration entry point
- `HeroRuntime.Team` remains the truth for ally/enemy logic
- `GameManager` owns match-level state and end flow
- external objectives own victory-rule specifics
- HUD reads state; it does not decide gameplay
- minimap reads runtime state; it does not decide gameplay
- legacy scene content must not coexist with the active runtime unless explicitly intended

## New minimap architecture in V15
- `MinimapBounds` defines the playable world bounds used by the minimap projection
- `MinimapPresenter` owns marker creation/update from runtime data
- `MinimapMarkerView` owns a single marker's UI behavior
- scene-side expected objects:
  - `Canvas/MinimapPanel`
  - `Canvas/MinimapPanel/MarkersRoot`
  - `MinimapSystem`
- marker prefabs:
  - `MinimapMarker_Local`
  - `MinimapMarker_Ally`
  - `MinimapMarker_Enemy`

## What is considered closed in V15
- V12 NPC respawn milestone
- V13 GameManager introduction milestone
- V13 HUD and end flow validation sequence
- V14 final stabilization / cleanup sequence
- V15 minimap V1.1 milestone

## Known non-goals at this stage
- no final Wonder/Core objective yet
- no full end-game recap screen yet
- no final hero selection screen yet
- no networking yet
- no final artistic map production yet

## Medium / long-term design track now documented
A future design exploration track is now explicitly documented:
- innovative map identity
- dynamic events
- factions / seasonal meta-layer
- asymmetry or controlled randomness
- mythic gods vs machine aesthetics if retained

Important:
This track is **not** the immediate production priority after V15. It is a strategic design lane to revisit after short-term core systems continue progressing.
