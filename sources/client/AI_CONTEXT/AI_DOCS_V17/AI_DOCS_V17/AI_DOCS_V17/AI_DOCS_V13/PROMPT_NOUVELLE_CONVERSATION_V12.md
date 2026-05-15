# PROMPT D'ENTRÉE — Nouvelle conversation V12

Tu es un expert Unity gameplay engineer.
Je travaille sur un prototype MOBA mobile 5v5 en Unity 6 URP, 1920x1080 paysage, New Input System, C#.

Lis en priorité :
- `Docs/PROJECT_CONTEXT_V12.md`
- `Docs/HANDOFF_PROMPT_V12.md`
- `Docs/ROADMAP_STATUS_V12.md`
- `AI_DOCS_V12/AI_BOOT_PROMPT_V12.md`
- `AI_DOCS_V12/AI_CONTEXT_V12_STUDIO.md`
- `AI_DOCS_V12/AI_PROJECT_MAP_V12.md`
- `Next_Step.txt`

## État de départ à considérer comme vrai
- le combat local Byakuya est validé
- les filtres par équipe sont validés
- les 9 héros non locaux utilisent une IA simple via `NpcHeroAI`
- les NPC disposent maintenant d'un respawn complet via `NpcHeroRespawn`
- la prochaine milestone structurante recommandée est `1.5 GameManager`

## Ce qu'il ne faut pas oublier
- `HeroSelector` est toujours désactivé
- `HeroSpawner` reste central
- `HeroRuntime.Team` est la source de vérité pour allié/ennemi
- ne pas revenir à l'ancienne logique `EnemyAI`/`EnemyMovement`/`EnemyAttack`
- le plot blanc qui suivait le joueur provenait d'un ancien `Enemy_01` de scène, pas d'un 11e héros
- le placement des spawn points dans la scène influence fortement le ressenti IA
- `HeroLoader` doit rester local-only

## Si une future IA doit agir
Elle doit partir de V12, résumer l'état actuel correctement, puis proposer les prochains changements sans perdre l'historique V9 -> V12.
