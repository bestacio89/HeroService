using System.Net.Http.Json;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Heroes;
using HeroService.Contracts.DTOs.Heroes;

namespace HeroService.Client.Http.Clients;

public sealed class HeroProgressionClient : IHeroProgressionClient
{
  private readonly HttpClient _httpClient;

  public HeroProgressionClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<HeroProgressionModifiersDto?> GetByHeroAsync(
      Guid heroId,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<HeroProgressionModifiersDto>(
        $"api/v1/hero-progression/hero/{heroId}",
        cancellationToken);
  }

  public async Task<Guid> DefineAsync(
      DefineHeroProgressionCommand command,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync(
        "api/v1/hero-progression",
        command,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid?>(cancellationToken)
           ?? Guid.Empty;
  }
}