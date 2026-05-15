Tu es un expert Unity gameplay engineer.

Projet : prototype MOBA mobile 5v5 sous Unity 6 URP, 1920x1080 paysage, New Input System, C#, namespaces `MobaPrototype.*`.

## État canonique V17
- le runtime 5v5 local reste stable
- `HeroSpawner` orchestre toujours le spawn des 10 héros et le wiring runtime
- `GameManager` reste la source de vérité du match
- `NpcHeroAI`, `NpcHeroRespawn`, le HUD minimal et la minimap V1.1 restent validés
- le flux pré-match MVP est connecté au runtime
- la transition pré-match → gameplay runtime est propre
- un vrai start flow runtime existe désormais : `PreGame` → `Countdown` → `InGame`
- les inputs gameplay sont verrouillés avant `GO` et réactivés après `GO`

## Détails runtime / pré-match V17
- `ModeSelectionPresenter` gère le choix du mode et la transition UI
- `HeroSelectionPanelPresenter` orchestre la sélection, la confirmation, le bootstrap et l'entrée dans le start flow
- `HeroSelectionSessionService` stocke mode / héros / lane / confirmation
- `MatchBootstrapper` construit puis applique le `MatchLaunchContext`
- `PreMatchRuntimeTransitionController` ferme le pré-match, active le HUD runtime et bind la caméra
- `MatchFlowSystem` porte `MatchStartFlowController` et `CountdownController`
- `PlayerInputGate` verrouille / déverrouille les inputs gameplay locaux

## Flow validé
1. choix du mode
2. ouverture du Hero Selection
3. choix du héros
4. clic Confirm
5. `MatchBootstrapper.TryPrepareAndApply()`
6. `PreMatchRuntimeTransitionController.TryEnterGameplay()`
7. `MatchStartFlowController.TryEnterPreGame()`
8. `MatchStartFlowController.TryStartCountdown()`
9. `3, 2, 1, GO`
10. `GameManager.StartMatch()`
11. `InGame`

## Important
- ne pas casser `HeroSpawner`
- ne pas déplacer de logique gameplay dans l'UI
- ne pas revenir aux scripts `EnemyAI` / `EnemyMovement` / `EnemyAttack`
- conserver `HeroSelectionSessionService` comme vérité du pré-match
- conserver `MatchBootstrapper` comme passerelle pré-match → runtime
- conserver `GameManager` comme propriétaire du match
- conserver `MatchFlowSystem` comme propriétaire du start flow runtime
- la sélection de lane n'est pas encore branchée côté gameplay
- l'objectif Wonder/Core n'est pas encore implémenté

## Prochaine étape logique après V17
`5.10.D` : Countdown UI dans le HUD runtime, puis `STEP 6` : Gameplay HUD.
