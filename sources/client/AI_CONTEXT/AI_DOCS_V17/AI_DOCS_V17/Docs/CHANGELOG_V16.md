# Changelog V16

## Objectif de cette version
V16 consolide tout le chantier Hero Selection démarré après V15 et le connecte proprement au runtime existant.

Cette version ne remplace pas l'architecture match validée auparavant ; elle ajoute une couche pré-match MVP qui respecte les responsabilités déjà en place.

## Ce qui a été ajouté / validé en V16

### 1) Session pré-match structurée
- validation de `GameModeType`
- validation de `LanePreferenceType`
- validation de `HeroSelectionSessionData`
- validation de `MatchLaunchContext`
- validation de `HeroSelectionSessionService` comme source de vérité du pré-match

### 2) UI de sélection MVP
- validation de `ModeSelectionPresenter`
- transition `ModeSelectionPanel` → `HeroSelectionPanel` validée
- création et validation de `HeroSelectionItemView`
- création et validation de `HeroSelectionPanelPresenter`
- génération dynamique de la liste de héros MVP
- sélection visuelle locale validée
- un seul héros sélectionné à la fois

### 3) Confirmation et verrouillage
- ajout du bouton `Confirm`
- activation conditionnelle du bouton
- confirmation de session via `HeroSelectionSessionService.Confirm()`
- verrouillage de la sélection après confirmation
- `IsConfirmed` validé en Play Mode

### 4) Bootstrap vers le runtime
- ajout de `MatchBootstrapper`
- validation de `BuildMatchLaunchContext()`
- préparation du `MatchLaunchContext` après confirmation
- ajout dans `HeroSpawner` du mapping `selectionHeroId` → `HeroDefinition`
- application du héros confirmé au héros local runtime via `TryApplyLaunchContext(...)`

### 5) Mise à jour documentaire
- commentaires de classes ajoutés / mis à jour sur les scripts produits
- `Next_Step_FINAL_V1_updated.txt` mis à jour avec l'historique jusqu'à STEP 5.8
- `script_catalogue_moba.pdf` mis à jour pour refléter l'état V16

## Décisions d'architecture confirmées en V16
- `GameManager` reste propriétaire du flow de match
- `HeroSpawner` reste l'orchestrateur runtime
- `HeroSelectionSessionService` reste la vérité du pré-match
- `MatchBootstrapper` est la passerelle pré-match → runtime
- l'UI ne contient pas de logique gameplay métier
- aucun retour vers les scripts legacy `Enemy_*`

## Ce qui reste inchangé en V16
- runtime 5v5 local stable
- NPC AI / respawn validés
- HUD minimal et end flow validés
- minimap V1.1 validée
- pas de networking
- pas de Wonder/Core final

## Résultat canonique V16
Le projet dispose maintenant :
- d'un runtime 5v5 local stable
- d'un pré-match MVP opérationnel
- d'une confirmation de héros verrouillée
- d'un `MatchLaunchContext` construit et appliqué au runtime local

V16 devient le nouveau point de reprise documentaire de référence avant les travaux de transition UX / start flow (`5.9`).
