using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// HTTP client for the HeroService bounded context.
/// Uses the shared Unity transport and keeps response mapping at the API boundary.
/// Call methods from Unity's main thread.
/// </summary>
public sealed class HeroApiClient
{
    private readonly string baseUrl;

    public HeroApiClient(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("A HeroService base URL is required.", nameof(baseUrl));
        }

        this.baseUrl = baseUrl;
    }

    /// <summary>
    /// Retrieves the legacy HeroService hero snapshot for a specific game version.
    /// This endpoint is not the authoritative match bootstrap contract. Match
    /// execution must consume a match-scoped snapshot from MatchService once available.
    /// Do not use this method to assemble match rules or runtime state client-side.
    /// </summary>
    [Obsolete("Match initialization must use MatchService's match-scoped snapshot contract.")]
    public async Task<HeroSnapshotDto> GetHeroSnapshotAsync(string heroId, string gameVersionId)
    {
        ValidateIdentifier(heroId, nameof(heroId));
        ValidateIdentifier(gameVersionId, nameof(gameVersionId));

        string path =
            $"api/v1/snapshots/heroes/{Uri.EscapeDataString(heroId)}/{Uri.EscapeDataString(gameVersionId)}";
        string json = await UnityHttpClient.GetJsonAsync(baseUrl, path, nameof(HeroApiClient));

        HeroSnapshotDto snapshot = JsonUtility.FromJson<HeroSnapshotDto>(json);
        if (snapshot == null)
        {
            throw new InvalidOperationException("HeroService returned an empty or invalid hero snapshot response.");
        }

        return snapshot;
    }

    /// <summary>
    /// Retrieves the available hero catalogue.
    /// The response contract is a direct JSON array.
    /// </summary>
    public async Task<HeroListItemDto[]> GetHeroesAsync()
    {
        string json = await UnityHttpClient.GetJsonAsync(
            baseUrl,
            "api/v1/heroes",
            nameof(HeroApiClient));

        HeroListItemDto[] heroes = JsonHelper.FromJsonArray<HeroListItemDto>(json);
        if (heroes == null)
        {
            throw new InvalidOperationException("HeroService returned an invalid hero catalogue response.");
        }

        return heroes;
    }

    private static void ValidateIdentifier(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A non-empty identifier is required.", parameterName);
        }
    }
}
