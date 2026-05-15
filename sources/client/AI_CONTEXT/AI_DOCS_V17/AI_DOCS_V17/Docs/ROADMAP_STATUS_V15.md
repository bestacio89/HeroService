# Roadmap Status V15

## Vision produit
Construire la bonne fondation Unity pour un MOBA 5v5 multijoueur, sans réseau pour l'instant mais pensé réseau dès le début.

## Phase 1 — Unity Core
### 1.1 Spawn système 10 héros
Statut : terminé.

### 1.2 Player local parmi les 10
Statut : terminé.

### 1.3 IA basique sur les 9 autres héros
Statut : première version fonctionnelle terminée.

Implémenté :
- `NpcHeroAI` sur les 9 héros non locaux
- Team-aware via `HeroRuntime.Team`
- TeamA = défense à l'ancre
- TeamB = avance vers frontline
- attaque de base simple
- espacement dynamique léger

Reste à améliorer plus tard :
- priorité des cibles
- lane logic
- skills IA

### 1.4 Hero Selection Screen
Statut : non commencé.

Nouvelle priorité court terme après V15.
Objectif attendu :
- écran de sélection de héros avant le runtime match
- choix propre du héros local
- préservation du runtime existant sans casser `HeroSpawner` et `HeroLoader`

### 1.5 GameManager
Statut : terminé sur le socle actuel.

Implémenté :
- `GameManager.cs`
- timer
- états de match
- résultat
- hooks de match
- pause / reprise / fin de match
- `RequestMatchEnd(...)`
- système objectif :
  - `IMatchObjectiveSource`
  - `MatchObjectiveSourceBase`
  - `DebugMatchObjective`
- HUD :
  - `MatchHUDPresenter`
- end flow minimal validé
- stabilisation finale validée

Sous-étapes validées :
- `1.5.C Step 2` ✅
- `1.5.C Step 3` ✅
- `1.5.D` ✅

### 1.6 Minimap basique
Statut : terminée en V1.1 validée.

Implémenté :
- `MinimapPanel`
- `MarkersRoot`
- `MinimapSystem`
- `MinimapBounds`
- `MinimapPresenter`
- `MinimapMarkerView`
- prefabs :
  - `MinimapMarker_Local`
  - `MinimapMarker_Ally`
  - `MinimapMarker_Enemy`
- projection monde → minimap
- distinction visuelle :
  - joueur local
  - alliés
  - ennemis
- amélioration de lisibilité des marqueurs
- clamp dans le cadre minimap
- rotation du joueur local avec marqueur triangle

Non inclus volontairement :
- fog of war
- pings
- clic sur minimap
- icônes d'objectifs avancés
- zoom / rotation de la carte entière

### 1.7 Mort et Respawn des héros / NPC
Statut : terminé en prototype pour les NPC.

Validé :
- Joueur local : base existante via `PlayerRespawn`
- NPC : `NpcHeroRespawn` implémenté et validé en Play Mode
- disparition visuelle pendant la mort
- retour au spawn d'origine
- restauration HP/mana
- reprise différée de l'IA

## Phase 2 — Assets visuels
Statut : non commencé au sens production.

Rappel important :
- MidJourney / Mushy / outils visuels ne sont pas prioritaires tant que le design gameplay et la structure de map ne sont pas validés
- la logique actuelle reste prototype-first / greybox-first

## Phase 3 — Réseau
Statut : non commencé.

## Priorité immédiate recommandée
1. `1.4` — Hero Selection Screen
2. vraie condition de victoire via Wonder / Core / objectif principal destructible
3. écran de fin de partie enrichi
4. itérations minimap optionnelles seulement si nécessaire (objectifs/tours/base)

## Chantier de design futur documenté (non prioritaire court terme)
### Idée de map innovante
Statut : piste de réflexion ajoutée, non engagée en production.

Axes possibles à explorer plus tard :
- map dynamique
- événements de map
- factions
- asymétrie contrôlée
- inspiration Mobile Legends sans copie
- identité forte potentielle : dieux / machines / guerre de factions saisonnière

Important :
Cette piste ne doit pas détourner les priorités court terme du socle gameplay.
