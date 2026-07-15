using System.Net.Http.Json;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Snapshots;

namespace HeroService.Client.Http.Clients;

public sealed class HeroSnapshotClient : IHeroSnapshotClient
{
  private readonly HttpClient _httpClient;

  public HeroSnapshotClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<HeroSnapshotDto?> GetAsync(
      Guid heroId,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<HeroSnapshotDto>(
        $"api/v1/snapshots/heroes/{heroId}",
        cancellationToken);
  }

  public async Task<IReadOnlyList<HeroSnapshotDto>> BrowseAsync(
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<IReadOnlyList<HeroSnapshotDto>>(
               $"api/v1/snapshots/heroes/browse",
               cancellationToken)
           ?? [];
  }
}