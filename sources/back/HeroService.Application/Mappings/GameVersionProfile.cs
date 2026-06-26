using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.GameVersions;
using HeroService.Domain.Heroes.Versioned.GameVersion;

namespace HeroService.Application.Mappings.GameVersions;

public sealed class GameVersionProfile : FranzMapProfile
{
  public GameVersionProfile()
  {
    CreateMap<GameVersion, GameVersionDto>()
        .ConstructUsing(v => new GameVersionDto(
            v.Id,
            v.VersionNumber,
            v.VersionName,
            v.IsActive
        ));
  }
}