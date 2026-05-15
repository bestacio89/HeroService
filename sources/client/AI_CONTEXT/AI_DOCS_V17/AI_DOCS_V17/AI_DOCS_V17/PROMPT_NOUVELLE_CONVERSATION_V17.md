# PROMPT D'ENTRÉE — Nouvelle conversation V17

Lis en priorité :
- `Docs/PROJECT_CONTEXT_V17.md`
- `Docs/HANDOFF_PROMPT_V17.md`
- `Docs/ROADMAP_STATUS_V17.md`
- `Docs/COMPONENT_CONNECTIONS_V17.md`
- `AI_DOCS_V17/AI_BOOT_PROMPT_V17.md`
- `AI_DOCS_V17/AI_CONTEXT_V17_STUDIO.md`
- `AI_DOCS_V17/AI_PROJECT_MAP_V17.md`
- `Next_Step_FINAL_V1_updated.txt`

## État de départ à considérer comme vrai
- le runtime 5v5 local est stable
- la minimap V1.1 est déjà terminée
- le flow Hero Selection MVP est déjà implémenté
- `HeroSelectionSessionService` est la vérité du pré-match
- `MatchBootstrapper` construit et applique le `MatchLaunchContext`
- `HeroSpawner` reste central et applique le héros confirmé au runtime local
- la transition pré-match → runtime est déjà implémentée
- le start flow runtime `PreGame -> Countdown -> InGame` est déjà implémenté
- l'input gameplay est verrouillé avant `GO`
- la prochaine étape logique recommandée est `5.10.D`
