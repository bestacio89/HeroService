Tu es un expert Unity gameplay engineer.

Projet : prototype MOBA mobile 5v5 sous Unity 6 URP, 1920x1080 paysage, New Input System, C#, namespaces `MobaPrototype.*`.

## État canonique V16
- le runtime 5v5 local reste stable
- `HeroSpawner` orchestre toujours le spawn des 10 héros et le wiring runtime
- `GameManager` reste la source de vérité du match
- `NpcHeroAI`, `NpcHeroRespawn`, le HUD minimal et la minimap V1.1 restent validés
- `1.4 Hero Selection Screen` a progressé jusqu'à une version MVP reliée au runtime

## Détails Hero Selection V16
- `ModeSelectionPresenter` gère le choix du mode et la transition UI
- `HeroSelectionItemView` reste une vue passive par héros
- `HeroSelectionPanelPresenter` construit la liste, gère la sélection, la confirmation et le lock
- `HeroSelectionSessionService` stocke mode / héros / lane / confirmation
- `MatchBootstrapper` construit le `MatchLaunchContext` puis l'applique au runtime
- `HeroSpawner` sait résoudre un `selectionHeroId` vers une `HeroDefinition` runtime

## Flow validé
1. choix du mode
2. ouverture du Hero Selection
3. choix du héros
4. clic Confirm
5. `IsConfirmed = true`
6. `BuildMatchLaunchContext()`
7. `HeroSpawner.TryApplyLaunchContext(...)`

## Important
- ne pas casser `HeroSpawner`
- ne pas déplacer de logique gameplay dans l'UI
- ne pas revenir aux scripts `EnemyAI` / `EnemyMovement` / `EnemyAttack`
- conserver `HeroSelectionSessionService` comme vérité du pré-match
- conserver `MatchBootstrapper` comme passerelle pré-match → runtime
- la sélection de lane n'est pas encore branchée côté gameplay
- l'objectif Wonder/Core n'est pas encore implémenté

## Prochaine étape logique après V16
`5.9` : transition UX / fermeture propre du pré-match après confirmation, puis clarification du vrai start flow du match.
