namespace HeroService.Admin.Components.Pages.Admin.Heroes;

/// <summary>
/// Immutable presentation colors for one hero classification identity.
/// Values are fixed Admin theme tokens, never user-provided CSS.
/// </summary>
public sealed record HeroVisualPalette(
    string Accent,
    string Surface,
    string Border)
{
  public string ToCssVariables(string prefix) =>
      $"--{prefix}-accent: {Accent}; --{prefix}-surface: {Surface}; --{prefix}-border: {Border};";
}
