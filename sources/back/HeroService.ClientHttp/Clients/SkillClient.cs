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

  // =========================================================
  // CREATE SKILL
  // =========================================================
  public async Task<Guid> CreateSkillAsync(
      CreateSkillCommand command,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync(
        "api/v1/skills",
        command,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid>(cancellationToken);
  }

  // =========================================================
  // ADD EFFECT
  // =========================================================
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

  // =========================================================
  // LORE (FIXED BOUNDARY)
  // =========================================================
  public async Task UpdateLoreAsync(
      Guid skillId,
      UpdateSkillLoreCommand command,
      CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PutAsJsonAsync(
        $"api/v1/skills/{skillId}/lore",
        command,
        cancellationToken);

    response.EnsureSuccessStatusCode();
  }

  // =========================================================
  // GET ALL
  // =========================================================
  public async Task<IReadOnlyCollection<SkillDto>> GetAllAsync(
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<IReadOnlyCollection<SkillDto>>(
               "api/v1/skills",
               cancellationToken)
           ?? [];
  }

  // =========================================================
  // GET BY NAME
  // =========================================================
  public async Task<SkillDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<SkillDto>(
        $"api/v1/skills/by-name/{Uri.EscapeDataString(name)}",
        cancellationToken);
  }

  // =========================================================
  // GET DETAILS
  // =========================================================
  public async Task<SkillDto?> GetDetailsAsync(
      Guid skillId,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<SkillDto>(
        $"api/v1/skills/{skillId}",
        cancellationToken);
  }

  // =========================================================
  // GET LORE (SCOPED TO SKILL)
  // =========================================================
  public async Task<SkillLoreDto?> GetLoreAsync(
      Guid skillId,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<SkillLoreDto>(
        $"api/v1/skills/{skillId}/lore",
        cancellationToken);
  }

  // =========================================================
  // BASE STATS
  // =========================================================
  public async Task<SkillBaseStatsDto?> GetBaseStatsAsync(
      Guid skillId,
      CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<SkillBaseStatsDto>(
        $"api/v1/skills/{skillId}/base-stats",
        cancellationToken);
  }

  // =========================================================
  // FILTER BY TYPE
  // =========================================================
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