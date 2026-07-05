using HeroService.Domain.Heroes.Skills;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Skills;

public static class SkillTheme
{
  public static Color GetColor(SkillType type) => type switch
  {
    SkillType.Damage => Color.Error,
    SkillType.Heal => Color.Success,
    SkillType.Shield => Color.Info,
    SkillType.Mobility => Color.Secondary,
    SkillType.Control => Color.Warning,
    SkillType.Buff => Color.Primary,
    SkillType.Debuff => Color.Dark,
    SkillType.Ultimate => Color.Error,
    _ => Color.Default
  };
}