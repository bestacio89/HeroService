# Changelog V15

## Objectif de cette version
V15 clôture l'étape `1.6` en livrant une **minimap V1.1 fonctionnelle** sur le prototype MOBA Unity 5v5, tout en consolidant la documentation projet pour qu'une future IA puisse reprendre le travail sans ambiguïté.

Cette version ne modifie pas la vision long terme du projet. Elle documente précisément ce qui a été fait, ce qui est validé, ce qui reste à faire à court terme, et ajoute la piste **"Idée de map innovante"** comme chantier de design futur, sans la confondre avec les priorités immédiates.

## Ce qui a été ajouté / validé en V15

### 1) Minimap V1.0 validée
- création d'un `MinimapPanel` UI
- création d'un `MarkersRoot` étiré sur toute la surface de la minimap
- ajout d'un fond visuel simple et lisible pour matérialiser la minimap
- création d'un `MinimapSystem` en scène (hors Canvas)
- ajout de `MinimapBounds`
- ajout de `MinimapPresenter`
- création des prefabs de marqueurs :
  - `MinimapMarker_Local`
  - `MinimapMarker_Ally`
  - `MinimapMarker_Enemy`
- lecture des listes canoniques via `GameManager`
- représentation du héros local, des alliés et des ennemis
- projection monde → UI fonctionnelle
- affichage runtime validé en Play Mode

### 2) Minimap V1.1 validée
- amélioration de la lisibilité des marqueurs
- ajout de tailles dédiées :
  - joueur local plus lisible
  - alliés lisibles
  - ennemis lisibles
- ajout du **clamp** des marqueurs dans les limites de la minimap
- ajout de la **rotation du marqueur du joueur local**
- remplacement du marqueur local rond par un **triangle orienté**
- validation runtime :
  - la flèche du joueur tourne correctement
  - les marqueurs restent dans le cadre
  - la minimap reste lisible en haut à droite

### 3) Décisions techniques validées pour la minimap
- la minimap reste **passive** : elle lit l'état du runtime, elle ne décide rien
- `GameManager` reste la source de vérité match-level
- `MarkersRoot` reste un simple conteneur UI
- `MinimapBounds` contient les bornes monde utilisées pour la projection
- `MinimapPresenter` orchestre la création et la mise à jour des marqueurs
- `MinimapMarkerView` reste une vue unitaire réutilisable
- le joueur local utilise un sprite orienté ; les alliés/ennemis peuvent rester en ronds simples
- aucun fog of war, ping, clic sur minimap ou icône avancée n'a été introduit en V15

### 4) Diagnostic et corrections réalisés pendant l'implémentation
- correction d'un mauvais placement initial du `MinimapPanel`
- repositionnement du panneau en haut à droite avec marge correcte
- ajout d'un fond de panel pour éviter l'impression de marqueurs "flottants"
- compréhension et correction du mode d'import du sprite triangle :
  - `Texture Type = Sprite (2D and UI)`
  - `Sprite Mode = Single`
- confirmation que le package `2D Sprite` n'était pas requis pour terminer la V1.1
- validation que la rotation du triangle local est correcte

## Ce qui reste inchangé en V15
- `HeroSpawner` reste l'orchestrateur du runtime
- `GameManager` reste propriétaire du flow de match
- `HeroLoader` reste local-only
- `HeroRuntime.Team` reste la vérité runtime pour allié/ennemi
- le système objectif (`IMatchObjectiveSource`, `MatchObjectiveSourceBase`, `DebugMatchObjective`) reste préparé pour la vraie condition de victoire future
- le HUD minimal et l'end flow minimal restent valides
- la victoire finale par "team wipe" reste non canonique

## Roadmap impactée par V15
- `1.6 Minimap` passe de **prochaine étape immédiate** à **terminée en V1.1 validée**
- la prochaine priorité court terme redevient `1.4 Hero Selection Screen`
- les chantiers suivants recommandés restent :
  1. `1.4` Hero Selection Screen
  2. vraie condition de victoire via Merveille / Core / objectif principal destructible
  3. écran de fin de partie enrichi

## Nouvelle piste documentée sans priorité court terme
Ajout explicite dans `Next_Step.txt` d'un chantier futur :
- **Idée de map innovante**
- inspiration Mobile Legends sans copie
- réflexion future sur :
  - map dynamique
  - factions
  - événements
  - asymétrie potentielle
  - variantes saisonnières

Important : cette piste est **stratégique / moyen-long terme**, pas une action de production immédiate.

## Résultat canonique V15
Le projet dispose maintenant d'un socle stabilisé avec :
- runtime 5v5 local validé
- combat et respawn NPC validés
- `GameManager` stable
- HUD minimal stable
- end flow minimal stable
- minimap V1.1 validée en runtime

V15 devient le nouveau point de reprise documentaire de référence.
