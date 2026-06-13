using System.Net.Http.Json;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Commands.Heroes.Classifications.MythologyTypes;

namespace HeroService.Client.Http.Clients;

public sealed class MythologyTypeClient : IMythologyTypeClient
{
  private readonly HttpClient _httpClient;

  public MythologyTypeClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<Guid> CreateAsync(
      CreateMythologyTypeCommand command,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync(
        "api/v1/mythology-types",
        command,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid?>(cancellationToken)
           ?? Guid.Empty;
  }

  public async Task RenameAsync(
      Guid id,
      RenameMythologyTypeCommand command,
      CancellationToken cancellationToken = default)
  {
    var payload = command with { Id = id };

    var response = await _httpClient.PutAsJsonAsync(
        $"api/v1/mythology-types/{id}",
        payload,
        cancellationToken);

    response.EnsureSuccessStatusCode();
  }

  public async Task RetireAsync(
      Guid id,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.DeleteAsync(
        $"api/v1/mythology-types/{id}",
        cancellationToken);

    response.EnsureSuccessStatusCode();
  }

  public async Task<IReadOnlyList<MythologyTypeDto>> GetAllAsync(
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<IReadOnlyList<MythologyTypeDto>>(
               "api/v1/mythology-types",
               cancellationToken)
           ?? [];
  }

  public async Task<MythologyTypeDto?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<MythologyTypeDto>(
        $"api/v1/mythology-types/{id}",
        cancellationToken);
  }

  public async Task<MythologyTypeDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<MythologyTypeDto>(
        $"api/v1/mythology-types/by-name/{Uri.EscapeDataString(name)}",
        cancellationToken);
  }
}