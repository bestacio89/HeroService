using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

namespace HeroService.Application.Mappings.Snapshots;

public sealed class HeroSnapshotMappingProfile : FranzMapProfile
{
  public HeroSnapshotMappingProfile()
  {
    CreateMap<HeroSnapshot, HeroSnapshotDto>()
        .ConstructUsing(src => new HeroSnapshotDto(
            src.HeroId,
            src.HeroName,
            src.GameVersionId,
            new HeroStatSnapshotDto(
                src.Stats.Health, src.Stats.Mana, src.Stats.AttackDamage, src.Stats.AbilityPower,
                src.Stats.AttackSpeed, src.Stats.CastSpeed, src.Stats.CritChance, src.Stats.CritDamageMultiplier,
                src.Stats.Armor, src.Stats.MagicResistance, src.Stats.DamageReduction,
                src.Stats.ShieldStrengthMultiplier, src.Stats.MovementSpeed, src.Stats.AttackRange,
                src.Stats.CooldownReduction, src.Stats.ResourceRegeneration
            ),
            new HeroSkillKitSnapshotDto(
                MapSkill(src.SkillKit.Passive),
                MapSkill(src.SkillKit.Primary),
                MapSkill(src.SkillKit.Secondary),
                MapSkill(src.SkillKit.Tertiary),
                MapSkill(src.SkillKit.Ultimate)
            ),
            new HeroKitProfileDto(
                src.KitProfile.DamageSkillCount, src.KitProfile.CrowdControlSkillCount,
                src.KitProfile.MobilitySkillCount, src.KitProfile.SustainSkillCount,
                src.KitProfile.UtilitySkillCount, src.KitProfile.HasSummon,
                src.KitProfile.HasTransformation, src.KitProfile.HasExecute,
                src.KitProfile.IsBurstOriented, src.KitProfile.IsSustainOriented,
                src.KitProfile.IsControlOriented, src.KitProfile.IsMobilityOriented
            )
        ));
  }

  private static SkillSnapshotDto MapSkill(SkillSnapshot src) => new(
    src.SkillId,
    src.Name,

    new SkillExecutionSnapshotDto(
        src.Execution.Cooldown ?? 0f,
        src.Execution.ManaCost ?? 0f,
        src.Execution.Damage ?? 0f,
        src.Execution.Healing ?? 0f,
        src.Execution.ShieldValue ?? 0f,
        src.Execution.CastTime ?? 0f,
        src.Execution.ChannelDuration ?? 0f,
        src.Execution.Range ?? 0f,
        src.Execution.CrowdControlDuration ?? 0f
    ),

    new SkillEffectSnapshotDto(
        src.Effects.HasDamage,
        src.Effects.HasDamageOverTime,
        src.Effects.HasHeal,
        src.Effects.HasHealOverTime,
        src.Effects.HasShield,

        src.Effects.HasSlow,
        src.Effects.HasRoot,
        src.Effects.HasStun,
        src.Effects.HasSilence,
        src.Effects.HasDisarm,
        src.Effects.HasBlind,

        src.Effects.HasFear,
        src.Effects.HasCharm,
        src.Effects.HasTaunt,
        src.Effects.HasConfuse,
        src.Effects.HasSleep,

        src.Effects.HasKnockback,
        src.Effects.HasKnockUp,
        src.Effects.HasPull,

        src.Effects.HasFreeze,
        src.Effects.HasPetrify,

        src.Effects.HasBuff,
        src.Effects.HasDebuff,

        src.Effects.HasMobility,
        src.Effects.HasExecute,

        src.Effects.HasUtility,
        src.Effects.HasVision,
        src.Effects.HasZoneControl,
        src.Effects.HasSummon,
        src.Effects.HasTransformation
    ),

    src.EffectExecutions.Select(e => new EffectExecutionSnapshotDto(
        e.EffectType,
        e.SourceSkillId,
        e.CasterId,
        e.TargetIds,

        e.FinalMagnitude,
        e.FinalDuration,
        e.FinalRadius,

        e.StacksApplied,

        e.IsInstant,
        e.IsPeriodic,
        e.IsChannelled,

        e.TickInterval,
        e.ChannelDuration,

        e.TargetType
    )).ToList()
);
}