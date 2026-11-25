using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

/// <summary>
/// Resolves known hero-class names to stable presentation icons.
/// Class identity remains reference data owned by HeroService; unknown names
/// deliberately receive a neutral icon instead of an inferred role.
/// </summary>
public static class HeroClassTheme
{
  public static string GetIcon(string? className)
  {
    if (string.IsNullOrWhiteSpace(className))
      return Icons.Material.Filled.HelpOutline;

    return className.Trim().ToUpperInvariant() switch
    {
      "ASSASSIN" or "SLAYER" => Icons.Material.Filled.VisibilityOff,
      "MAGE" or "SORCERER" => Icons.Material.Filled.AutoAwesome,
      "RANGER" or "MARKSMAN" => Icons.Material.Filled.GpsFixed,
      "SUPPORT" or "ENCHANTER" => Icons.Material.Filled.Favorite,
      "TANK" or "GUARDIAN" => Icons.Material.Filled.Shield,
      "WARRIOR" or "BRUISER" or "FIGHTER" => Icons.Material.Filled.FitnessCenter,
      _ => Icons.Material.Filled.HelpOutline
    };
  }
}
