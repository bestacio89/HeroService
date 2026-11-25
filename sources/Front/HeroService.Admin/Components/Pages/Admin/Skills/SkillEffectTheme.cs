using HeroService.Domain.Heroes.Skills;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Skills;

/// <summary>
/// Maps gameplay effect semantics to stable Admin-only Material icons.
/// EffectType remains the authoritative domain classification.
/// </summary>
public static class SkillEffectTheme
{
  public static string GetIcon(EffectType type) => type switch
  {
    EffectType.Damage => Icons.Material.Filled.Whatshot,
    EffectType.DamageOverTime => Icons.Material.Filled.LocalFireDepartment,
    EffectType.Heal => Icons.Material.Filled.Healing,
    EffectType.HealOverTime => Icons.Material.Filled.MedicalServices,
    EffectType.Shield => Icons.Material.Filled.Shield,
    EffectType.Slow => Icons.Material.Filled.AvTimer,
    EffectType.Root => Icons.Material.Filled.Park,
    EffectType.Stun => Icons.Material.Filled.FlashOn,
    EffectType.Silence => Icons.Material.Filled.VolumeOff,
    EffectType.Disarm => Icons.Material.Filled.BackHand,
    EffectType.Blind => Icons.Material.Filled.VisibilityOff,
    EffectType.Fear => Icons.Material.Filled.SentimentVeryDissatisfied,
    EffectType.Charm => Icons.Material.Filled.Favorite,
    EffectType.Taunt => Icons.Material.Filled.Campaign,
    EffectType.Confuse => Icons.Material.Filled.Psychology,
    EffectType.Sleep => Icons.Material.Filled.Bedtime,
    EffectType.Knockback => Icons.Material.Filled.OpenWith,
    EffectType.KnockUp => Icons.Material.Filled.North,
    EffectType.Pull => Icons.Material.Filled.South,
    EffectType.Freeze => Icons.Material.Filled.AcUnit,
    EffectType.Petrify => Icons.Material.Filled.Terrain,
    EffectType.Buff => Icons.Material.Filled.TrendingUp,
    EffectType.Debuff => Icons.Material.Filled.TrendingDown,
    EffectType.Mobility => Icons.Material.Filled.DirectionsRun,
    EffectType.Execute => Icons.Material.Filled.Gavel,
    EffectType.Utility => Icons.Material.Filled.Build,
    EffectType.Vision => Icons.Material.Filled.Visibility,
    EffectType.ZoneControl => Icons.Material.Filled.Radar,
    EffectType.Summon => Icons.Material.Filled.Pets,
    EffectType.Transformation => Icons.Material.Filled.AutoFixHigh,
    _ => Icons.Material.Filled.HelpOutline
  };
}
