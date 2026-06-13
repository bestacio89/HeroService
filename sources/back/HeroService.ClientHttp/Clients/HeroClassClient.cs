using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using System.Net.Http.Json;

public sealed class HeroClassClient : IHeroClassClient
{
  private readonly HttpClient _httpClient;

  public HeroClassClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<HeroClassDto?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<HeroClassDto>(
        $"api/v1/hero-classes/{id}",
        cancellationToken);
  }

  public async Task<HeroClassDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<HeroClassDto>(
        $"api/v1/hero-classes/by-name/{name}",
        cancellationToken);
  }

  public async Task<IReadOnlyCollection<HeroClassDto>> GetAllAsync(
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<IReadOnlyCollection<HeroClassDto>>(
               "api/v1/hero-classes",
               cancellationToken)
           ?? [];
  }
}