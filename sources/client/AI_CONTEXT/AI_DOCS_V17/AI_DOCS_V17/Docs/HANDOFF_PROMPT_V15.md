Tu es un expert Unity gameplay engineer.

Projet : prototype MOBA mobile 5v5 sous Unity 6 URP, 1920x1080 paysage, New Input System, C#, namespaces `MobaPrototype.*`.

## État canonique V15
- 10 héros spawnent via `HeroSpawner`.
- 1 héros local parmi les 10 est jouable au joystick.
- `HeroRuntime` stocke la `HeroDefinition` et la `TeamId`.
- `HeroLoader` charge Byakuya et bind 4 skills runtime pour le local.
- Les 9 héros non locaux utilisent `NpcHeroAI`.
- Tous les NPC disposent de `NpcHeroRespawn`.
- `GameManager` existe et pilote l’état et le flow du match.
- `GameManager` possède le timer, le résultat, les équipes, le héros local, les transitions et la fin de match.
- `RequestMatchEnd(...)` permet à un système externe de demander la fin de match.
- `IMatchObjectiveSource`, `MatchObjectiveSourceBase` et `DebugMatchObjective` préparent la future victoire par objectif externe.
- `MatchHUDPresenter` lit le GameManager et affiche timer/state/result.
- Le HUD minimal et le end flow minimal sont validés visuellement.
- `1.5.D` est terminée : le socle GameManager est stabilisé.
- `1.6` est maintenant terminée : la minimap V1.1 est validée.

## Détails minimap V15
- `MinimapPanel` existe côté Canvas
- `MarkersRoot` est étiré sur tout le panel
- `MinimapSystem` existe en scène, hors Canvas
- `MinimapBounds` définit les bornes monde utilisées pour la projection
- `MinimapPresenter` crée et met à jour les marqueurs depuis le runtime
- `MinimapMarkerView` pilote les marqueurs unitaires UI
- 3 prefabs existent :
  - `MinimapMarker_Local`
  - `MinimapMarker_Ally`
  - `MinimapMarker_Enemy`
- le joueur local utilise un triangle orienté
- les alliés et ennemis utilisent des marqueurs simples
- les marqueurs sont clampés dans la minimap
- la rotation du joueur local est validée

## Important
- Ne pas revenir à l'ancienne logique `EnemyAI` / `EnemyMovement` / `EnemyAttack` pour les héros 5v5.
- `HeroSelector` reste désactivé à ce stade.
- `HeroSpawner` reste la porte d'entrée du spawn runtime.
- `HeroLoader` doit rester local-only (`heroLoader.enabled = isLocal`).
- `GameManager` reste propriétaire de `EndMatch()`.
- La future victoire ne doit pas être hardcodée dans le HUD.
- La future Merveille / Core devra notifier le GameManager via le système objectif.
- La minimap doit rester passive et lire les données canoniques, sans logique métier.
- Le vieux `Enemy_01` de la scène était la source du plot blanc qui suivait le joueur ; il doit rester supprimé ou inactif.

## Étapes roadmap atteintes
- 1.1 fait
- 1.2 fait
- 1.3 première version fonctionnelle faite
- 1.5 terminée sur le socle GameManager/HUD/end flow/stabilisation
- 1.6 terminée en V1.1 validée
- 1.7 faite en version prototype pour tous les NPC

## Prochain objectif recommandé à court terme
Implémenter `1.4` :
- Hero Selection Screen
- sélection propre du héros local avant le runtime
- intégration sans casser `HeroSpawner`, `HeroLoader` et le socle validé

## Chantier futur documenté mais non prioritaire court terme
- Idée de map innovante
- map dynamique
- factions
- événements
- inspiration Mobile Legends + différenciation
- éventuelle thématique dieux / machines

Cette piste doit rester documentée, mais ne doit pas remplacer les priorités de production immédiates.
