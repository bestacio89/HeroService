using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes;
using HeroService.Contracts.Persistence;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using HeroService.Domain.Heroes.Core;

namespace HeroService.Application.Heroes.Commands;

public sealed class CreateHeroCommandHandler
    : ICommandHandler<CreateHeroCommand, Guid>
{
  private readonly IEntityRepository<Hero, Guid> _heroes;
  private readonly IEntityRepository<MythologyType, Guid> _mythologies;
  private readonly IHeroAffiliationRepository _affiliations;
  private readonly IEntityFactory<Guid, Hero> _factory;

  public CreateHeroCommandHandler(
      IEntityRepository<Hero, Guid> heroes,
      IEntityRepository<MythologyType, Guid> mythologies,
      IHeroAffiliationRepository affiliations,
      IEntityFactory<Guid, Hero> factory)
  {
    _heroes = heroes;
    _mythologies = mythologies;
    _affiliations = affiliations;
    _factory = factory;
  }

  public async Task<Guid> Handle(
    CreateHeroCommand request,
    CancellationToken cancellationToken)
  {
    var hero = _factory.Create();

    hero.GetType()
        .GetProperty("Name")!
        .SetValue(hero, request.Name);

    hero.GetType()
        .GetProperty("HeroClassId")!
        .SetValue(hero, request.HeroClassId);

    var mythology = await _mythologies.GetByIdAsync(
        request.MythologyCode.ToLower,
        cancellationToken)
        ?? throw new InvalidOperationException("Mythology does not exist");

    var affiliation = await _affiliations.GetByMythologyAndOriginAsync(
        mythology.Name,
        request.OriginType,
        cancellationToken)
        ?? throw new InvalidOperationException(
            "No valid affiliation exists for this Mythology + Origin combination");

    hero.SetAffiliation(affiliation.Id);

    hero.MarkCreated("system");

    await _heroes.AddAsync(hero, cancellationToken);

    return hero.Id;
  }
}