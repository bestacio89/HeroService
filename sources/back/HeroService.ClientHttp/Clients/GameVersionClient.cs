using System.Net.Http.Json;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.GameVersions;
using HeroService.Contracts.Commands.GameVersions;

namespace HeroService.Client.Http.Clients;

public sealed class GameVersionClient : IGameVersionClient
{
  private readonly HttpClient _httpClient;

  public GameVersionClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<Guid> CreateAsync(
      CreateGameVersionCommand command,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync(
        "api/v1/game-versions",
        command,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid?>(cancellationToken)
           ?? Guid.Empty;
  }

  public async Task<GameVersionDto?> GetActiveAsync(
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<GameVersionDto>(
        "api/v1/game-versions/active",
        cancellationToken);
  }

  public async Task<GameVersionDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<GameVersionDto>(
        $"api/v1/game-versions/by-name/{Uri.EscapeDataString(name)}",
        cancellationToken);
  }
}