using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Domain.Heroes.Versioned.Snapshotting;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

namespace HeroService.Application.Mappings.Snapshots;

public sealed class SnapshotProfile : FranzMapProfile
{
  public SnapshotProfile()
  {
    // =========================================================
    // SkillEffectSnapshot → DTO
    // =========================================================
    CreateMap<SkillEffectSnapshot, SkillEffectSnapshotDto>()
        .ConstructUsing(src => new SkillEffectSnapshotDto(
            src.HasDamage,
            src.HasDamageOverTime,
            src.HasHeal,
            src.HasHealOverTime,
            src.HasShield,
            src.HasCrowdControl,
            src.HasBuff,
            src.HasDebuff,
            src.HasMobility,
            src.HasExecute,
            src.HasUtility,
            src.HasVision,
            src.HasZoneControl,
            src.HasSummon,
            src.HasTransformation
        ));

    // =========================================================
    // SkillExecutionSnapshot → DTO
    // =========================================================
    CreateMap<SkillExecutionSnapshot, SkillExecutionSnapshotDto>()
        .ConstructUsing(src => new SkillExecutionSnapshotDto(
            src.Cooldown,
            src.ManaCost,
            src.Damage,
            src.Healing,
            src.ShieldValue,
            src.CastTime,
            src.ChannelDuration,
            src.Range,
            src.CrowdControlDuration
        ));

    // =========================================================
    // SkillSnapshot → DTO
    // =========================================================
    CreateMap<SkillSnapshot, SkillSnapshotDto>()
        .ConstructUsing(src => new SkillSnapshotDto(
            src.SkillId,
            new SkillExecutionSnapshotDto(
                src.Execution.Cooldown,
                src.Execution.ManaCost,
                src.Execution.Damage,
                src.Execution.Healing,
                src.Execution.ShieldValue,
                src.Execution.CastTime,
                src.Execution.ChannelDuration,
                src.Execution.Range,
                src.Execution.CrowdControlDuration
            ),
            new SkillEffectSnapshotDto(
                src.Effects.HasDamage,
                src.Effects.HasDamageOverTime,
                src.Effects.HasHeal,
                src.Effects.HasHealOverTime,
                src.Effects.HasShield,
                src.Effects.HasCrowdControl,
                src.Effects.HasBuff,
                src.Effects.HasDebuff,
                src.Effects.HasMobility,
                src.Effects.HasExecute,
                src.Effects.HasUtility,
                src.Effects.HasVision,
                src.Effects.HasZoneControl,
                src.Effects.HasSummon,
                src.Effects.HasTransformation
            )
        ));

    // =========================================================
    // HeroSkillKitSnapshot → DTO
    // Explicit construction — carries full SkillSnapshotDto objects.
    // Property names are aligned: Passive, Primary, Secondary,
    // Tertiary, Ultimate on both domain and DTO sides.
    // =========================================================
    CreateMap<HeroSkillKitSnapshot, HeroSkillKitSnapshotDto>()
        .ConstructUsing(src => new HeroSkillKitSnapshotDto(
            MapSkill(src.Passive),
            MapSkill(src.Primary),
            MapSkill(src.Secondary),
            MapSkill(src.Tertiary),
            MapSkill(src.Ultimate)
        ));

    // =========================================================
    // HeroStatSnapshot → DTO
    // TODO: ShieldStrengthMultiplier not yet in domain snapshot —
    //       passing 0f as placeholder until domain is aligned.
    // =========================================================
    CreateMap<HeroStatSnapshot, HeroStatSnapshotDto>()
        .ConstructUsing(src => new HeroStatSnapshotDto(
            src.Health,
            src.Mana,
            src.AttackDamage,
            src.AbilityPower,
            src.AttackSpeed,
            src.CastSpeed,
            src.CritChance,
            src.CritDamageMultiplier,
            src.Armor,
            src.MagicResistance,
            src.DamageReduction,
            0f, // TODO: ShieldStrengthMultiplier — align domain snapshot
            src.MovementSpeed,
            src.AttackRange,
            src.CooldownReduction,
            src.ResourceRegeneration
        ));

    // =========================================================
    // HeroKitProfile → DTO
    // =========================================================
    CreateMap<HeroKitProfile, HeroKitProfileDto>()
        .ConstructUsing(src => new HeroKitProfileDto(
            src.DamageSkillCount,
            src.CrowdControlSkillCount,
            src.MobilitySkillCount,
            src.SustainSkillCount,
            src.UtilitySkillCount,
            src.HasSummon,
            src.HasTransformation,
            src.HasExecute,
            src.IsBurstOriented,
            src.IsSustainOriented,
            src.IsControlOriented,
            src.IsMobilityOriented
        ));

    // =========================================================
    // HeroSnapshot → DTO
    // Franz ConstructUsing takes Func<TSource, TDestination> only —
    // no mapper context. All nested DTOs constructed inline.
    // =========================================================
    CreateMap<HeroSnapshot, HeroSnapshotDto>()
        .ConstructUsing(src => new HeroSnapshotDto(
            src.HeroId,
            src.GameVersionId,
            new HeroStatSnapshotDto(
                src.Stats.Health,
                src.Stats.Mana,
                src.Stats.AttackDamage,
                src.Stats.AbilityPower,
                src.Stats.AttackSpeed,
                src.Stats.CastSpeed,
                src.Stats.CritChance,
                src.Stats.CritDamageMultiplier,
                src.Stats.Armor,
                src.Stats.MagicResistance,
                src.Stats.DamageReduction,
                0f, // TODO: ShieldStrengthMultiplier — align domain snapshot
                src.Stats.MovementSpeed,
                src.Stats.AttackRange,
                src.Stats.CooldownReduction,
                src.Stats.ResourceRegeneration
            ),
            new HeroSkillKitSnapshotDto(
                MapSkill(src.SkillKit.Passive),
                MapSkill(src.SkillKit.Primary),
                MapSkill(src.SkillKit.Secondary),
                MapSkill(src.SkillKit.Tertiary),
                MapSkill(src.SkillKit.Ultimate)
            ),
            new HeroKitProfileDto(
                src.KitProfile.DamageSkillCount,
                src.KitProfile.CrowdControlSkillCount,
                src.KitProfile.MobilitySkillCount,
                src.KitProfile.SustainSkillCount,
                src.KitProfile.UtilitySkillCount,
                src.KitProfile.HasSummon,
                src.KitProfile.HasTransformation,
                src.KitProfile.HasExecute,
                src.KitProfile.IsBurstOriented,
                src.KitProfile.IsSustainOriented,
                src.KitProfile.IsControlOriented,
                src.KitProfile.IsMobilityOriented
            )
        ));
  }

  // =========================================================
  // HELPERS
  // Extracted to avoid repeating the full SkillSnapshot → DTO
  // construction in both CreateMap<HeroSkillKitSnapshot> and
  // CreateMap<HeroSnapshot> blocks.
  // =========================================================
  private static SkillSnapshotDto MapSkill(SkillSnapshot src) =>
      new(
          src.SkillId,
          new SkillExecutionSnapshotDto(
              src.Execution.Cooldown,
              src.Execution.ManaCost,
              src.Execution.Damage,
              src.Execution.Healing,
              src.Execution.ShieldValue,
              src.Execution.CastTime,
              src.Execution.ChannelDuration,
              src.Execution.Range,
              src.Execution.CrowdControlDuration
          ),
          new SkillEffectSnapshotDto(
              src.Effects.HasDamage,
              src.Effects.HasDamageOverTime,
              src.Effects.HasHeal,
              src.Effects.HasHealOverTime,
              src.Effects.HasShield,
              src.Effects.HasCrowdControl,
              src.Effects.HasBuff,
              src.Effects.HasDebuff,
              src.Effects.HasMobility,
              src.Effects.HasExecute,
              src.Effects.HasUtility,
              src.Effects.HasVision,
              src.Effects.HasZoneControl,
              src.Effects.HasSummon,
              src.Effects.HasTransformation
          )
      );
}