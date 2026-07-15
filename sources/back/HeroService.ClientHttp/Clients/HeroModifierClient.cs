using System.Net.Http.Json;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Modifiers;
using HeroService.Contracts.DTOs.Modifiers;

namespace HeroService.Client.Http.Clients;

public sealed class HeroModifierClient : IHeroModifierClient
{
    private readonly HttpClient _httpClient;


    public HeroModifierClient(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<Guid> CreateAsync(
        CreateHeroModifierCommand command,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/hero-modifiers",
            command,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid?>(
                   cancellationToken)
               ?? Guid.Empty;
    }


    public async Task UpdateAsync(
        Guid id,
        UpdateHeroModifierCommand command,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/v1/hero-modifiers/{id}",
            command,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }


    public async Task<HeroModifierDto?> GetByHeroAsync(
        Guid heroId,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<HeroModifierDto>(
            $"api/v1/hero-modifiers/hero/{heroId}",
            cancellationToken);
    }


    public async Task<IReadOnlyList<HeroModifierDto>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<IReadOnlyList<HeroModifierDto>>(
                   "api/v1/hero-modifiers",
                   cancellationToken)
               ?? Array.Empty<HeroModifierDto>();
    }
}