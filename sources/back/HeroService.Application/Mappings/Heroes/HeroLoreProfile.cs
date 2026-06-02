using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Domain.Heroes.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Application.Mappings.Heroes;

public class HeroLoreProfile : FranzMapProfile
{
  public HeroLoreProfile()
  {
    CreateMap<HeroLore, HeroLoreDto>()
      .ConstructUsing(l => new HeroLoreDto(
          l.Title,
          l.Description,
          l.BackgroundStory
      ));
  }
}
