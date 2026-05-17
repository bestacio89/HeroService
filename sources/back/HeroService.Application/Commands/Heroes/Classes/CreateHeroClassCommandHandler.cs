using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes;
using HeroService.Domain.Heroes.Core;

namespace HeroService.Application.Commands.Heroes.Classes;

public sealed class CreateHeroClassCommandHandler
    : ICommandHandler<CreateHeroClassCommand, Guid>
{
  private readonly IEntityFactory<Guid, HeroClass> _factory;
  private readonly IEntityRepository<HeroClass, Guid> _repository;

  public CreateHeroClassCommandHandler(
      IEntityFactory<Guid, HeroClass> factory,
      IEntityRepository<HeroClass, Guid> repository)
  {
    _factory = factory;
    _repository = repository;
  }

  public async Task<Guid> Handle(
      CreateHeroClassCommand command,
      CancellationToken cancellationToken)
  {
    var heroClass = _factory.Create();

    heroClass.Define(
        command.Name,
        createdBy: "system" // replace with real user context later
    );

    await _repository.AddAsync(heroClass, cancellationToken);

    return heroClass.Id;
  }
}