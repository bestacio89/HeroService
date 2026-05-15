# PROMPT D'ENTRÉE — Nouvelle conversation V14

Tu es un expert Unity gameplay engineer.
Je travaille sur un prototype MOBA mobile 5v5 en Unity 6 URP, 1920x1080 paysage, New Input System, C#.

Lis en priorité :
- `Docs/PROJECT_CONTEXT_V14.md`
- `Docs/HANDOFF_PROMPT_V14.md`
- `Docs/ROADMAP_STATUS_V14.md`
- `AI_DOCS_V14/AI_BOOT_PROMPT_V14.md`
- `AI_DOCS_V14/AI_CONTEXT_V14_STUDIO.md`
- `AI_DOCS_V14/AI_PROJECT_MAP_V14.md`
- `Next_Step.txt`

## État de départ à considérer comme vrai
- le runtime 5v5 local est stable
- les NPC disposent d’un respawn complet via `NpcHeroRespawn`
- `GameManager` existe, est branché et pilote le flow de match
- `RequestMatchEnd(...)` existe
- `IMatchObjectiveSource`, `MatchObjectiveSourceBase` et `DebugMatchObjective` existent
- `MatchHUDPresenter` exploite déjà le GameManager côté UI
- le HUD minimal et l’end flow minimal sont validés en runtime
- l’étape `1.5.D` est terminée
- la prochaine étape recommandée est `1.6` : minimap

## Ce qu'il ne faut pas oublier
- `HeroSelector` est toujours désactivé
- `HeroSpawner` reste central
- `HeroRuntime.Team` est la source de vérité pour allié/ennemi
- ne pas revenir à l'ancienne logique `EnemyAI`/`EnemyMovement`/`EnemyAttack`
- le plot blanc qui suivait le joueur provenait d'un ancien `Enemy_01` de scène, pas d'un 11e héros
- `HeroLoader` doit rester local-only
- la victoire finale ne doit pas être modélisée par un team wipe
- la future Merveille / Core devra piloter la victoire via le point d’extension objectif
- ne pas rouvrir des étapes déjà validées sans preuve d'une régression réelle

## Si une future IA doit agir
Elle doit partir de V14, résumer l'état actuel correctement, puis proposer la suite sans perdre l'historique V10 -> V14.
