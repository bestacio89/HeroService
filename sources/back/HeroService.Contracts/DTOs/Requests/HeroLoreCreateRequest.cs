namespace HeroService.Contracts.DTOs.Requests;

public sealed class HeroLoreCreateRequest
{
  public string Title { get; init; } = string.Empty;

  public string Description { get; init; } = string.Empty;

  public string BackgroundStory { get; init; } = string.Empty;
}