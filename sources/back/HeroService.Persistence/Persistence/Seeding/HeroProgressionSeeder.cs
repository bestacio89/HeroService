using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Domain.Heroes.Progression;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

/// <summary>
/// Per-level stat growth for all 18 heroes, taken verbatim from the codex's
/// "HeroProgressionModifiers -- par niveau" table. Not version-scoped --
/// growth curves are part of a hero's identity, not a balance-patch axis
/// (see HeroModifierSeeder for the version-scoped multipliers).
/// </summary>
public sealed class HeroProgressionSeeder : ISeeder
{
  public int Order => 8;

  private readonly IEntityFactory<Guid, HeroProgressionModifiers> _factory;
  private readonly IEntityRepository<HeroProgressionModifiers, Guid> _repo;
  private readonly IHeroRepository _heroes;
  private readonly IUnitOfWork _uow;

  public HeroProgressionSeeder(
      IEntityFactory<Guid, HeroProgressionModifiers> factory,
      IEntityRepository<HeroProgressionModifiers, Guid> repo,
      IHeroRepository heroes,
      IUnitOfWork uow)
  {
    _factory = factory;
    _repo = repo;
    _heroes = heroes;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _repo.GetAllAsync(ct)).Any())
      return;

    var system = "seed-system";

    // =========================================================
    // THOR
    // =========================================================
    var thor = await _heroes.GetByNameAsync("Thor", ct)
        ?? throw new InvalidOperationException("Missing Hero: Thor");

    var thorProg = _factory.Create();
    thorProg.Define(
        heroId: thor.Id,
        healthPerLevel: 155f,
        manaPerLevel: 42f,
        attackDamagePerLevel: 7.5f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 3.6f,
        magicResistancePerLevel: 2.8f,
        attackSpeedPerLevel: 0.018f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.4f,
        createdBy: system);
    await _repo.AddAsync(thorProg, ct);

    // =========================================================
    // ARES
    // =========================================================
    var ares = await _heroes.GetByNameAsync("Ares", ct)
        ?? throw new InvalidOperationException("Missing Hero: Ares");

    var aresProg = _factory.Create();
    aresProg.Define(
        heroId: ares.Id,
        healthPerLevel: 148f,
        manaPerLevel: 40f,
        attackDamagePerLevel: 8.2f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 3.4f,
        magicResistancePerLevel: 2.6f,
        attackSpeedPerLevel: 0.02f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.4f,
        createdBy: system);
    await _repo.AddAsync(aresProg, ct);

    // =========================================================
    // SUSANOO
    // =========================================================
    var susanoo = await _heroes.GetByNameAsync("Susanoo", ct)
        ?? throw new InvalidOperationException("Missing Hero: Susanoo");

    var susanooProg = _factory.Create();
    susanooProg.Define(
        heroId: susanoo.Id,
        healthPerLevel: 140f,
        manaPerLevel: 45f,
        attackDamagePerLevel: 7.8f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 3.1f,
        magicResistancePerLevel: 2.5f,
        attackSpeedPerLevel: 0.024f,
        castSpeedPerLevel: 0.012f,
        resourceRegenerationPerLevel: 0.45f,
        createdBy: system);
    await _repo.AddAsync(susanooProg, ct);

    // =========================================================
    // LOKI
    // =========================================================
    var loki = await _heroes.GetByNameAsync("Loki", ct)
        ?? throw new InvalidOperationException("Missing Hero: Loki");

    var lokiProg = _factory.Create();
    lokiProg.Define(
        heroId: loki.Id,
        healthPerLevel: 118f,
        manaPerLevel: 38f,
        attackDamagePerLevel: 9.5f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.6f,
        magicResistancePerLevel: 2.2f,
        attackSpeedPerLevel: 0.026f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.35f,
        createdBy: system);
    await _repo.AddAsync(lokiProg, ct);

    // =========================================================
    // NYX
    // =========================================================
    var nyx = await _heroes.GetByNameAsync("Nyx", ct)
        ?? throw new InvalidOperationException("Missing Hero: Nyx");

    var nyxProg = _factory.Create();
    nyxProg.Define(
        heroId: nyx.Id,
        healthPerLevel: 120f,
        manaPerLevel: 40f,
        attackDamagePerLevel: 8.8f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.7f,
        magicResistancePerLevel: 2.3f,
        attackSpeedPerLevel: 0.024f,
        castSpeedPerLevel: 0.012f,
        resourceRegenerationPerLevel: 0.4f,
        createdBy: system);
    await _repo.AddAsync(nyxProg, ct);

    // =========================================================
    // SET
    // =========================================================
    var set = await _heroes.GetByNameAsync("Set", ct)
        ?? throw new InvalidOperationException("Missing Hero: Set");

    var setProg = _factory.Create();
    setProg.Define(
        heroId: set.Id,
        healthPerLevel: 128f,
        manaPerLevel: 42f,
        attackDamagePerLevel: 9.0f,
        magicDamagePerLevel: 1.5f,
        armorPerLevel: 2.9f,
        magicResistancePerLevel: 2.4f,
        attackSpeedPerLevel: 0.022f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.4f,
        createdBy: system);
    await _repo.AddAsync(setProg, ct);

    // =========================================================
    // ZEUS
    // =========================================================
    var zeus = await _heroes.GetByNameAsync("Zeus", ct)
        ?? throw new InvalidOperationException("Missing Hero: Zeus");

    var zeusProg = _factory.Create();
    zeusProg.Define(
        heroId: zeus.Id,
        healthPerLevel: 110f,
        manaPerLevel: 68f,
        attackDamagePerLevel: 4.2f,
        magicDamagePerLevel: 14.5f,
        armorPerLevel: 2.2f,
        magicResistancePerLevel: 2.6f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.02f,
        resourceRegenerationPerLevel: 0.7f,
        createdBy: system);
    await _repo.AddAsync(zeusProg, ct);

    // =========================================================
    // AMUN-RA
    // =========================================================
    var amunRa = await _heroes.GetByNameAsync("Amun-Ra", ct)
        ?? throw new InvalidOperationException("Missing Hero: Amun-Ra");

    var amunRaProg = _factory.Create();
    amunRaProg.Define(
        heroId: amunRa.Id,
        healthPerLevel: 122f,
        manaPerLevel: 74f,
        attackDamagePerLevel: 4.5f,
        magicDamagePerLevel: 13.5f,
        armorPerLevel: 2.4f,
        magicResistancePerLevel: 2.8f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.018f,
        resourceRegenerationPerLevel: 0.8f,
        createdBy: system);
    await _repo.AddAsync(amunRaProg, ct);

    // =========================================================
    // CHRONOS
    // =========================================================
    var chronos = await _heroes.GetByNameAsync("Chronos", ct)
        ?? throw new InvalidOperationException("Missing Hero: Chronos");

    var chronosProg = _factory.Create();
    chronosProg.Define(
        heroId: chronos.Id,
        healthPerLevel: 112f,
        manaPerLevel: 70f,
        attackDamagePerLevel: 4.0f,
        magicDamagePerLevel: 13.0f,
        armorPerLevel: 2.3f,
        magicResistancePerLevel: 2.7f,
        attackSpeedPerLevel: 0.01f,
        castSpeedPerLevel: 0.026f,
        resourceRegenerationPerLevel: 0.75f,
        createdBy: system);
    await _repo.AddAsync(chronosProg, ct);

    // =========================================================
    // HERAKLES
    // =========================================================
    var herakles = await _heroes.GetByNameAsync("Herakles", ct)
        ?? throw new InvalidOperationException("Missing Hero: Herakles");

    var heraklesProg = _factory.Create();
    heraklesProg.Define(
        heroId: herakles.Id,
        healthPerLevel: 185f,
        manaPerLevel: 45f,
        attackDamagePerLevel: 5.5f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 4.6f,
        magicResistancePerLevel: 3.6f,
        attackSpeedPerLevel: 0.014f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.5f,
        createdBy: system);
    await _repo.AddAsync(heraklesProg, ct);

    // =========================================================
    // ANUBIS
    // =========================================================
    var anubis = await _heroes.GetByNameAsync("Anubis", ct)
        ?? throw new InvalidOperationException("Missing Hero: Anubis");

    var anubisProg = _factory.Create();
    anubisProg.Define(
        heroId: anubis.Id,
        healthPerLevel: 178f,
        manaPerLevel: 52f,
        attackDamagePerLevel: 4.8f,
        magicDamagePerLevel: 2.5f,
        armorPerLevel: 4.4f,
        magicResistancePerLevel: 4.0f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.012f,
        resourceRegenerationPerLevel: 0.6f,
        createdBy: system);
    await _repo.AddAsync(anubisProg, ct);

    // =========================================================
    // HEL
    // =========================================================
    var hel = await _heroes.GetByNameAsync("Hel", ct)
        ?? throw new InvalidOperationException("Missing Hero: Hel");

    var helProg = _factory.Create();
    helProg.Define(
        heroId: hel.Id,
        healthPerLevel: 195f,
        manaPerLevel: 48f,
        attackDamagePerLevel: 5.0f,
        magicDamagePerLevel: 2.0f,
        armorPerLevel: 4.5f,
        magicResistancePerLevel: 3.9f,
        attackSpeedPerLevel: 0.01f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.55f,
        createdBy: system);
    await _repo.AddAsync(helProg, ct);

    // =========================================================
    // ISIS
    // =========================================================
    var isis = await _heroes.GetByNameAsync("Isis", ct)
        ?? throw new InvalidOperationException("Missing Hero: Isis");

    var isisProg = _factory.Create();
    isisProg.Define(
        heroId: isis.Id,
        healthPerLevel: 130f,
        manaPerLevel: 78f,
        attackDamagePerLevel: 3.8f,
        magicDamagePerLevel: 10.5f,
        armorPerLevel: 2.9f,
        magicResistancePerLevel: 3.2f,
        attackSpeedPerLevel: 0.01f,
        castSpeedPerLevel: 0.016f,
        resourceRegenerationPerLevel: 1.0f,
        createdBy: system);
    await _repo.AddAsync(isisProg, ct);

    // =========================================================
    // FREYJA
    // =========================================================
    var freyja = await _heroes.GetByNameAsync("Freyja", ct)
        ?? throw new InvalidOperationException("Missing Hero: Freyja");

    var freyjaProg = _factory.Create();
    freyjaProg.Define(
        heroId: freyja.Id,
        healthPerLevel: 140f,
        manaPerLevel: 72f,
        attackDamagePerLevel: 4.0f,
        magicDamagePerLevel: 9.5f,
        armorPerLevel: 3.2f,
        magicResistancePerLevel: 3.4f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.014f,
        resourceRegenerationPerLevel: 0.9f,
        createdBy: system);
    await _repo.AddAsync(freyjaProg, ct);

    // =========================================================
    // ENKI
    // =========================================================
    var enki = await _heroes.GetByNameAsync("Enki", ct)
        ?? throw new InvalidOperationException("Missing Hero: Enki");

    var enkiProg = _factory.Create();
    enkiProg.Define(
        heroId: enki.Id,
        healthPerLevel: 126f,
        manaPerLevel: 80f,
        attackDamagePerLevel: 3.5f,
        magicDamagePerLevel: 10.0f,
        armorPerLevel: 2.8f,
        magicResistancePerLevel: 3.1f,
        attackSpeedPerLevel: 0.01f,
        castSpeedPerLevel: 0.016f,
        resourceRegenerationPerLevel: 1.0f,
        createdBy: system);
    await _repo.AddAsync(enkiProg, ct);

    // =========================================================
    // ARTEMIS
    // =========================================================
    var artemis = await _heroes.GetByNameAsync("Artemis", ct)
        ?? throw new InvalidOperationException("Missing Hero: Artemis");

    var artemisProg = _factory.Create();
    artemisProg.Define(
        heroId: artemis.Id,
        healthPerLevel: 115f,
        manaPerLevel: 42f,
        attackDamagePerLevel: 9.8f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.5f,
        magicResistancePerLevel: 2.2f,
        attackSpeedPerLevel: 0.03f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.4f,
        createdBy: system);
    await _repo.AddAsync(artemisProg, ct);

    // =========================================================
    // VIDAR
    // =========================================================
    var vidar = await _heroes.GetByNameAsync("Vidar", ct)
        ?? throw new InvalidOperationException("Missing Hero: Vidar");

    var vidarProg = _factory.Create();
    vidarProg.Define(
        heroId: vidar.Id,
        healthPerLevel: 125f,
        manaPerLevel: 40f,
        attackDamagePerLevel: 10.5f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.8f,
        magicResistancePerLevel: 2.4f,
        attackSpeedPerLevel: 0.026f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.35f,
        createdBy: system);
    await _repo.AddAsync(vidarProg, ct);

    // =========================================================
    // RAMA
    // =========================================================
    var rama = await _heroes.GetByNameAsync("Rama", ct)
        ?? throw new InvalidOperationException("Missing Hero: Rama");

    var ramaProg = _factory.Create();
    ramaProg.Define(
        heroId: rama.Id,
        healthPerLevel: 118f,
        manaPerLevel: 44f,
        attackDamagePerLevel: 9.2f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.6f,
        magicResistancePerLevel: 2.3f,
        attackSpeedPerLevel: 0.032f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.45f,
        createdBy: system);
    await _repo.AddAsync(ramaProg, ct);

    await _uow.CommitAsync(ct);
  }
}