using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes.Classifications.MythologyTypes;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Commands.Heroes.Classifications.MythologyTypes;

public sealed class RetireMythologyTypeCommandHandler
    : ICommandHandler<RetireMythologyTypeCommand>
{
  private readonly IEntityRepository<MythologyType, Guid> _repository;

  public RetireMythologyTypeCommandHandler(
      IEntityRepository<MythologyType, Guid> repository)
  {
    _repository = repository;
  }

  public async Task Handle(
      RetireMythologyTypeCommand command,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByIdAsync(command.Id, cancellationToken);

    entity.MarkDeleted("system");

    await _repository.UpdateAsync(entity, cancellationToken);
  }
}