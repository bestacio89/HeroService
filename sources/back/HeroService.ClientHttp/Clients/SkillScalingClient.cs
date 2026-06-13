using System.Net.Http.Json;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Commands.Skills;

namespace HeroService.Client.Http.Clients;

public sealed class SkillScalingClient : ISkillScalingClient
{
  private readonly HttpClient _httpClient;

  public SkillScalingClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<SkillScalingModifierDto?> GetBySkillAsync(
      Guid skillId,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<SkillScalingModifierDto>(
        $"api/v1/skill-scaling/skill/{skillId}",
        cancellationToken);
  }

  public async Task<Guid> DefineAsync(
      DefineSkillScalingCommand command,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync(
        "api/v1/skill-scaling",
        command,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid?>(cancellationToken)
           ?? Guid.Empty;
  }
}