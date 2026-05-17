using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginCulture;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Commands.Heroes.Classifications.OriginCultures;

public sealed class CreateOriginCultureCommandHandler
    : ICommandHandler<CreateOriginCultureCommand, Guid>
{
  private readonly IEntityFactory<Guid, OriginCulture> _factory;
  private readonly IEntityRepository<OriginCulture, Guid> _repository;

  public CreateOriginCultureCommandHandler(
      IEntityFactory<Guid, OriginCulture> factory,
      IEntityRepository<OriginCulture, Guid> repository)
  {
    _factory = factory;
    _repository = repository;
  }

  public async Task<Guid> Handle(
      CreateOriginCultureCommand command,
      CancellationToken cancellationToken)
  {
    var entity = _factory.Create();

    entity.Define(command.Name, "System");

    await _repository.AddAsync(entity, cancellationToken);

    return entity.Id;
  }
}