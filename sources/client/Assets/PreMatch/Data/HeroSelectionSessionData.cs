using System;

/// <summary>
/// Représente l'état canonique de la session de sélection pré-match pour le joueur local.
/// Cette donnée conserve le mode choisi, le héros actuellement sélectionné,
/// la lane préférée et l'état de confirmation.
///
/// Backend integration notes:
/// - SelectedHeroId remains the Unity presentation/runtime hero key used by HeroSpawner.
/// - SelectedHeroGuid and ActiveGameVersionId are legacy integration fields retained
///   for compatibility; they must not be used to assemble authoritative match state.
/// - Match bootstrap must consume the match-scoped definition and runtime snapshot
///   returned by MatchService when that contract is available. HeroService remains
///   authoritative for hero definitions, not the complete match execution context.
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
  /// Legacy HeroService identifier. This is not sufficient to initialize a match
  /// and must not be used as a substitute for a MatchService match snapshot.
  /// Retained temporarily for compatibility with prototype integrations.
  /// </summary>
  public string SelectedHeroGuid = string.Empty;

  /// <summary>
  /// Legacy game-version identifier retained for compatibility only.
  /// MatchService will resolve the applicable version and freeze its constraints
  /// into the match definition; Unity must not independently assemble this state.
  /// </summary>
  public string ActiveGameVersionId = string.Empty;

  // ---------------------------------------------------------------
  // Helpers
  // ---------------------------------------------------------------

  /// <summary>
  /// Indicates whether legacy backend identifiers are present. This does not mean
  /// that a server-authoritative match snapshot has been acquired.
  /// </summary>
  public bool HasBackendData =>
      !string.IsNullOrEmpty(SelectedHeroGuid) &&
      !string.IsNullOrEmpty(ActiveGameVersionId);
}