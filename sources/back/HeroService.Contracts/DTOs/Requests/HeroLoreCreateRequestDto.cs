namespace HeroService.Contracts.DTOs.Requests;

public sealed class HeroLoreCreateRequestDto
{
  public string Title { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;

  public string BackgroundStory { get; set; } = string.Empty;
}