using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Context;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginCulture;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Commands.Heroes.Classifications.OriginCultures;

public sealed class RenameOriginCultureCommandHandler
    : ICommandHandler<RenameOriginCultureCommand>
{
  private readonly IEntityRepository<OriginCulture, Guid> _repository;

  public RenameOriginCultureCommandHandler(
      IEntityRepository<OriginCulture, Guid> repository)
  {
    _repository = repository;
  }

  public async Task Handle(
      RenameOriginCultureCommand command,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByIdAsync(command.Id, cancellationToken);

    var userId = MediatorContext.Current.UserId ?? "system";

    entity.Rename(command.Name, userId);

    await _repository.UpdateAsync(entity, cancellationToken);
  }
}