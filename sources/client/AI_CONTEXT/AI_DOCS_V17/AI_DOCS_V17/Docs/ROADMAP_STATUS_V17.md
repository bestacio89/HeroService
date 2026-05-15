# Roadmap Status V17

## Vision produit
Construire une fondation Unity robuste pour un MOBA 5v5 pensé réseau dès le départ, tout en validant les systèmes gameplay par couches successives.

## Phase 1 — Unity Core
### 1.1 Spawn système 10 héros
Statut : terminé.

### 1.2 Player local parmi les 10
Statut : terminé.

### 1.3 IA basique sur les 9 autres héros
Statut : première version fonctionnelle terminée.

### 1.4 Hero Selection Screen
Statut : **quasi clôturée côté MVP runtime-connectée**.

Validé jusqu'en V17 :
- `ModeSelectionPresenter`
- `HeroSelectionItemView`
- `HeroSelectionPanelPresenter`
- `HeroSelectionSessionService`
- `MatchBootstrapper`
- bouton `Confirm`
- verrouillage après confirmation
- `BuildMatchLaunchContext()`
- application du contexte au runtime via `HeroSpawner.TryApplyLaunchContext(...)`
- transition UX vers le runtime
- phase `PreGame`
- countdown runtime
- input lock jusqu'à `GO`

Reste à faire pour clôture plus complète de 1.4 :
- countdown UI visible dans le HUD runtime
- polish visuel final du lancement
- extension future lane / owned heroes / data source réelle
- éventuel passage vers une scène match dédiée si le projet l'exige plus tard

### 1.5 GameManager
Statut : terminé sur le socle actuel, avec start flow runtime désormais découplé du bootstrap.

### 1.6 Minimap basique
Statut : terminée en V1.1 validée.

### 1.7 Mort et Respawn des héros / NPC
Statut : terminé en prototype pour les NPC ; base joueur locale en place.

## Phase 2 — Assets visuels
Statut : non commencé au sens production.

## Phase 3 — Réseau
Statut : non commencé.

## Priorité immédiate recommandée après V17
1. `5.10.D` — Countdown UI dans le HUD runtime
2. `STEP 6` — Gameplay HUD (HP, mana, cooldowns, infos héros local)
3. vraie condition de victoire via Wonder / Core / objectif principal destructible
4. écran de fin de partie enrichi

## Chantier de design futur documenté
### Idée de map innovante
Statut : piste documentée, non prioritaire court terme.

Axes possibles à explorer plus tard :
- map dynamique
- événements de map
- factions
- asymétrie contrôlée
- identité forte du projet

Important :
Cette piste ne doit pas détourner les priorités court terme du socle gameplay et du flow pré-match/runtime.
