using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Context;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes.Classifications.MythologyTypes;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Commands.Heroes.Classifications.MythologyTypes;

public sealed class RenameMythologyTypeCommandHandler
    : ICommandHandler<RenameMythologyTypeCommand>
{
  private readonly IEntityRepository<MythologyType, Guid> _repository;

  public RenameMythologyTypeCommandHandler(
      IEntityRepository<MythologyType, Guid> repository)
  {
    _repository = repository;
  }

  public async Task Handle(
      RenameMythologyTypeCommand command,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByIdAsync(command.Id, cancellationToken);

    var userId = MediatorContext.Current.UserId ?? "system";

    entity.Rename(command.Name, userId);

    await _repository.UpdateAsync(entity, cancellationToken);
  }
}