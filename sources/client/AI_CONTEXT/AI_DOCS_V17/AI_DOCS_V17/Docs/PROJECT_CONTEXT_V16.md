# Project Context V16

## État canonique du projet
Le prototype Unity MOBA 5v5 local dispose désormais d'un socle stable **et** d'un flux pré-match MVP relié au runtime.

La boucle validée reste :
- spawn des 10 héros
- combat
- mort
- respawn
- fin de match

À cela s'ajoutent maintenant les briques suivantes, validées après V15 :
- sélection du mode de jeu
- transition `ModeSelectionPanel` → `HeroSelectionPanel`
- génération dynamique d'une liste MVP de héros
- sélection visuelle d'un héros local
- persistance de la sélection dans `HeroSelectionSessionService`
- confirmation de sélection avec verrouillage (`IsConfirmed`)
- construction d'un `MatchLaunchContext`
- bootstrap pré-match → runtime via `MatchBootstrapper`
- application runtime du héros local confirmé via `HeroSpawner`

## Position produit réelle en V16
Le projet est en phase :
**post-fondations / pré-feature gameplay avancé**

Le runtime de match reste stable, et le chantier `1.4 Hero Selection Screen` est désormais en **version MVP connectée au runtime**.

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
- `HeroAvailabilityService` validé comme service MVP de disponibilité
- `HeroSelectionSessionService` validé comme source de vérité du pré-match
- `ModeSelectionPresenter` validé
- `HeroSelectionItemView` validé
- `HeroSelectionPanelPresenter` validé
- `MatchBootstrapper` validé

### Validation fonctionnelle du flow V16
Le flow validé en Play Mode est désormais :
1. lancement de la scène
2. affichage du `ModeSelectionPanel`
3. clic sur un mode (ex. `Classic`)
4. ouverture du `HeroSelectionPanel`
5. génération dynamique de la liste de héros MVP
6. clic sur un héros → sélection visuelle + mise à jour de `HeroSelectionSessionService`
7. bouton `Confirm` activé après sélection
8. clic `Confirm` → confirmation session + verrouillage
9. `MatchBootstrapper` construit un `MatchLaunchContext`
10. `HeroSpawner` applique le contexte au runtime local via mapping `selectionHeroId` → `HeroDefinition`

## Architecture validée en V16
- `GameManager` reste propriétaire du flow de match
- `HeroSpawner` reste propriétaire du spawn et du wiring runtime
- le pré-match reste piloté par des services dédiés
- l'UI reste passive / orchestratrice, jamais propriétaire du gameplay
- `HeroSelectionSessionService` reste la source de vérité du pré-match
- `MatchBootstrapper` est la passerelle entre session confirmée et runtime
- le mapping pré-match → runtime reste centralisé dans `HeroSpawner`

## Non-objectifs toujours valables
- pas de networking
- pas de vraie sélection de lane branchée côté gameplay
- pas de changement de scène dédié pré-match → match
- pas de vraie condition de victoire Wonder/Core encore implémentée
- pas d'écran de fin enrichi

## Priorité immédiate après V16
La prochaine étape logique est `5.9` :
- transition UX propre après confirmation
- clarification du passage vers le match
- éventuel masquage / fermeture du pré-match
- préparation d'un vrai start flow sans casser le runtime
