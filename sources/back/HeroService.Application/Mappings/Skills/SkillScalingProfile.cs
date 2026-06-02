using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Mappings.Skills;

public sealed class SkillScalingProfile : FranzMapProfile
{
  public SkillScalingProfile()
  {
    CreateMap<SkillScalingModifier, SkillScalingModifierDto>()
        .ConstructUsing(profile => new SkillScalingModifierDto(
            profile.Id,
            profile.SkillId,
            profile.AttackDamageRatio,
            profile.AbilityPowerRatio,
            profile.MaxHealthRatio
        ));
  }
}