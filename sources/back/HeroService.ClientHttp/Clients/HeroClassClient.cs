using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Heroes;
using HeroService.Contracts.DTOs.Heroes;
using System.Net.Http.Json;

namespace HeroService.Client.Http.Clients;

public sealed class HeroClassClient : IHeroClassClient
{
  private readonly HttpClient _httpClient;

  public HeroClassClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  // -----------------------------
  // CREATE
  // -----------------------------
  public async Task<Guid> CreateAsync(
      CreateHeroClassCommand command,
      CancellationToken ct = default)
  {
    var response = await _httpClient.PostAsJsonAsync(
        "api/v1/hero-classes",
        command,
        ct);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid>(ct);
  }

  // -----------------------------
  // GET ALL
  // -----------------------------
  public async Task<IReadOnlyCollection<HeroClassDto>> GetAllAsync(
      CancellationToken ct = default)
  {
    return await _httpClient.GetFromJsonAsync<IReadOnlyCollection<HeroClassDto>>(
               "api/v1/hero-classes",
               ct)
           ?? [];
  }

  // -----------------------------
  // GET BY ID
  // -----------------------------
  public async Task<HeroClassDto?> GetByIdAsync(
      Guid id,
      CancellationToken ct = default)
  {
    return await _httpClient.GetFromJsonAsync<HeroClassDto>(
        $"api/v1/hero-classes/{id}",
        ct);
  }

  // -----------------------------
  // GET BY NAME
  // -----------------------------
  public async Task<HeroClassDto?> GetByNameAsync(
      string name,
      CancellationToken ct = default)
  {
    return await _httpClient.GetFromJsonAsync<HeroClassDto>(
        $"api/v1/hero-classes/by-name/{Uri.EscapeDataString(name)}",
        ct);
  }
}