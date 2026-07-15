using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Modifiers;
using HeroService.Contracts.DTOs.Modifiers;
using System.Net.Http.Json;

namespace HeroService.Client.Http.Clients;

public sealed class SkillModifierClient : ISkillModifierClient
{
  private readonly HttpClient _httpClient;


  public SkillModifierClient(
      HttpClient httpClient)
  {
    _httpClient = httpClient;
  }


  public async Task<Guid> CreateAsync(
      CreateSkillModifierCommand command,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync(
        "api/v1/skill-modifiers",
        command,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid?>(
               cancellationToken)
           ?? Guid.Empty;
  }


  public async Task UpdateAsync(
      Guid id,
      UpdateSkillModifierCommand command,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PutAsJsonAsync(
        $"api/v1/skill-modifiers/{id}",
        command,
        cancellationToken);

    response.EnsureSuccessStatusCode();
  }


  public async Task<SkillModifierDto?> GetBySkillAsync(
      Guid skillId,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<SkillModifierDto>(
        $"api/v1/skill-modifiers/skill/{skillId}",
        cancellationToken);
  }


  public async Task<IReadOnlyList<SkillModifierDto>> GetActiveAsync(
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<IReadOnlyList<SkillModifierDto>>(
               "api/v1/skill-modifiers",
               cancellationToken)
           ?? Array.Empty<SkillModifierDto>();
  }
}