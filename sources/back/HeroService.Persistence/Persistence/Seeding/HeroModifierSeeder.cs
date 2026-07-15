using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Contracts.Persistence;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

public sealed class HeroModifierSeeder : ISeeder
{
  public int Order => 6;

  private readonly IEntityFactory<Guid, HeroModifier> _heroModifierFactory;
  private readonly IGameVersionRepository _versions;
  private readonly IEntityRepository<HeroModifier, Guid> _modifiers;
  private readonly IEntityRepository<Hero, Guid> _heroes;
  private readonly IUnitOfWork _uow;


  public HeroModifierSeeder(
      IEntityFactory<Guid, HeroModifier> heroModifierFactory,
      IGameVersionRepository versions,
      IEntityRepository<HeroModifier, Guid> modifiers,
      IEntityRepository<Hero, Guid> heroes,
      IUnitOfWork uow)
  {
    _heroModifierFactory = heroModifierFactory;
    _versions = versions;
    _modifiers = modifiers;
    _heroes = heroes;
    _uow = uow;
  }


  public async Task SeedAsync(CancellationToken ct)
  {
    var existing = await _modifiers.GetAllAsync(ct);

    if (existing.Any())
      return;


    var version = await _versions.GetActiveAsync(ct)
        ?? throw new InvalidOperationException(
            "No active GameVersion found.");


    var heroes = await _heroes.GetAllAsync(ct);

    if (heroes.Count == 0)
      return;


    var system = "seed-system";


    // =========================================================
    // THOR MODIFIER
    // =========================================================

    var thor = heroes.First(h => h.Name == "Thor");


    var thorMod = _heroModifierFactory.Create();

    thorMod.Define(
        version.Id,
        thor.Id,

        1.00f,
        1.00f,

        0.95f,
        1.05f,

        0.05f, // +5% Ignore Enemy Defense

        1.00f,
        1.00f,

        1.00f,
        1.10f,

        1.00f,
        1.00f,
        1.00f,

        1.00f,

        1.00f,
        1.00f,

        0.95f,
        1.00f,

        system
    );


    await _modifiers.AddAsync(thorMod, ct);



    // =========================================================
    // HERAKLES MODIFIER
    // =========================================================

    var herakles = heroes.First(h => h.Name == "Herakles");


    var heraklesMod = _heroModifierFactory.Create();

    heraklesMod.Define(
        version.Id,
        herakles.Id,

        1.15f,
        1.05f,

        0.90f,
        0.85f,

        0.02f, // +2% Ignore Enemy Defense

        0.95f,
        1.00f,

        1.05f,
        1.00f,

        1.20f,
        1.15f,
        1.10f,

        1.25f,

        0.90f,
        1.00f,

        1.05f,
        1.10f,

        system
    );


    await _modifiers.AddAsync(heraklesMod, ct);


    await _uow.CommitAsync(ct);
  }
}