using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Core.Skills;

namespace HeroService.Application.Mappings.Heroes;

public sealed class HeroProfile : FranzMapProfile
{
  public HeroProfile()
  {
    // =========================================================
    // 1. ROOT HERO GRAPH
    // =========================================================
    CreateMap<Hero, HeroDto>()
        .ConstructUsing((h, mapper) => new HeroDto(
            h.Id,
            h.Name,
            h.HeroClassId,
            mapper.Map<HeroClass, HeroClassDto>(h.HeroClass),
            mapper.Map<HeroAffiliation, HeroAffiliationDto>(h.Affiliation),
            h.BaseStats != null ? mapper.Map<HeroBaseStats, HeroBaseStatsDto>(h.BaseStats) : null!,
            h.SkillKit != null ? mapper.Map<HeroSkillKit, HeroSkillKitDto>(h.SkillKit) : null!
        ));

    // =========================================================
    // 2. AFFILIATION COMPOSITE GRAPH
    // =========================================================
    CreateMap<HeroAffiliation, HeroAffiliationDto>()
        .ConstructUsing((src, mapper) => new HeroAffiliationDto(
            mapper.Map<OriginArchetype, OriginArchetypeDto>(src.Archetype),
            mapper.Map<MythologyType, MythologyTypeDto>(src.Mythology),
            mapper.Map<OriginCulture, OriginCultureDto>(src.OriginCulture)
        ));

    // =========================================================
    // 3. CONVENTION METADATA PRE-WARMING
    // =========================================================
    CreateMap<HeroClass, HeroClassDto>();
    CreateMap<OriginArchetype, OriginArchetypeDto>();
    CreateMap<MythologyType, MythologyTypeDto>();
    CreateMap<OriginCulture, OriginCultureDto>();
    CreateMap<HeroBaseStats, HeroBaseStatsDto>();
    CreateMap<HeroSkillKit, HeroSkillKitDto>();
  }
}