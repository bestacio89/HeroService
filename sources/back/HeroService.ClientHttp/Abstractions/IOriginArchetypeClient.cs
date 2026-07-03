using HeroService.Contracts.Commands.Heroes.Classifications.OriginArchetypes;
using HeroService.Contracts.DTOs.Heroes;

namespace HeroService.Client.Http.Abstractions;

public interface IOriginArchetypeClient :
    IAdminLifecycleClient<
        OriginArchetypeDto,
        CreateOriginArchetypeCommand,
        RenameOriginArchetypeCommand>
{
}