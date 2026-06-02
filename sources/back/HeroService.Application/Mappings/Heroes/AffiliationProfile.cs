using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Mappings.Heroes;

public class HeroAffiliationProfile : FranzMapProfile
{
  public HeroAffiliationProfile()
  {
    // ============================
    // HERO AFFILIATION → DTO
    // ============================
    CreateMap<HeroAffiliation, HeroAffiliationDto>()
        .ConstructUsing(a => new HeroAffiliationDto(
            new OriginArchetypeDto(
                a.Archetype.Id,
                a.Archetype.Name
            ),

            new MythologyTypeDto(
                a.Mythology.Id,
                a.Mythology.Name
            ),

            new OriginCultureDto(
                a.OriginCulture.Id,
                a.OriginCulture.Name
            )
        ));

    // ============================
    // CLASSIFICATIONS → DTOs
    // (reusable mappings if needed elsewhere)
    // ============================

    CreateMap<OriginArchetype, OriginArchetypeDto>()
        .ConstructUsing(src => new OriginArchetypeDto(
            src.Id,
            src.Name
        ));

    CreateMap<MythologyType, MythologyTypeDto>()
        .ConstructUsing(src => new MythologyTypeDto(
            src.Id,
            src.Name
        ));

    CreateMap<OriginCulture, OriginCultureDto>()
        .ConstructUsing(src => new OriginCultureDto(
            src.Id,
            src.Name
        ));
  }
}