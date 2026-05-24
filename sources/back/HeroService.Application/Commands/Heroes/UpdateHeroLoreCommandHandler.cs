using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Context;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes.Lore;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Domain.Heroes.Core;

namespace HeroService.Application.Commands.Heroes.Lore;

public sealed class UpdateHeroLoreCommandHandler
    : ICommandHandler<UpdateHeroLoreCommand>
{
  private readonly IHeroRepository _heroes;
  private readonly IHeroLoreRepository _loreRepository;
  private readonly IEntityRepository<HeroLore, Guid> _heroLoreBaseRepository;

  public UpdateHeroLoreCommandHandler(
      IHeroRepository heroes,
      IHeroLoreRepository loreRepository,
    IEntityRepository<HeroLore, Guid> herolorebaserapo)
  {
    _heroes = heroes;
    _loreRepository = loreRepository;
    _heroLoreBaseRepository = herolorebaserapo;
  }

  public async Task Handle(
      UpdateHeroLoreCommand command,
      CancellationToken cancellationToken)
  {
    var hero = await _heroes.GetByNameAsync(
        command.HeroName,
        cancellationToken);

    if (hero is null)
      throw new InvalidOperationException(
          $"Hero '{command.HeroName}' not found.");

    var lore = await _loreRepository.GetByHeroIdAsync(
        hero.Id,
        cancellationToken);

    if (lore is null)
      throw new InvalidOperationException(
          $"Lore for hero '{command.HeroName}' not found.");

    var userId = MediatorContext.Current.UserId ?? "system";

    lore.Revise(
        command.Title,
        command.Description,
        command.BackgroundStory,
        userId
    );

    await _heroLoreBaseRepository.UpdateAsync(
        lore,
        cancellationToken);
  }
}