using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

/// <summary>
/// Resolves hero class reference data to semantic Admin icons and primary visual palettes.
/// Unknown values intentionally use a neutral fallback.
/// </summary>
public static class HeroClassTheme
{
  private static readonly HeroVisualPalette NeutralPalette =
      new("#A8B0BD", "rgba(168, 176, 189, 0.08)", "rgba(168, 176, 189, 0.30)");

  public static string GetIcon(string? className)
  {
    var normalized = Normalize(className);

    return normalized switch
    {
      "warrior" or "fighter" or "bruiser" => Icons.Material.Filled.SportsMartialArts,
      "assassin" or "rogue" => Icons.Material.Filled.VisibilityOff,
      "mage" or "wizard" or "sorcerer" => Icons.Material.Filled.AutoFixHigh,
      "tank" or "guardian" or "defender" => Icons.Material.Filled.Shield,
      "support" or "healer" => Icons.Material.Filled.VolunteerActivism,
      "ranger" or "marksman" or "archer" => Icons.Material.Filled.GpsFixed,
      _ => Icons.Material.Filled.PersonOutline
    };
  }

  public static HeroVisualPalette GetPalette(string? className) =>
      Normalize(className) switch
      {
        "assassin" => new HeroVisualPalette(
            "#F07B86", "rgba(240, 123, 134, 0.10)", "rgba(240, 123, 134, 0.38)"),
        "mage" => new HeroVisualPalette(
            "#C4A5FF", "rgba(196, 165, 255, 0.10)", "rgba(196, 165, 255, 0.38)"),
        "ranger" => new HeroVisualPalette(
            "#69D6A3", "rgba(105, 214, 163, 0.10)", "rgba(105, 214, 163, 0.38)"),
        "support" => new HeroVisualPalette(
            "#66D9D0", "rgba(102, 217, 208, 0.10)", "rgba(102, 217, 208, 0.38)"),
        "tank" => new HeroVisualPalette(
            "#8CBCE8", "rgba(140, 188, 232, 0.10)", "rgba(140, 188, 232, 0.38)"),
        "warrior" => new HeroVisualPalette(
            "#F0C36A", "rgba(240, 195, 106, 0.10)", "rgba(240, 195, 106, 0.38)"),
        _ => NeutralPalette
      };

  public static string GetCardStyle(string? className) =>
      GetPalette(className).ToCssVariables("hero");

  public static string GetBadgeStyle(string? className) =>
      GetPalette(className).ToCssVariables("hero");

  internal static string Normalize(string? value) =>
      string.IsNullOrWhiteSpace(value)
          ? string.Empty
          : string.Join(' ', value.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
              .ToLowerInvariant();
}
