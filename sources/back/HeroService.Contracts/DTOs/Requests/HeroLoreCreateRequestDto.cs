namespace HeroService.Contracts.DTOs.Requests;

public sealed class HeroLoreCreateRequestDto
{
  public string Title { get; init; } = string.Empty;

  public string Description { get; init; } = string.Empty;

  public string BackgroundStory { get; init; } = string.Empty;
}