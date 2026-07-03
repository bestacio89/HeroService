using HeroService.Contracts.Commands.Heroes.Classifications.OriginCulture;
using HeroService.Contracts.DTOs.Heroes;

namespace HeroService.Client.Http.Abstractions;

public interface IOriginCultureClient :
    IAdminLifecycleClient<
        OriginCultureDto,
        CreateOriginCultureCommand,
        RenameOriginCultureCommand>
{
}