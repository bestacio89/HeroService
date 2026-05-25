using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Client HTTP pour communiquer avec le backend HeroService.
///
/// Responsabilités :
/// - GET /api/v1/snapshots/heroes/{heroId}/{gameVersionId} → HeroSnapshotDto
/// - GET /api/v1/heroes → HeroListItemDto[] (disponible quand HeroController sera exposé)
///
/// Utilisation :
/// - Instancié par MatchBootstrapper avec l'URL de base du backend.
/// - Les méthodes sont async/await — appeler depuis une coroutine ou un contexte async.
///
/// Contraintes :
/// - JsonUtility ne supporte pas les tableaux JSON directs ([...]).
///   Pour GetHeroesAsync(), un JsonHelper wrapper est requis.
/// - UnityWebRequest doit être utilisé depuis le thread principal Unity.
/// </summary>
public class HeroApiClient
{
  private readonly string _baseUrl;

  public HeroApiClient(string baseUrl)
  {
    _baseUrl = baseUrl;
  }

  // ---------------------------------------------------------------
  // GET api/v1/snapshots/heroes/{heroId}/{gameVersionId}
  // ---------------------------------------------------------------

  /// <summary>
  /// Récupère le snapshot complet d'un héros pour une version de jeu donnée.
  /// Appelé par MatchBootstrapper après confirmation de la sélection du héros.
  /// </summary>
  public async Task<HeroSnapshotDto> GetHeroSnapshotAsync(
      string heroId,
      string gameVersionId)
  {
    var url = $"{_baseUrl}/api/v1/snapshots/heroes/{heroId}/{gameVersionId}";

    using var req = UnityWebRequest.Get(url);
    var op = req.SendWebRequest();

    while (!op.isDone)
      await Task.Yield();

    if (req.result != UnityWebRequest.Result.Success)
    {
      Debug.LogError($"[HeroApiClient] Snapshot error ({req.responseCode}): {req.error}\nURL: {url}");
      throw new Exception($"[HeroApiClient] Snapshot error: {req.error}");
    }

    var json = req.downloadHandler.text;
    Debug.Log($"[HeroApiClient] Snapshot reçu pour heroId={heroId}");

    return JsonUtility.FromJson<HeroSnapshotDto>(json);
  }

  // ---------------------------------------------------------------
  // GET api/v1/heroes
  // ---------------------------------------------------------------

  /// <summary>
  /// Récupère la liste des héros disponibles.
  /// Disponible quand HeroController sera exposé côté backend.
  /// Appelé par HeroAvailabilityService (Phase 3).
  /// </summary>
  public async Task<HeroListItemDto[]> GetHeroesAsync()
  {
    var url = $"{_baseUrl}/api/v1/heroes";

    using var req = UnityWebRequest.Get(url);
    var op = req.SendWebRequest();

    while (!op.isDone)
      await Task.Yield();

    if (req.result != UnityWebRequest.Result.Success)
    {
      Debug.LogError($"[HeroApiClient] Heroes error ({req.responseCode}): {req.error}\nURL: {url}");
      throw new Exception($"[HeroApiClient] Heroes error: {req.error}");
    }

    var json = req.downloadHandler.text;
    Debug.Log($"[HeroApiClient] Liste héros reçue.");

    // JsonUtility ne supporte pas les tableaux JSON directs — wrapper requis.
    // Si l'API retourne { "items": [...] }, utiliser JsonUtility.FromJson<HeroListWrapper>(json).items
    // Si l'API retourne [...] directement, utiliser JsonHelper.FromJsonArray<HeroListItemDto>(json).
    return JsonHelper.FromJsonArray<HeroListItemDto>(json);
  }
}