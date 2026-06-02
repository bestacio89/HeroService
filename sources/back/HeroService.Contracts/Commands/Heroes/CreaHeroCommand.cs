using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Requests;

namespace HeroService.Contracts.Commands.Heroes;

public sealed record CreateHeroCommand(
    HeroCreateRequestDto Request
) : ICommand<Guid>;