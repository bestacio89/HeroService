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

      TextPrimary = "#E6E6E6",
      ActionDefault = "#D4AF37"
    },

    Typography = new Typography
    {
      H1 = new H1Typography
      {
        FontFamily = new[] { "Cinzel", "serif" },
        FontSize = "4.5rem",
        FontWeight = "300",
        LineHeight = "1.1",
        LetterSpacing = "-.02em"
      },

      H2 = new H2Typography
      {
        FontFamily = new[] { "Cinzel", "serif" },
        FontSize = "3.5rem",
        FontWeight = "300",
        LineHeight = "1.15",
        LetterSpacing = "-.015em"
      },

      H3 = new H3Typography
      {
        FontFamily = new[] { "Cinzel", "serif" },
        FontSize = "2.75rem",
        FontWeight = "400",
        LineHeight = "1.2",
        LetterSpacing = "-.01em"
      },

      H4 = new H4Typography
      {
        FontFamily = new[] { "Cinzel", "serif" },
        FontSize = "2rem",
        FontWeight = "400",
        LineHeight = "1.25",
        LetterSpacing = "-.005em"
      },

      H5 = new H5Typography
      {
        FontFamily = new[] { "Cinzel", "serif" },
        FontSize = "1.5rem",
        FontWeight = "500",
        LineHeight = "1.3",
        LetterSpacing = "0"
      },

      H6 = new H6Typography
      {
        FontFamily = new[] { "Cinzel", "serif" },
        FontSize = "1.25rem",
        FontWeight = "600",
        LineHeight = "1.4",
        LetterSpacing = ".005em"
      },

      Subtitle1 = new Subtitle1Typography
      {
        FontFamily = new[] { "Cinzel", "Arial", "sans-serif" },
        FontSize = "1rem",
        FontWeight = "500",
        LineHeight = "1.5",
        LetterSpacing = ".00938em"
      },

      Subtitle2 = new Subtitle2Typography
      {
        FontFamily = new[] { "Cinzel", "Arial", "sans-serif" },
        FontSize = ".875rem",
        FontWeight = "500",
        LineHeight = "1.57",
        LetterSpacing = ".00714em"
      },

      Body1 = new Body1Typography
      {
        FontFamily = new[] { "Cinzel", "Helvetica", "Arial", "sans-serif" },
        FontSize = "1rem",
        FontWeight = "400",
        LineHeight = "1.5",
        LetterSpacing = ".00938em"
      },

      Body2 = new Body2Typography
      {
        FontFamily = new[] { "Cinzel", "Helvetica", "Arial", "sans-serif" },
        FontSize = ".875rem",
        FontWeight = "400",
        LineHeight = "1.43",
        LetterSpacing = ".01071em"
      },

      Button = new ButtonTypography
      {
        FontFamily = new[] { "Cinzel", "sans-serif" },
        FontSize = ".875rem",
        FontWeight = "600",
        LineHeight = "1.75",
        LetterSpacing = ".02857em",
        TextTransform = "uppercase"
      },

      Caption = new CaptionTypography
      {
        FontFamily = new[] { "Cinzel", "sans-serif" },
        FontSize = ".75rem",
        FontWeight = "400",
        LineHeight = "1.66",
        LetterSpacing = ".03333em"
      },

      Overline = new OverlineTypography
      {
        FontFamily = new[] { "Cinzel", "sans-serif" },
        FontSize = ".75rem",
        FontWeight = "400",
        LineHeight = "2.66",
        LetterSpacing = ".08333em",
        TextTransform = "uppercase"
      },

      Default = new DefaultTypography
      {
        FontFamily = new[] { "Cinzel", "Helvetica", "Arial", "sans-serif" },
        FontSize = ".875rem",
        FontWeight = "400",
        LineHeight = "1.43",
        LetterSpacing = ".01071em"
      }
    },

    LayoutProperties = new LayoutProperties
    {
      DefaultBorderRadius = "4px"
    }
  };
}