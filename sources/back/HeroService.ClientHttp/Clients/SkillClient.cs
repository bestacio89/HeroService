using System.Net.Http.Json;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Commands.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Client.Http.Clients;

public sealed class SkillClient : ISkillClient
{
  private readonly HttpClient _httpClient;

  public SkillClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<Guid> CreateSkillAsync(
      CreateSkillCommand command,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync(
        "api/v1/skills",
        command,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid?>(cancellationToken)
           ?? Guid.Empty;
  }

  public async Task<Guid?> AddEffectAsync(
      Guid skillId,
      CreateSkillEffectCommand command,
      CancellationToken cancellationToken = default)
  {
    var payload = command with { SkillId = skillId };

    var response = await _httpClient.PostAsJsonAsync(
        $"api/v1/skills/{skillId}/effects",
        payload,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid?>(cancellationToken);
  }

  public async Task UpdateLoreAsync(
      Guid skillLoreId,
      UpdateSkillLoreCommand command,
      CancellationToken cancellationToken = default)
  {
    var payload = command with { SkillLoreId = skillLoreId };

    var response = await _httpClient.PutAsJsonAsync(
        $"api/v1/skills/{skillLoreId}/lore",
        payload,
        cancellationToken);

    response.EnsureSuccessStatusCode();
  }

  public async Task<IReadOnlyCollection<SkillDto>> GetAllAsync(
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<IReadOnlyCollection<SkillDto>>(
               "api/v1/skills",
               cancellationToken)
           ?? [];
  }

  public async Task<SkillDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<SkillDto>(
        $"api/v1/skills/by-name/{Uri.EscapeDataString(name)}",
        cancellationToken);
  }

  public async Task<SkillDto?> GetDetailsAsync(
      Guid skillId,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<SkillDto>(
        $"api/v1/skills/{skillId}",
        cancellationToken);
  }

  public async Task<SkillLoreDto?> GetLoreAsync(
      Guid skillId,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<SkillLoreDto>(
        $"api/v1/skills/{skillId}/lore",
        cancellationToken);
  }

  public async Task<SkillBaseStatsDto?> GetBaseStatsAsync(
      Guid skillId,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<SkillBaseStatsDto>(
        $"api/v1/skills/{skillId}/base-stats",
        cancellationToken);
  }

  public async Task<IReadOnlyCollection<SkillDto>> GetByTypeAsync(
      SkillType skillType,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<IReadOnlyCollection<SkillDto>>(
               $"api/v1/skills/by-type/{skillType}",
               cancellationToken)
           ?? [];
  }
}