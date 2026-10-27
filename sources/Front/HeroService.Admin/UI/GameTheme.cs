using MudBlazor;

namespace HeroService.Admin.UI;

public static class GameTheme
{
  public static MudTheme Create() => new()
  {
    PaletteDark = new PaletteDark
    {
      Primary = "#D4AF37",
      Secondary = "#7B3F00",
      Background = "#0B0F14",
      Surface = "#121826",
      AppbarBackground = "#0B0F14",
      DrawerBackground = "#0F141D",
      TextPrimary = "#E6E6E6",
      TextSecondary = "#A8B0BD",
      ActionDefault = "#D4AF37",
      Divider = "rgba(212, 175, 55, 0.16)",
      LinesDefault = "rgba(212, 175, 55, 0.12)"
    },
    Typography = new Typography
    {
      H1 = new H1Typography
      {
        FontFamily = new[] { "Cinzel", "Georgia", "serif" },
        FontSize = "2.5rem",
        FontWeight = "600",
        LineHeight = "1.15",
        LetterSpacing = "-.02em"
      },
      H2 = new H2Typography
      {
        FontFamily = new[] { "Cinzel", "Georgia", "serif" },
        FontSize = "2rem",
        FontWeight = "600",
        LineHeight = "1.2"
      },
      H3 = new H3Typography
      {
        FontFamily = new[] { "Cinzel", "Georgia", "serif" },
        FontSize = "1.625rem",
        FontWeight = "600",
        LineHeight = "1.25"
      },
      H4 = new H4Typography
      {
        FontFamily = new[] { "Cinzel", "Georgia", "serif" },
        FontSize = "1.375rem",
        FontWeight = "600",
        LineHeight = "1.3"
      },
      H5 = new H5Typography
      {
        FontFamily = new[] { "Cinzel", "Georgia", "serif" },
        FontSize = "1.125rem",
        FontWeight = "600",
        LineHeight = "1.35"
      },
      H6 = new H6Typography
      {
        FontFamily = new[] { "Cinzel", "Georgia", "serif" },
        FontSize = "1rem",
        FontWeight = "600",
        LineHeight = "1.4"
      },
      Subtitle1 = new Subtitle1Typography
      {
        FontFamily = new[] { "Inter", "Segoe UI", "Arial", "sans-serif" },
        FontSize = "1rem",
        FontWeight = "600",
        LineHeight = "1.5"
      },
      Subtitle2 = new Subtitle2Typography
      {
        FontFamily = new[] { "Inter", "Segoe UI", "Arial", "sans-serif" },
        FontSize = ".875rem",
        FontWeight = "600",
        LineHeight = "1.45"
      },
      Body1 = new Body1Typography
      {
        FontFamily = new[] { "Inter", "Segoe UI", "Arial", "sans-serif" },
        FontSize = ".9375rem",
        FontWeight = "400",
        LineHeight = "1.55"
      },
      Body2 = new Body2Typography
      {
        FontFamily = new[] { "Inter", "Segoe UI", "Arial", "sans-serif" },
        FontSize = ".875rem",
        FontWeight = "400",
        LineHeight = "1.5"
      },
      Button = new ButtonTypography
      {
        FontFamily = new[] { "Inter", "Segoe UI", "Arial", "sans-serif" },
        FontSize = ".8125rem",
        FontWeight = "600",
        LineHeight = "1.4",
        LetterSpacing = ".02em",
        TextTransform = "none"
      },
      Caption = new CaptionTypography
      {
        FontFamily = new[] { "Inter", "Segoe UI", "Arial", "sans-serif" },
        FontSize = ".75rem",
        FontWeight = "400",
        LineHeight = "1.45",
        LetterSpacing = ".01em"
      },
      Overline = new OverlineTypography
      {
        FontFamily = new[] { "Inter", "Segoe UI", "Arial", "sans-serif" },
        FontSize = ".6875rem",
        FontWeight = "600",
        LineHeight = "1.5",
        LetterSpacing = ".12em",
        TextTransform = "uppercase"
      },
      Default = new DefaultTypography
      {
        FontFamily = new[] { "Inter", "Segoe UI", "Arial", "sans-serif" },
        FontSize = ".875rem",
        FontWeight = "400",
        LineHeight = "1.5",
        LetterSpacing = "normal"
      }
    },
    LayoutProperties = new LayoutProperties
    {
      DefaultBorderRadius = "10px"
    }
  };
}