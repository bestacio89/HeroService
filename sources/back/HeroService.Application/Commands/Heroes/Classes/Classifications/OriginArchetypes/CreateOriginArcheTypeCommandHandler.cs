using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Context;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginArchetypes;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Commands.Heroes.Classifications.OriginArchetypes;

public sealed class CreateOriginArchetypeCommandHandler
    : ICommandHandler<CreateOriginArchetypeCommand, Guid>
{
  private readonly IEntityFactory<Guid, OriginArchetype> _factory;
  private readonly IEntityRepository<OriginArchetype, Guid> _repository;

  public CreateOriginArchetypeCommandHandler(
      IEntityFactory<Guid, OriginArchetype> factory,
      IEntityRepository<OriginArchetype, Guid> repository)
  {
    _factory = factory;
    _repository = repository;
  }

  public async Task<Guid> Handle(
      CreateOriginArchetypeCommand command,
      CancellationToken cancellationToken)
  {
    var entity = _factory.Create();

    var userId = MediatorContext.Current.UserId ?? "system";

    entity.Define(command.Name, userId);

    await _repository.AddAsync(entity, cancellationToken);

    return entity.Id;
  }
}