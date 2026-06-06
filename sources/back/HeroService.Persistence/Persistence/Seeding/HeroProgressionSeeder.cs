using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Domain.Heroes.Progression;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

public sealed class HeroProgressionSeeder : ISeeder
{
  public int Order => 8;

  private readonly IEntityFactory<Guid, HeroProgressionModifiers> _factory;
  private readonly IEntityRepository<HeroProgressionModifiers, Guid> _repo;
  private readonly IHeroRepository _heroes; // Domain interface inheriting from INameLookupRepository
  private readonly Franz.Common.EntityFramework.IUnitOfWork _uow;

  public HeroProgressionSeeder(
      IEntityFactory<Guid, HeroProgressionModifiers> factory,
      IEntityRepository<HeroProgressionModifiers, Guid> repo,
      IHeroRepository heroes,
      Franz.Common.EntityFramework.IUnitOfWork uow)
  {
    _factory = factory;
    _repo = repo;
    _heroes = heroes;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    // Idempotency Boundary Check
    if ((await _repo.GetAllAsync(ct)).Any())
      return;

    var system = "seed-system";

    // Direct lookups matching your exact contract method signature
    var thor = await _heroes.GetByNameAsync("Thor", ct)
        ?? throw new InvalidOperationException("Domain configuration error during seed. Missing Hero root: Thor");

    var herakles = await _heroes.GetByNameAsync("Herakles", ct)
        ?? throw new InvalidOperationException("Domain configuration error during seed. Missing Hero root: Herakles");

    // =========================================================
    // THOR PROGRESSION MODIFIERS (Bruiser Template)
    // =========================================================
    var thorProg = _factory.Create();
    thorProg.Define(
        heroId: thor.Id,
        healthPerLevel: 88f,
        manaPerLevel: 35f,
        attackDamagePerLevel: 3.5f,
        abilityPowerPerLevel: 2.2f,
        armorPerLevel: 4.0f,
        magicResistancePerLevel: 1.75f,
        attackSpeedPerLevel: 0.025f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.5f,
        createdBy: system
    );
    await _repo.AddAsync(thorProg, ct);

    // =========================================================
    // HERAKLES PROGRESSION MODIFIERS (Pure Tank/Juggernaut Template)
    // =========================================================
    var heraklesProg = _factory.Create();
    heraklesProg.Define(
        heroId: herakles.Id,
        healthPerLevel: 104f,
        manaPerLevel: 20f,
        attackDamagePerLevel: 4.2f,
        abilityPowerPerLevel: 0.0f,
        armorPerLevel: 5.2f,
        magicResistancePerLevel: 2.1f,
        attackSpeedPerLevel: 0.018f,
        castSpeedPerLevel: 0.0f,
        resourceRegenerationPerLevel: 0.8f,
        createdBy: system
    );
    await _repo.AddAsync(heraklesProg, ct);

    // Transaction batch save execution
    await _uow.CommitAsync(ct);
  }
}