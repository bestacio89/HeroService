using HeroService.Contracts.Commands.Heroes.Classifications.MythologyTypes;
using HeroService.Contracts.DTOs.Heroes;

namespace HeroService.Client.Http.Abstractions;

public interface IMythologyTypeClient :
    IAdminLifecycleClient<
        MythologyTypeDto,
        CreateMythologyTypeCommand,
        RenameMythologyTypeCommand>
{
}