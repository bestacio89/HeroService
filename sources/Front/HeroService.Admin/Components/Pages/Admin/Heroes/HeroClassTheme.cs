using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

/// <summary>
/// Resolves seeded hero class names to semantic Admin icons.
/// Unknown reference-data values intentionally use a neutral fallback.
/// </summary>
public static class HeroClassTheme
{
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

  private static string Normalize(string? value) =>
    string.IsNullOrWhiteSpace(value)
      ? string.Empty
      : string.Join(' ', value.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
          .ToLowerInvariant();
}
