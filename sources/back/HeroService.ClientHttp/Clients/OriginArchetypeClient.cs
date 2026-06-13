using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Heroes.Classifications.NewFolder;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginArchetype;
using HeroService.Contracts.DTOs.Heroes;
using System;
using System.Net.Http.Json;

namespace HeroService.Client.Http.Clients;

public sealed class OriginArchetypeClient : IOriginArchetypeClient
{
  private readonly HttpClient _httpClient;

  public OriginArchetypeClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<IReadOnlyList<OriginArchetypeDto>> GetAllAsync(
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<IReadOnlyList<OriginArchetypeDto>>(
               "api/v1/origin-archetypes",
               cancellationToken)
           ?? [];
  }

  public async Task<OriginArchetypeDto?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<OriginArchetypeDto>(
        $"api/v1/origin-archetypes/{id}",
        cancellationToken);
  }

  public async Task<OriginArchetypeDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<OriginArchetypeDto>(
        $"api/v1/origin-archetypes/by-name/{Uri.EscapeDataString(name)}",
        cancellationToken);
  }

  public async Task<Guid> CreateAsync(
      CreateOriginArchetypeCommand command,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync(
        "api/v1/origin-archetypes",
        command,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid?>(cancellationToken)
           ?? Guid.Empty;
  }

  public async Task RenameAsync(
      Guid id,
      RenameOriginArchetypeCommand command,
      CancellationToken cancellationToken = default)
  {
    var payload = command with { Id = id };

    var response = await _httpClient.PutAsJsonAsync(
        $"api/v1/origin-archetypes/{id}",
        payload,
        cancellationToken);

    response.EnsureSuccessStatusCode();
  }

  public async Task RetireAsync(
      Guid id,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.DeleteAsync(
        $"api/v1/origin-archetypes/{id}",
        cancellationToken);

    response.EnsureSuccessStatusCode();
  }
}