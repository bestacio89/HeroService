using HeroService.Domain.Heroes.Skills;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Skills;

/// <summary>
/// Provides stable, high-contrast presentation colors for skill categories.
/// These values are UI metadata; the domain enum remains the source of truth.
/// </summary>
public static class SkillTheme
{
  // Retained for existing MudBlazor call sites; the skill library uses the
  // high-contrast palette below to distinguish every category.
  public static Color GetColor(SkillType type) => type switch
  {
    SkillType.Damage => Color.Error,
    SkillType.Heal => Color.Success,
    SkillType.Shield => Color.Info,
    SkillType.Mobility => Color.Secondary,
    SkillType.Control => Color.Warning,
    SkillType.Buff => Color.Primary,
    SkillType.Debuff => Color.Tertiary,
    SkillType.Ultimate => Color.Error,
    _ => Color.Default
  };

  public static string GetAccent(SkillType type) => type switch
  {
    SkillType.Damage => "#FF6B6B",
    SkillType.Heal => "#4ADE80",
    SkillType.Shield => "#38BDF8",
    SkillType.Mobility => "#C084FC",
    SkillType.Control => "#FBBF24",
    SkillType.Buff => "#2DD4BF",
    SkillType.Debuff => "#FB923C",
    SkillType.Ultimate => "#F472D0",
    _ => "#A1A1AA"
  };

  public static string GetTint(SkillType type) => type switch
  {
    SkillType.Damage => "rgba(255, 107, 107, 0.13)",
    SkillType.Heal => "rgba(74, 222, 128, 0.13)",
    SkillType.Shield => "rgba(56, 189, 248, 0.13)",
    SkillType.Mobility => "rgba(192, 132, 252, 0.13)",
    SkillType.Control => "rgba(251, 191, 36, 0.13)",
    SkillType.Buff => "rgba(45, 212, 191, 0.13)",
    SkillType.Debuff => "rgba(251, 146, 60, 0.13)",
    SkillType.Ultimate => "rgba(244, 114, 208, 0.13)",
    _ => "rgba(161, 161, 170, 0.13)"
  };

  public static string GetBadgeStyle(SkillType type) =>
      $"color: {GetAccent(type)}; background-color: {GetTint(type)}; border: 1px solid {GetAccent(type)}; font-weight: 600;";

  public static string GetCardStyle(SkillType type) =>
      $"height: 100%; display: flex; flex-direction: column; background: var(--app-surface-raised, #171F2D); border: 1px solid var(--app-border, rgba(212,175,55,.16)); border-top: 3px solid {GetAccent(type)}; border-radius: 14px;";

  public static string GetMarkStyle(SkillType type) =>
      $"width: 3rem; height: 3rem; display: grid; place-items: center; border-radius: 12px; border: 1px solid {GetAccent(type)}; background-color: {GetTint(type)}; color: {GetAccent(type)};";

  public static string GetDescription(SkillType type) => type switch
  {
    SkillType.Damage => "Burst or sustained damage",
    SkillType.Heal => "Restore health",
    SkillType.Shield => "Protect or mitigate damage",
    SkillType.Mobility => "Dash, blink or reposition",
    SkillType.Control => "Stun, root, slow or disrupt",
    SkillType.Buff => "Enhance an ally or self",
    SkillType.Debuff => "Weaken or impair an enemy",
    SkillType.Ultimate => "High-impact signature ability",
    _ => "Unclassified ability"
  };
}
