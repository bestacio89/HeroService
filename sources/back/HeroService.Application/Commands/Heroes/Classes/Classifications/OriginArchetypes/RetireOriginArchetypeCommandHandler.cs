using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Context;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginArchetype;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginArchetypes;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Commands.Heroes.Classifications.OriginArchetypes;

public sealed class RetireOriginArchetypeCommandHandler
    : ICommandHandler<RetireOriginArchetypeCommand>
{
  private readonly IEntityRepository<OriginArchetype, Guid> _repository;

  public RetireOriginArchetypeCommandHandler(
      IEntityRepository<OriginArchetype, Guid> repository)
  {
    _repository = repository;
  }

  public async Task Handle(
      RetireOriginArchetypeCommand command,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByIdAsync(command.Id, cancellationToken);

    var userId = MediatorContext.Current.UserId ?? "system";

    entity.MarkDeleted(userId);

    await _repository.UpdateAsync(entity, cancellationToken);
  }
}