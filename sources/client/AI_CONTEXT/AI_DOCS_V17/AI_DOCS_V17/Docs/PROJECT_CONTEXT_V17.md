# Project Context V17

## État canonique du projet
Le prototype Unity MOBA 5v5 local dispose maintenant :
- d'un runtime match stable
- d'un flux pré-match MVP connecté au runtime
- d'une transition UX propre vers le gameplay
- d'un vrai start flow runtime avec pré-départ, countdown et input lock

La boucle validée côté runtime reste :
- spawn des 10 héros
- combat
- mort
- respawn
- fin de match

À cela s'ajoutent désormais, validés jusqu'à V17 :
- sélection du mode de jeu
- transition `ModeSelectionPanel` → `HeroSelectionPanel`
- génération dynamique d'une liste MVP de héros
- sélection visuelle d'un héros local
- persistance de la sélection dans `HeroSelectionSessionService`
- confirmation et verrouillage de sélection
- construction d'un `MatchLaunchContext`
- bootstrap pré-match → runtime via `MatchBootstrapper`
- application runtime du héros local confirmé via `HeroSpawner`
- fermeture propre du pré-match après confirmation
- activation du HUD gameplay runtime
- caméra bindée sur le héros local runtime
- création d'une phase `PreGame`
- countdown `3, 2, 1, GO`
- input gameplay verrouillé avant `GO`
- input gameplay réactivé à l'entrée en `InGame`

## Position produit réelle en V17
Le projet est en phase :
**post-fondations / start flow runtime validé / prêt pour enrichissement HUD**

## Ce qui est validé
### Runtime / match
- `HeroSpawner` reste l'orchestrateur du spawn des 10 héros
- `GameManager` reste la source de vérité du match
- `TeamManager` reste la référence allié / ennemi
- `NpcHeroAI` et `NpcHeroRespawn` restent validés
- `MatchHUDPresenter` reste branché au `GameManager`
- la minimap V1.1 reste validée

### Pré-match
- `GameModeType` validé
- `LanePreferenceType` validé comme donnée transportée
- `HeroSelectionSessionData` validé
- `MatchLaunchContext` validé
- `HeroAvailabilityService` validé
- `HeroSelectionSessionService` validé
- `ModeSelectionPresenter` validé
- `HeroSelectionItemView` validé
- `HeroSelectionPanelPresenter` validé
- `MatchBootstrapper` validé
- `PreMatchRuntimeTransitionController` validé

### Flow runtime de démarrage
- `MatchFlowSystem` validé comme objet runtime dédié
- `MatchStartFlowController` validé
- `CountdownController` validé
- `PlayerInputGate` validé
- verrouillage de mouvement validé avant `GO`
- verrouillage d'attaque de base validé avant `GO`

### Validation fonctionnelle du flow V17
Le flow validé en Play Mode est désormais :
1. lancement de la scène
2. affichage du `ModeSelectionPanel`
3. clic sur un mode (ex. `Classic`)
4. ouverture du `HeroSelectionPanel`
5. sélection d'un héros local
6. clic `Confirm`
7. `MatchBootstrapper` construit et applique un `MatchLaunchContext`
8. `PreMatchRuntimeTransitionController` ferme le pré-match et révèle le runtime
9. `MatchStartFlowController` entre en `PreGame`
10. `CountdownController` lance `3, 2, 1, GO`
11. `GameManager.StartMatch()` est appelé à la fin du countdown
12. l'input gameplay n'est autorisé qu'après `GO`

## Architecture validée en V17
- `GameManager` reste propriétaire du flow de match
- `HeroSpawner` reste propriétaire du spawn et du wiring runtime
- le pré-match reste piloté par des services dédiés
- l'UI reste passive / orchestratrice
- `HeroSelectionSessionService` reste la source de vérité du pré-match
- `MatchBootstrapper` reste la passerelle session confirmée → runtime
- `PreMatchRuntimeTransitionController` gère uniquement la transition UX/runtime
- `MatchFlowSystem` porte le start flow runtime
- `MatchStartFlowController` porte les états `Idle / PreGame / Countdown / InGame`
- `CountdownController` reste agnostique du gameplay métier
- `PlayerInputGate` centralise l'autorisation des inputs gameplay locaux

## Non-objectifs toujours valables
- pas de networking
- pas de vraie sélection de lane branchée côté gameplay
- pas de changement de scène dédié pré-match → match
- pas de vraie condition de victoire Wonder/Core encore implémentée
- pas d'écran de fin enrichi

## Priorité immédiate après V17
La prochaine étape logique est `5.10.D` :
- affichage UI du countdown dans le HUD runtime
- cohérence visuelle du pré-départ

Puis :
- `STEP 6 — Gameplay HUD`
