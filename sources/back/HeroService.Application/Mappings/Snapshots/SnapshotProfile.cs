using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Domain.Heroes.Versioned.Snapshotting;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

namespace HeroService.Application.Mappings.Snapshots;

public sealed class HeroSnapshotMappingProfile : FranzMapProfile
{
  public HeroSnapshotMappingProfile()
  {
    // =========================================================
    // 1. ROOT SNAPSHOT GRAPH
    // =========================================================
    CreateMap<HeroSnapshot, HeroSnapshotDto>()
        .ConstructUsing((src, mapper) => new HeroSnapshotDto(
            src.HeroId,
            src.HeroName,
            src.GameVersionId,
            mapper.Map<HeroStatSnapshot, HeroStatSnapshotDto>(src.Stats),
            mapper.Map<HeroSkillKitSnapshot, HeroSkillKitSnapshotDto>(src.SkillKit),
            mapper.Map<HeroKitProfile, HeroKitProfileDto>(src.KitProfile)
        ));

    // =========================================================
    // 2. SKILL KIT GROUPING
    // =========================================================
    CreateMap<HeroSkillKitSnapshot, HeroSkillKitSnapshotDto>()
        .ConstructUsing((src, mapper) => new HeroSkillKitSnapshotDto(
            mapper.Map<SkillSnapshot, SkillSnapshotDto>(src.Passive),
            mapper.Map<SkillSnapshot, SkillSnapshotDto>(src.Primary),
            mapper.Map<SkillSnapshot, SkillSnapshotDto>(src.Secondary),
            mapper.Map<SkillSnapshot, SkillSnapshotDto>(src.Tertiary),
            mapper.Map<SkillSnapshot, SkillSnapshotDto>(src.Ultimate)
        ));

    // =========================================================
    // 3. GRANULAR SKILL SUB-GRAPH (Bypasses Static Closures)
    // =========================================================
    CreateMap<SkillSnapshot, SkillSnapshotDto>()
        .ConstructUsing((src, mapper) => new SkillSnapshotDto(
            src.SkillId,
            src.Name,
            mapper.Map<SkillExecutionSnapshot, SkillExecutionSnapshotDto>(src.Execution),
            mapper.Map<SkillEffectSnapshot, SkillEffectSnapshotDto>(src.Effects),
            mapper.Map<IReadOnlyCollection<EffectExecutionSnapshot>, List<EffectExecutionSnapshotDto>>(src.EffectExecutions)
        ));

    // =========================================================
    // 4. CONVENTION METADATA PRE-WARMING
    // =========================================================
    CreateMap<HeroStatSnapshot, HeroStatSnapshotDto>();
    CreateMap<HeroKitProfile, HeroKitProfileDto>();
    CreateMap<SkillExecutionSnapshot, SkillExecutionSnapshotDto>();
    CreateMap<SkillEffectSnapshot, SkillEffectSnapshotDto>();
    CreateMap<EffectExecutionSnapshot, EffectExecutionSnapshotDto>();
  }
}