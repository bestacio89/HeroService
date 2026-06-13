using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.DTOs.Requests;
using System.Net.Http.Json;

namespace HeroService.Client.Http.Clients;

public sealed class HeroClient : IHeroClient
{
  private readonly HttpClient _httpClient;

  public HeroClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<HeroDto?> GetByIdAsync(
      Guid heroId,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<HeroDto>(
        $"api/v1/heroes/{heroId}",
        cancellationToken);
  }

  public async Task<HeroDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<HeroDto>(
        $"api/v1/heroes/by-name/{Uri.EscapeDataString(name)}",
        cancellationToken);
  }

  public async Task<Guid> CreateAsync(
      HeroCreateRequestDto request,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync(
        "api/v1/heroes",
        request,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid?>(cancellationToken)
           ?? Guid.Empty;
  }

  public async Task<IReadOnlyCollection<HeroDto>> BrowseAsync(
      Guid? heroClassId = null,
      Guid? mythologyTypeId = null,
      Guid? cultureId = null,
      Guid? archetypeId = null,
      CancellationToken cancellationToken = default)
  {
    var query = new List<string>();

    if (heroClassId.HasValue)
      query.Add($"heroClassId={heroClassId}");

    if (mythologyTypeId.HasValue)
      query.Add($"mythologyTypeId={mythologyTypeId}");

    if (cultureId.HasValue)
      query.Add($"cultureId={cultureId}");

    if (archetypeId.HasValue)
      query.Add($"archetypeId={archetypeId}");

    var uri = "api/v1/heroes";

    if (query.Count > 0)
      uri += "?" + string.Join("&", query);

    return await _httpClient.GetFromJsonAsync<IReadOnlyCollection<HeroDto>>(
               uri,
               cancellationToken)
           ?? [];
  }
}