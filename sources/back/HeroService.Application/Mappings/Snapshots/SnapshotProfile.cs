using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

namespace HeroService.Application.Mappings.Snapshots;

public sealed class SnapshotProfile : FranzMapProfile
{
    public SnapshotProfile()
    {
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
                    src.Effects.HasHealing,
                    src.Effects.HasShield,
                    src.Effects.HasCrowdControl,
                    src.Effects.HasMobility,
                    src.Effects.HasBuff,
                    src.Effects.HasDebuff,
                    src.Effects.HasExecute
                )
            ));

        // =========================================================
        // Execution Snapshot → DTO
        // =========================================================
        CreateMap<SkillExecutionSnapshot, SkillExecutionSnapshotDto>();

        // =========================================================
        // Effects Snapshot → DTO
        // =========================================================
        CreateMap<SkillEffectSnapshot, SkillEffectSnapshotDto>();

        // =========================================================
        // HeroStatSnapshot → DTO
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
                0f, // TODO: align domain snapshot
                src.MovementSpeed,
                src.AttackRange,
                src.CooldownReduction,
                src.ResourceRegeneration
            ));

        // =========================================================
        // HeroSkillKitSnapshot → DTO
        // =========================================================
        CreateMap<HeroSkillKitSnapshot, HeroSkillKitSnapshotDto>();

        // =========================================================
        // HeroSnapshot → DTO
        // =========================================================
        CreateMap<HeroSnapshot, HeroSnapshotDto>();
    }
}