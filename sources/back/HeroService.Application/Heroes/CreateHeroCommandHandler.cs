using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes;
using HeroService.Contracts.Persistence;
using HeroService.Domain.Heroes.Core;

namespace HeroService.Application.Heroes.Commands;

public sealed class CreateHeroCommandHandler : ICommandHandler<CreateHeroCommand, Guid>
{
  private readonly IEntityRepository<Hero, Guid> _heroes;
  private readonly IMythologyRepository _mythologies;
  private readonly IHeroAffiliationRepository _affiliations;
  private readonly IEntityFactory<Guid, Hero> _factory;

  public CreateHeroCommandHandler(
      IEntityRepository<Hero, Guid> heroes,
      IMythologyRepository mythologies,
      IHeroAffiliationRepository affiliations,
      IEntityFactory<Guid, Hero> factory)
  {
    _heroes = heroes;
    _mythologies = mythologies;
    _affiliations = affiliations;
    _factory = factory;
  }

  public async Task<Guid> Handle(CreateHeroCommand request, CancellationToken cancellationToken)
  {
    // 1. Resolve domain constraints (Mythology & Affiliation)
    var mythology = await _mythologies.GetByNameAsync(request.MythologyCode, cancellationToken)
        ?? throw new InvalidOperationException($"Mythology '{request.MythologyCode}' does not exist.");

    var affiliation = await _affiliations.GetByOriginAndMythologyAsync(request.OriginType, mythology.Id, cancellationToken)
        ?? throw new InvalidOperationException("Invalid Origin/Mythology pairing.");

    // 2. Instantiate Aggregate via Factory
    var hero = _factory.Create();

    // 3. Initialize minimum required state
    // We do NOT pass Skill IDs here because they are derived/projected 
    // downstream based on HeroClass or specialized SkillKit lookup logic.
    hero.Initialize(
        request.Name,
        request.HeroClassId,
        affiliation.Id,
        createdBy: "system"
    );

    // 4. Persist
    await _heroes.AddAsync(hero, cancellationToken);

    return hero.Id;
  }
}