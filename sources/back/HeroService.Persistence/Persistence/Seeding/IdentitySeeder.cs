using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using HeroService.Domain.Heroes.Core;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

public sealed class IdentitySeeder : ISeeder2
{
  public int Order => 2;
  private readonly IEntityFactory<Guid, HeroClass> _classFactory;
  private readonly IEntityFactory<Guid, OriginArchetype> _archetypeFactory;
  private readonly IEntityFactory<Guid, MythologyType> _mythologyFactory;
  private readonly IEntityFactory<Guid, OriginCulture> _cultureFactory;

  private readonly IEntityRepository<HeroClass, Guid> _classes;
  private readonly IEntityRepository<OriginArchetype, Guid> _archetypes;
  private readonly IEntityRepository<MythologyType, Guid> _mythologies;
  private readonly IEntityRepository<OriginCulture, Guid> _cultures;

  private readonly IUnitOfWork _uow;

  public IdentitySeeder(
      IEntityFactory<Guid, HeroClass> classFactory,
      IEntityFactory<Guid, OriginArchetype> archetypeFactory,
      IEntityFactory<Guid, MythologyType> mythologyFactory,
      IEntityFactory<Guid, OriginCulture> cultureFactory,
      IEntityRepository<HeroClass, Guid> classes,
      IEntityRepository<OriginArchetype, Guid> archetypes,
      IEntityRepository<MythologyType, Guid> mythologies,
      IEntityRepository<OriginCulture, Guid> cultures,
      IUnitOfWork uow)
  {
    _classFactory = classFactory;
    _archetypeFactory = archetypeFactory;
    _mythologyFactory = mythologyFactory;
    _cultureFactory = cultureFactory;

    _classes = classes;
    _archetypes = archetypes;
    _mythologies = mythologies;
    _cultures = cultures;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _classes.GetAllAsync(ct)).Any())
      return;

    var system = "seed-system";

    // =========================================================
    // HERO CLASSES
    // =========================================================
    await SeedClass("Warrior", system, ct);
    await SeedClass("Tank", system, ct);
    await SeedClass("Mage", system, ct);
    await SeedClass("Assassin", system, ct);
    await SeedClass("Ranger", system, ct);
    await SeedClass("Support", system, ct);

    // =========================================================
    // ARCHETYPES
    // =========================================================
    await SeedArchetype("Bruiser", system, ct);
    await SeedArchetype("Divine", system, ct);
    await SeedArchetype("Trickster", system, ct);
    await SeedArchetype("Berserker", system, ct);
    await SeedArchetype("Protector", system, ct);

    // =========================================================
    // MYTHOLOGIES
    // =========================================================
    await SeedMythology("Greek", system, ct);
    await SeedMythology("Norse", system, ct);
    await SeedMythology("Egyptian", system, ct);
    await SeedMythology("Japanese", system, ct);
    await SeedMythology("Celtic", system, ct);

    // =========================================================
    // CULTURES
    // =========================================================
    await SeedCulture("Europe", system, ct);
    await SeedCulture("Asia", system, ct);
    await SeedCulture("Africa", system, ct);
    await SeedCulture("Americas", system, ct);
    await SeedCulture("Mediterranean", system, ct);

    await _uow.CommitAsync(ct);
  }

  private async Task SeedClass(string name, string createdBy, CancellationToken ct)
  {
    if ((await _classes.GetAllAsync(ct)).Any(x => x.Name == name))
      return;

    var entity = _classFactory.Create();
    entity.Define(name, createdBy);

    await _classes.AddAsync(entity, ct);
  }

  private async Task SeedArchetype(string name, string createdBy, CancellationToken ct)
  {
    if ((await _archetypes.GetAllAsync(ct)).Any(x => x.Name == name))
      return;

    var entity = _archetypeFactory.Create();
    entity.Define(name, createdBy);

    await _archetypes.AddAsync(entity, ct);
  }

  private async Task SeedMythology(string name, string createdBy, CancellationToken ct)
  {
    if ((await _mythologies.GetAllAsync(ct)).Any(x => x.Name == name))
      return;

    var entity = _mythologyFactory.Create();
    entity.Define(name, createdBy);

    await _mythologies.AddAsync(entity, ct);
  }

  private async Task SeedCulture(string name, string createdBy, CancellationToken ct)
  {
    if ((await _cultures.GetAllAsync(ct)).Any(x => x.Name == name))
      return;

    var entity = _cultureFactory.Create();
    entity.Define(name, createdBy);

    await _cultures.AddAsync(entity, ct);
  }
}