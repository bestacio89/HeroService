namespace HeroService.Admin.Components.Pages.Admin.Heroes;

/// <summary>
/// Resolves mythology reference-data names to secondary Admin visual palettes.
/// Mythology remains distinct from origin culture and never overrides hero-class identity.
/// </summary>
public static class MythologyTheme
{
  private static readonly HeroVisualPalette NeutralPalette =
      new("#A8B0BD", "rgba(168, 176, 189, 0.06)", "rgba(168, 176, 189, 0.26)");

  public static HeroVisualPalette GetPalette(string? mythologyName) =>
      HeroClassTheme.Normalize(mythologyName) switch
      {
        "greek" => new HeroVisualPalette(
            "#D6B46A", "rgba(214, 180, 106, 0.06)", "rgba(214, 180, 106, 0.32)"),
        "norse" => new HeroVisualPalette(
            "#8CC8E8", "rgba(140, 200, 232, 0.06)", "rgba(140, 200, 232, 0.32)"),
        "egyptian" => new HeroVisualPalette(
            "#E58B62", "rgba(229, 139, 98, 0.06)", "rgba(229, 139, 98, 0.32)"),
        "japanese" => new HeroVisualPalette(
            "#F07878", "rgba(240, 120, 120, 0.06)", "rgba(240, 120, 120, 0.32)"),
        "celtic" => new HeroVisualPalette(
            "#79B77A", "rgba(121, 183, 122, 0.06)", "rgba(121, 183, 122, 0.32)"),
        "mesopotamian" => new HeroVisualPalette(
            "#C7A77B", "rgba(199, 167, 123, 0.06)", "rgba(199, 167, 123, 0.32)"),
        "hindu" => new HeroVisualPalette(
            "#E7AE4F", "rgba(231, 174, 79, 0.06)", "rgba(231, 174, 79, 0.32)"),
        "chinese" => new HeroVisualPalette(
            "#69C69B", "rgba(105, 198, 155, 0.06)", "rgba(105, 198, 155, 0.32)"),
        "yoruba" => new HeroVisualPalette(
            "#A99BE8", "rgba(169, 155, 232, 0.06)", "rgba(169, 155, 232, 0.32)"),
        "maya" => new HeroVisualPalette(
            "#5CC7C3", "rgba(92, 199, 195, 0.06)", "rgba(92, 199, 195, 0.32)"),
        _ => NeutralPalette
      };

  public static string GetBadgeStyle(string? mythologyName) =>
      GetPalette(mythologyName).ToCssVariables("mythology");
}
