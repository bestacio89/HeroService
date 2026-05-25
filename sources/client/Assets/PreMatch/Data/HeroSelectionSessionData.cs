using System;

/// <summary>
/// Représente l'état canonique de la session de sélection pré-match pour le joueur local.
/// Cette donnée conserve le mode choisi, le héros actuellement sélectionné,
/// la lane préférée et l'état de confirmation.
///
/// Évolution V4 — intégration backend :
/// - SelectedHeroId reste la clé locale Unity (ex: "hero_mage") utilisée
///   par HeroSpawner pour résoudre le HeroDefinition en scène.
/// - SelectedHeroGuid est le Guid retourné par l'API backend (GET /api/v1/heroes).
///   Il est utilisé par MatchBootstrapper pour appeler GET /api/v1/snapshots/heroes/{guid}/{versionId}.
/// - ActiveGameVersionId est le Guid de la version de jeu active, requis
///   par l'endpoint snapshot pour un calcul déterministe des stats.
///
/// Règle de priorité :
/// - SelectedHeroGuid vide = pas encore de backend connecté → comportement legacy.
/// - SelectedHeroGuid renseigné = backend connecté → stats viennent du snapshot.
/// </summary>
[Serializable]
public class HeroSelectionSessionData
{
  // ---------------------------------------------------------------
  // Données existantes — inchangées
  // ---------------------------------------------------------------

  public GameModeType SelectedMode = GameModeType.Classic;

  /// <summary>
  /// Identifiant local Unity du héros sélectionné (ex: "hero_mage").
  /// Utilisé par HeroSpawner pour résoudre le HeroDefinition en scène.
  /// Conservé pour la compatibilité avec le système de spawn existant.
  /// </summary>
  public string SelectedHeroId = string.Empty;

  public LanePreferenceType PreferredLane = LanePreferenceType.None;

  public bool IsConfirmed = false;

  // ---------------------------------------------------------------
  // Nouvelles données — intégration backend (Phase 3 / Phase 4)
  // ---------------------------------------------------------------

  /// <summary>
  /// Guid du héros retourné par l'API backend (GET /api/v1/heroes).
  /// Utilisé par MatchBootstrapper pour appeler l'endpoint snapshot.
  /// Vide tant que le backend n'est pas connecté.
  /// </summary>
  public string SelectedHeroGuid = string.Empty;

  /// <summary>
  /// Guid de la version de jeu active, requis par GET /api/v1/snapshots/heroes/{heroId}/{versionId}.
  /// Sera fourni par la configuration de l'application ou récupéré depuis l'API.
  /// Vide tant que le backend n'est pas connecté.
  /// </summary>
  public string ActiveGameVersionId = string.Empty;

  // ---------------------------------------------------------------
  // Helpers
  // ---------------------------------------------------------------

  /// <summary>
  /// Indique si le backend est connecté et les données Guid disponibles.
  /// Quand false, le système fonctionne en mode legacy (ScriptableObjects statiques).
  /// </summary>
  public bool HasBackendData =>
      !string.IsNullOrEmpty(SelectedHeroGuid) &&
      !string.IsNullOrEmpty(ActiveGameVersionId);
}