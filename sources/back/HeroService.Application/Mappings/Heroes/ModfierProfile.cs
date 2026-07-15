using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Application.Mappings.Modifiers;

public sealed class ModifierProfile : FranzMapProfile
{
  public ModifierProfile()
  {
    CreateMap<HeroModifier, HeroModifierDto>()
        .ConstructUsing(src => new HeroModifierDto(
            src.HeroId,
            src.GameVersionId,

            src.HealthMultiplier,
            src.ManaMultiplier,

            src.AttackDamageMultiplier,
            src.MagicDamageMultiplier,
            src.IgnoreEnemyDefenseAdjustment,

            src.AttackSpeedMultiplier,
            src.CastSpeedMultiplier,

            src.CritChanceMultiplier,
            src.CritDamageMultiplier,

            src.ArmorMultiplier,
            src.MagicResistanceMultiplier,
            src.DamageReductionMultiplier,

            src.ShieldStrengthMultiplier,

            src.MovementSpeedMultiplier,
            src.AttackRangeMultiplier,

            src.CooldownReductionMultiplier,
            src.ResourceRegenerationMultiplier
        ));


    CreateMap<SkillModifier, SkillModifierDto>()
        .ConstructUsing(src => new SkillModifierDto(
            src.SkillId,
            src.GameVersionId,

            src.CooldownMultiplier,
            src.ManaCostMultiplier,

            src.DamageMultiplier,
            src.HealingMultiplier,
            src.ShieldMultiplier,

            src.CastTimeMultiplier,
            src.ChannelDurationMultiplier,

            src.CrowdControlDurationMultiplier,

            src.RangeMultiplier
        ));
  }
}