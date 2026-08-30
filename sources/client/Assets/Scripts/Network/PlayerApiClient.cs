using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Client HTTP pour communiquer avec le backend Player/Profile.
///
/// Responsabilités :
/// - GET /api/v1/players/{playerId} → PlayerProfileDto
///
/// Utilisation :
/// - Instancié par PlayerProfileService avec l'URL de base du Player/Profile service.
/// - Ce service est distinct de HeroService : la base URL n'est PAS la même
///   (bounded context "User" séparé, cf. architecture Loadouts/User).
/// - Les méthodes sont async/await — appeler depuis une coroutine ou un contexte async.
///
/// Contraintes :
/// - JsonUtility ne supporte pas les tableaux JSON directs ([...]) ; non pertinent ici
///   car cet endpoint retourne un objet unique.
/// - UnityWebRequest doit être utilisé depuis le thread principal Unity.
/// </summary>
public class PlayerApiClient
{
  private readonly string _baseUrl;

  public PlayerApiClient(string baseUrl)
  {
    _baseUrl = baseUrl;
  }

  // ---------------------------------------------------------------
  // GET api/v1/players/{playerId}
  // ---------------------------------------------------------------

  /// <summary>
  /// Récupère le profil du joueur (niveau, xp, monnaies, héros équipé).
  /// Appelé par PlayerProfileService au démarrage du Home.
  /// </summary>
  public async Task<PlayerProfileDto> GetProfileAsync(string playerId)
  {
    var url = $"{_baseUrl}/api/v1/players/{playerId}";

    using var req = UnityWebRequest.Get(url);
    var op = req.SendWebRequest();

    while (!op.isDone)
      await Task.Yield();

    if (req.result != UnityWebRequest.Result.Success)
    {
      Debug.LogError($"[PlayerApiClient] Profile error ({req.responseCode}): {req.error}\nURL: {url}");
      throw new Exception($"[PlayerApiClient] Profile error: {req.error}");
    }

    var json = req.downloadHandler.text;
    Debug.Log($"[PlayerApiClient] Profil reçu pour playerId={playerId}");

    return JsonUtility.FromJson<PlayerProfileDto>(json);
  }
}
