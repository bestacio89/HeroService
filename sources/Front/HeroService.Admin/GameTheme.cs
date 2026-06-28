using MudBlazor;

namespace HeroService.Admin.UI;

public static class GameTheme
{
  public static MudTheme Create() => new MudTheme()
  {
    PaletteDark = new PaletteDark()
    {
      Primary = "#D4AF37",
      Secondary = "#7B3F00",
      Background = "#0B0F14",
      Surface = "#121826",
      AppbarBackground = "#0B0F14",
      TextPrimary = "#E6E6E6"
    }
  };
}