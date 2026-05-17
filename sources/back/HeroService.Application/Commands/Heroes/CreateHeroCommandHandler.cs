using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Application.Commands.Heroes.Services;
using HeroService.Contracts.Commands.Heroes;
using HeroService.Contracts.DTOs.Requests;
using HeroService.Contracts.Persistence;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Core.Skills;

public sealed class CreateHeroCommandHandler : ICommandHandler<CreateHeroCommand, Guid>
{
  private readonly IHeroCreationService _creationService;

  public CreateHeroCommandHandler(IHeroCreationService creationService)
  {
    _creationService = creationService;
  }

  public async Task<Guid> Handle(CreateHeroCommand command, CancellationToken ct)
  {
    return await _creationService.CreateAsync(command.Request, ct);
  }
}