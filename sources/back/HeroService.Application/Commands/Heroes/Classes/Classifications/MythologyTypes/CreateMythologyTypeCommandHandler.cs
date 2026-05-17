using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Context;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes.Classifications.MythologyTypes;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Commands.Heroes.Classifications.MythologyTypes;

public sealed class CreateMythologyTypeCommandHandler
    : ICommandHandler<CreateMythologyTypeCommand, Guid>
{
  private readonly IEntityFactory<Guid, MythologyType> _factory;
  private readonly IEntityRepository<MythologyType, Guid> _repository;

  public CreateMythologyTypeCommandHandler(
      IEntityFactory<Guid, MythologyType> factory,
      IEntityRepository<MythologyType, Guid> repository)
  {
    _factory = factory;
    _repository = repository;
  }

  public async Task<Guid> Handle(
      CreateMythologyTypeCommand command,
      CancellationToken cancellationToken)
  {
    var entity = _factory.Create();

    var userId = MediatorContext.Current.UserId ?? "system";

    entity.Define(command.Name, userId);

    await _repository.AddAsync(entity, cancellationToken);

    return entity.Id;
  }
}