using HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;
using HeroService.Contracts.DTOs.Snapshots;

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
            MapExecution(src.Execution),
            MapEffects(src.Effects)
        ));

    // =========================================================
    // Execution Snapshot → DTO
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
    // Effects Snapshot → DTO
    // =========================================================
    CreateMap<SkillEffectSnapshot, SkillEffectSnapshotDto>()
        .ConstructUsing(src => new SkillEffectSnapshotDto(
            src.HasDamage,
            src.HasHealing,
            src.HasShield,
            src.HasCrowdControl,
            src.HasMobility,
            src.HasBuff,
            src.HasDebuff,
            src.HasExecute
        ));
  }

  // =========================================================
  // Small helpers (optional but keeps readability clean)
  // =========================================================
  private static SkillExecutionSnapshotDto MapExecution(SkillExecutionSnapshot src)
    => new(
        src.Cooldown,
        src.ManaCost,
        src.Damage,
        src.Healing,
        src.ShieldValue,
        src.CastTime,
        src.ChannelDuration,
        src.Range,
        src.CrowdControlDuration
    );

  private static SkillEffectSnapshotDto MapEffects(SkillEffectSnapshot src)
    => new(
        src.HasDamage,
        src.HasHealing,
        src.HasShield,
        src.HasCrowdControl,
        src.HasMobility,
        src.HasBuff,
        src.HasDebuff,
        src.HasExecute
    );
}