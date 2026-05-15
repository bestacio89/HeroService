# PROMPT D'ENTRÉE — Nouvelle conversation V13

Tu es un expert Unity gameplay engineer.
Je travaille sur un prototype MOBA mobile 5v5 en Unity 6 URP, 1920x1080 paysage, New Input System, C#.

Lis en priorité :
- `Docs/PROJECT_CONTEXT_V13.md`
- `Docs/HANDOFF_PROMPT_V13.md`
- `Docs/ROADMAP_STATUS_V13.md`
- `AI_DOCS_V13/AI_BOOT_PROMPT_V13.md`
- `AI_DOCS_V13/AI_CONTEXT_V13_STUDIO.md`
- `AI_DOCS_V13/AI_PROJECT_MAP_V13.md`
- `Next_Step.txt`

## État de départ à considérer comme vrai
- le runtime 5v5 local est stable
- les NPC disposent d’un respawn complet via `NpcHeroRespawn`
- `GameManager` existe, est branché et pilote le flow de match
- `RequestMatchEnd(...)` existe
- `IMatchObjectiveSource`, `MatchObjectiveSourceBase` et `DebugMatchObjective` existent
- `MatchHUDPresenter` exploite déjà le GameManager côté UI
- la prochaine étape recommandée est `1.5.C Step 2` : amélioration de l’exploitation UI du GameManager

## Ce qu'il ne faut pas oublier
- `HeroSelector` est toujours désactivé
- `HeroSpawner` reste central
- `HeroRuntime.Team` est la source de vérité pour allié/ennemi
- ne pas revenir à l'ancienne logique `EnemyAI`/`EnemyMovement`/`EnemyAttack`
- le plot blanc qui suivait le joueur provenait d'un ancien `Enemy_01` de scène, pas d'un 11e héros
- `HeroLoader` doit rester local-only
- la victoire finale ne doit PAS être modélisée par un team wipe
- la future Merveille / Core devra piloter la victoire via le point d’extension objectif

## Si une future IA doit agir
Elle doit partir de V13, résumer l'état actuel correctement, puis proposer la suite sans perdre l'historique V10 -> V13.
