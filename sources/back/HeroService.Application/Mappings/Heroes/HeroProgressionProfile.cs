using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Domain.Heroes.Progression;

namespace HeroService.Application.Mappings.Heroes;

public sealed class HeroProgressionProfile : FranzMapProfile
{
  public HeroProgressionProfile()
  {
    CreateMap<HeroProgressionModifiers, HeroProgressionModifiersDto>()
        .ConstructUsing(mod => new HeroProgressionModifiersDto(
            mod.Id,
            mod.HeroId,
            mod.HealthPerLevel,
            mod.ManaPerLevel,
            mod.AttackDamagePerLevel,
            mod.MagicDamagePerLevel,
            mod.ArmorPerLevel,
            mod.MagicResistancePerLevel,
            mod.AttackSpeedPerLevel,
            mod.CastSpeedPerLevel,
            mod.ResourceRegenerationPerLevel
        ));
  }
}