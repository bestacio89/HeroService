# Changelog V17

## Objet
V17 consolide la transition pré-match → runtime et le start flow du match.

## Résumé des évolutions depuis V16
V16 validait une Hero Selection MVP connectée au runtime.
V17 ajoute les briques suivantes sans casser le socle match :

### STEP 5.9 — Transition UX / runtime
- ajout de `PreMatchRuntimeTransitionController`
- fermeture propre du pré-match après confirmation
- activation du `CombatUI` runtime
- binding explicite de la caméra sur `GameManager.LocalHero` via `CameraFollowTopDown.SetTarget(...)`
- maintien de `HeroSelectionPanelPresenter` comme orchestrateur UI uniquement

### STEP 5.10.A — Phase de pré-départ
- création d'un `MatchFlowSystem` dédié sous `Managers`
- ajout de `MatchStartFlowController`
- séparation explicite entre pré-match et runtime start flow
- nouvelle phase `PreGame` après la transition runtime

### STEP 5.10.B — Countdown
- ajout de `CountdownController`
- ajout d'un état `Countdown` dans `MatchStartFlowController`
- retrait du démarrage automatique du match dans `GameManager.InitializeMatch(...)`
- démarrage réel du match déplacé à la fin du countdown via `GameManager.StartMatch()`
- ordre de logs rendu cohérent : `Countdown started` puis `3, 2, 1, GO`

### STEP 5.10.C — Input Lock
- ajout de `PlayerInputGate` sur le héros runtime local
- `MatchStartFlowController` désactive les inputs en `PreGame` / `Countdown`
- `MatchStartFlowController` réactive les inputs en `InGame`
- `PlayerMovement` consulte désormais `PlayerInputGate`
- `PlayerBasicAttack` consulte désormais `PlayerInputGate`
- validation Play Mode : impossible de bouger ou d'attaquer avant `GO`, fonctionnement normal après `GO`

## Refactors structurants validés
- `GameManager.InitializeMatch(...)` initialise le match sans l'entrer immédiatement en `Playing`
- `GameManager.StartMatch()` devient le point d'entrée réel du gameplay actif
- `HeroSelectionPanelPresenter` orchestre désormais :
  1. `Confirm()`
  2. `MatchBootstrapper.TryPrepareAndApply()`
  3. `PreMatchRuntimeTransitionController.TryEnterGameplay()`
  4. `MatchStartFlowController.TryEnterPreGame()`
  5. `MatchStartFlowController.TryStartCountdown()`
- création d'une séparation explicite entre :
  - `PreMatchSystem` = données / sélection / bootstrap / transition
  - `MatchFlowSystem` = flow runtime du démarrage

## Conséquences produit / architecture
- le passage pré-match → runtime est maintenant clair, visible et testable
- le match n'entre plus en gameplay actif tant que le countdown n'est pas terminé
- l'UI n'est toujours pas propriétaire de logique gameplay
- `HeroSpawner` n'a pas été transformé en système de transition, ce qui préserve son rôle runtime

## État final V17
Validé en Play Mode :
1. `ModeSelectionPanel`
2. `HeroSelectionPanel`
3. sélection du héros
4. `Confirm`
5. bootstrap runtime appliqué
6. fermeture du pré-match
7. affichage du HUD runtime
8. `PreGame`
9. countdown `3, 2, 1, GO`
10. entrée en `InGame`
11. input gameplay seulement après `GO`

## Prochaine étape recommandée après V17
- `STEP 5.10.D — Countdown UI`
- puis `STEP 6 — Gameplay HUD`
