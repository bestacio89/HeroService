using System.Net.Http.Json;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginCulture;

namespace HeroService.Client.Http.Clients;

public sealed class OriginCultureClient : IOriginCultureClient
{
  private readonly HttpClient _httpClient;

  public OriginCultureClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<IReadOnlyList<OriginCultureDto>> GetAllAsync(
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<IReadOnlyList<OriginCultureDto>>(
               "api/v1/origin-cultures",
               cancellationToken)
           ?? [];
  }

  public async Task<OriginCultureDto?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<OriginCultureDto>(
        $"api/v1/origin-cultures/{id}",
        cancellationToken);
  }

  public async Task<OriginCultureDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<OriginCultureDto>(
        $"api/v1/origin-cultures/by-name/{Uri.EscapeDataString(name)}",
        cancellationToken);
  }

  public async Task<Guid> CreateAsync(
      CreateOriginCultureCommand command,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync(
        "api/v1/origin-cultures",
        command,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid?>(cancellationToken)
           ?? Guid.Empty;
  }

  public async Task RenameAsync(
      Guid id,
      RenameOriginCultureCommand command,
      CancellationToken cancellationToken = default)
  {
    var payload = command with { Id = id };

    var response = await _httpClient.PutAsJsonAsync(
        $"api/v1/origin-cultures/{id}",
        payload,
        cancellationToken);

    response.EnsureSuccessStatusCode();
  }

  public async Task RetireAsync(
      Guid id,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.DeleteAsync(
        $"api/v1/origin-cultures/{id}",
        cancellationToken);

    response.EnsureSuccessStatusCode();
  }
}