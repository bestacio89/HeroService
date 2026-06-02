using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Mappings.Skills;

public sealed class SkillScalingProfile : FranzMapProfile
{
  public SkillScalingProfile()
  {
    CreateMap<SkillScalingProfile, SkillScalingModifierDto>()
        .ConstructUsing(profile => new SkillScalingProfileDto(
          profile.
     

        ));
  }
}