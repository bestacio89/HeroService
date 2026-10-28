using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// HTTP client for the Player/Profile bounded context.
/// Its base URL is configured independently from HeroService.
/// Call methods from Unity's main thread.
/// </summary>
public sealed class PlayerApiClient
{
    private readonly string baseUrl;

    public PlayerApiClient(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("A player service base URL is required.", nameof(baseUrl));
        }

        this.baseUrl = baseUrl;
    }

    /// <summary>
    /// Retrieves a player's profile.
    /// </summary>
    public async Task<PlayerProfileDto> GetProfileAsync(string playerId)
    {
        if (string.IsNullOrWhiteSpace(playerId))
        {
            throw new ArgumentException("A non-empty player identifier is required.", nameof(playerId));
        }

        string path = $"api/v1/players/{Uri.EscapeDataString(playerId)}";
        string json = await UnityHttpClient.GetJsonAsync(baseUrl, path, nameof(PlayerApiClient));

        PlayerProfileDto profile = JsonUtility.FromJson<PlayerProfileDto>(json);
        if (profile == null)
        {
            throw new InvalidOperationException("The player service returned an empty or invalid profile response.");
        }

        return profile;
    }
}
