using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Domain.Heroes.Progression;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

/// <summary>
/// Per-level stat growth for all 30 heroes, taken verbatim from the v17
/// codex's HeroProgressionModifiers table. Not version-scoped -- growth
/// curves are identity, not a balance-patch axis.
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
    await _repo.AddAsync(thorProg, ct);

    // =========================================================
    // ARES
    // =========================================================
    var ares = await _heroes.GetByNameAsync("Ares", ct)
        ?? throw new InvalidOperationException("Missing Hero: Ares");

    var aresProg = _factory.Create();
    aresProg.Define(
        heroId: ares.Id,
        healthPerLevel: 150f,
        manaPerLevel: 38f,
        attackDamagePerLevel: 8.5f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 3.3f,
        magicResistancePerLevel: 2.5f,
        attackSpeedPerLevel: 0.02f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.4f,
        createdBy: system);
    await _repo.AddAsync(aresProg, ct);

    // =========================================================
    // GUAN YU
    // =========================================================
    var guanYu = await _heroes.GetByNameAsync("Guan Yu", ct)
        ?? throw new InvalidOperationException("Missing Hero: Guan Yu");

    var guanYuProg = _factory.Create();
    guanYuProg.Define(
        heroId: guanYu.Id,
        healthPerLevel: 158f,
        manaPerLevel: 38f,
        attackDamagePerLevel: 7.6f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 3.6f,
        magicResistancePerLevel: 2.8f,
        attackSpeedPerLevel: 0.018f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.42f,
        createdBy: system);
    await _repo.AddAsync(guanYuProg, ct);

    // =========================================================
    // OGUN
    // =========================================================
    var ogun = await _heroes.GetByNameAsync("Ogun", ct)
        ?? throw new InvalidOperationException("Missing Hero: Ogun");

    var ogunProg = _factory.Create();
    ogunProg.Define(
        heroId: ogun.Id,
        healthPerLevel: 152f,
        manaPerLevel: 39f,
        attackDamagePerLevel: 8.4f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 3.5f,
        magicResistancePerLevel: 2.7f,
        attackSpeedPerLevel: 0.019f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.41f,
        createdBy: system);
    await _repo.AddAsync(ogunProg, ct);

    // =========================================================
    // SUSANOO
    // =========================================================
    var susanoo = await _heroes.GetByNameAsync("Susanoo", ct)
        ?? throw new InvalidOperationException("Missing Hero: Susanoo");

    var susanooProg = _factory.Create();
    susanooProg.Define(
        heroId: susanoo.Id,
        healthPerLevel: 142f,
        manaPerLevel: 44f,
        attackDamagePerLevel: 7.9f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 3.2f,
        magicResistancePerLevel: 2.5f,
        attackSpeedPerLevel: 0.023f,
        castSpeedPerLevel: 0.012f,
        resourceRegenerationPerLevel: 0.44f,
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
        healthPerLevel: 120f,
        manaPerLevel: 37f,
        attackDamagePerLevel: 9.6f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.6f,
        magicResistancePerLevel: 2.2f,
        attackSpeedPerLevel: 0.026f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.34f,
        createdBy: system);
    await _repo.AddAsync(lokiProg, ct);

    // =========================================================
    // SET
    // =========================================================
    var set = await _heroes.GetByNameAsync("Set", ct)
        ?? throw new InvalidOperationException("Missing Hero: Set");

    var setProg = _factory.Create();
    setProg.Define(
        heroId: set.Id,
        healthPerLevel: 124f,
        manaPerLevel: 39f,
        attackDamagePerLevel: 9.2f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.7f,
        magicResistancePerLevel: 2.3f,
        attackSpeedPerLevel: 0.024f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.36f,
        createdBy: system);
    await _repo.AddAsync(setProg, ct);

    // =========================================================
    // KALI
    // =========================================================
    var kali = await _heroes.GetByNameAsync("Kali", ct)
        ?? throw new InvalidOperationException("Missing Hero: Kali");

    var kaliProg = _factory.Create();
    kaliProg.Define(
        heroId: kali.Id,
        healthPerLevel: 122f,
        manaPerLevel: 38f,
        attackDamagePerLevel: 9.4f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.65f,
        magicResistancePerLevel: 2.25f,
        attackSpeedPerLevel: 0.025f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.35f,
        createdBy: system);
    await _repo.AddAsync(kaliProg, ct);

    // =========================================================
    // CAMAZOTZ
    // =========================================================
    var camazotz = await _heroes.GetByNameAsync("Camazotz", ct)
        ?? throw new InvalidOperationException("Missing Hero: Camazotz");

    var camazotzProg = _factory.Create();
    camazotzProg.Define(
        heroId: camazotz.Id,
        healthPerLevel: 123f,
        manaPerLevel: 37f,
        attackDamagePerLevel: 9.0f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.7f,
        magicResistancePerLevel: 2.25f,
        attackSpeedPerLevel: 0.024f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.35f,
        createdBy: system);
    await _repo.AddAsync(camazotzProg, ct);

    // =========================================================
    // NYX
    // =========================================================
    var nyx = await _heroes.GetByNameAsync("Nyx", ct)
        ?? throw new InvalidOperationException("Missing Hero: Nyx");

    var nyxProg = _factory.Create();
    nyxProg.Define(
        heroId: nyx.Id,
        healthPerLevel: 121f,
        manaPerLevel: 38f,
        attackDamagePerLevel: 9.1f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.65f,
        magicResistancePerLevel: 2.25f,
        attackSpeedPerLevel: 0.024f,
        castSpeedPerLevel: 0.012f,
        resourceRegenerationPerLevel: 0.35f,
        createdBy: system);
    await _repo.AddAsync(nyxProg, ct);

    // =========================================================
    // ZEUS
    // =========================================================
    var zeus = await _heroes.GetByNameAsync("Zeus", ct)
        ?? throw new InvalidOperationException("Missing Hero: Zeus");

    var zeusProg = _factory.Create();
    zeusProg.Define(
        heroId: zeus.Id,
        healthPerLevel: 100f,
        manaPerLevel: 55f,
        attackDamagePerLevel: 4.2f,
        magicDamagePerLevel: 10.3f,
        armorPerLevel: 2.2f,
        magicResistancePerLevel: 2.6f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.02f,
        resourceRegenerationPerLevel: 0.5f,
        createdBy: system);
    await _repo.AddAsync(zeusProg, ct);

    // =========================================================
    // RA
    // =========================================================
    var ra = await _heroes.GetByNameAsync("Ra", ct)
        ?? throw new InvalidOperationException("Missing Hero: Ra");

    var raProg = _factory.Create();
    raProg.Define(
        heroId: ra.Id,
        healthPerLevel: 113f,
        manaPerLevel: 55f,
        attackDamagePerLevel: 4.2f,
        magicDamagePerLevel: 8.9f,
        armorPerLevel: 2.2f,
        magicResistancePerLevel: 2.6f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.02f,
        resourceRegenerationPerLevel: 0.5f,
        createdBy: system);
    await _repo.AddAsync(raProg, ct);

    // =========================================================
    // AGNI
    // =========================================================
    var agni = await _heroes.GetByNameAsync("Agni", ct)
        ?? throw new InvalidOperationException("Missing Hero: Agni");

    var agniProg = _factory.Create();
    agniProg.Define(
        heroId: agni.Id,
        healthPerLevel: 105f,
        manaPerLevel: 55f,
        attackDamagePerLevel: 4.2f,
        magicDamagePerLevel: 9.5f,
        armorPerLevel: 2.2f,
        magicResistancePerLevel: 2.6f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.02f,
        resourceRegenerationPerLevel: 0.5f,
        createdBy: system);
    await _repo.AddAsync(agniProg, ct);

    // =========================================================
    // RAIJIN
    // =========================================================
    var raijin = await _heroes.GetByNameAsync("Raijin", ct)
        ?? throw new InvalidOperationException("Missing Hero: Raijin");

    var raijinProg = _factory.Create();
    raijinProg.Define(
        heroId: raijin.Id,
        healthPerLevel: 105f,
        manaPerLevel: 52f,
        attackDamagePerLevel: 4.2f,
        magicDamagePerLevel: 10.1f,
        armorPerLevel: 2.2f,
        magicResistancePerLevel: 2.6f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.02f,
        resourceRegenerationPerLevel: 0.5f,
        createdBy: system);
    await _repo.AddAsync(raijinProg, ct);

    // =========================================================
    // MARDUK
    // =========================================================
    var marduk = await _heroes.GetByNameAsync("Marduk", ct)
        ?? throw new InvalidOperationException("Missing Hero: Marduk");

    var mardukProg = _factory.Create();
    mardukProg.Define(
        heroId: marduk.Id,
        healthPerLevel: 105f,
        manaPerLevel: 59f,
        attackDamagePerLevel: 4.2f,
        magicDamagePerLevel: 9.5f,
        armorPerLevel: 2.2f,
        magicResistancePerLevel: 2.6f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.02f,
        resourceRegenerationPerLevel: 0.5f,
        createdBy: system);
    await _repo.AddAsync(mardukProg, ct);

    // =========================================================
    // HERAKLES
    // =========================================================
    var herakles = await _heroes.GetByNameAsync("Herakles", ct)
        ?? throw new InvalidOperationException("Missing Hero: Herakles");

    var heraklesProg = _factory.Create();
    heraklesProg.Define(
        heroId: herakles.Id,
        healthPerLevel: 178f,
        manaPerLevel: 38f,
        attackDamagePerLevel: 6.4f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 4.4f,
        magicResistancePerLevel: 4.0f,
        attackSpeedPerLevel: 0.015f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.42f,
        createdBy: system);
    await _repo.AddAsync(heraklesProg, ct);

    // =========================================================
    // YMIR
    // =========================================================
    var ymir = await _heroes.GetByNameAsync("Ymir", ct)
        ?? throw new InvalidOperationException("Missing Hero: Ymir");

    var ymirProg = _factory.Create();
    ymirProg.Define(
        heroId: ymir.Id,
        healthPerLevel: 196f,
        manaPerLevel: 38f,
        attackDamagePerLevel: 6.0f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 5.0f,
        magicResistancePerLevel: 4.0f,
        attackSpeedPerLevel: 0.015f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.42f,
        createdBy: system);
    await _repo.AddAsync(ymirProg, ct);

    // =========================================================
    // GEB
    // =========================================================
    var geb = await _heroes.GetByNameAsync("Geb", ct)
        ?? throw new InvalidOperationException("Missing Hero: Geb");

    var gebProg = _factory.Create();
    gebProg.Define(
        heroId: geb.Id,
        healthPerLevel: 178f,
        manaPerLevel: 38f,
        attackDamagePerLevel: 6.0f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 5.1f,
        magicResistancePerLevel: 4.0f,
        attackSpeedPerLevel: 0.015f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.42f,
        createdBy: system);
    await _repo.AddAsync(gebProg, ct);

    // =========================================================
    // KUMBHAKARNA
    // =========================================================
    var kumbhakarna = await _heroes.GetByNameAsync("Kumbhakarna", ct)
        ?? throw new InvalidOperationException("Missing Hero: Kumbhakarna");

    var kumbhakarnaProg = _factory.Create();
    kumbhakarnaProg.Define(
        heroId: kumbhakarna.Id,
        healthPerLevel: 199f,
        manaPerLevel: 38f,
        attackDamagePerLevel: 6.0f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 4.6f,
        magicResistancePerLevel: 4.0f,
        attackSpeedPerLevel: 0.014f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.42f,
        createdBy: system);
    await _repo.AddAsync(kumbhakarnaProg, ct);

    // =========================================================
    // GILGAMESH
    // =========================================================
    var gilgamesh = await _heroes.GetByNameAsync("Gilgamesh", ct)
        ?? throw new InvalidOperationException("Missing Hero: Gilgamesh");

    var gilgameshProg = _factory.Create();
    gilgameshProg.Define(
        heroId: gilgamesh.Id,
        healthPerLevel: 178f,
        manaPerLevel: 38f,
        attackDamagePerLevel: 6.3f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 4.6f,
        magicResistancePerLevel: 4.2f,
        attackSpeedPerLevel: 0.015f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.42f,
        createdBy: system);
    await _repo.AddAsync(gilgameshProg, ct);

    // =========================================================
    // FREYJA
    // =========================================================
    var freyja = await _heroes.GetByNameAsync("Freyja", ct)
        ?? throw new InvalidOperationException("Missing Hero: Freyja");

    var freyjaProg = _factory.Create();
    freyjaProg.Define(
        heroId: freyja.Id,
        healthPerLevel: 120f,
        manaPerLevel: 48f,
        attackDamagePerLevel: 4.8f,
        magicDamagePerLevel: 6.0f,
        armorPerLevel: 2.8f,
        magicResistancePerLevel: 3.2f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.015f,
        resourceRegenerationPerLevel: 0.55f,
        createdBy: system);
    await _repo.AddAsync(freyjaProg, ct);

    // =========================================================
    // ISIS
    // =========================================================
    var isis = await _heroes.GetByNameAsync("Isis", ct)
        ?? throw new InvalidOperationException("Missing Hero: Isis");

    var isisProg = _factory.Create();
    isisProg.Define(
        heroId: isis.Id,
        healthPerLevel: 120f,
        manaPerLevel: 48f,
        attackDamagePerLevel: 4.8f,
        magicDamagePerLevel: 6.0f,
        armorPerLevel: 2.8f,
        magicResistancePerLevel: 3.2f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.015f,
        resourceRegenerationPerLevel: 0.6f,
        createdBy: system);
    await _repo.AddAsync(isisProg, ct);

    // =========================================================
    // APHRODITE
    // =========================================================
    var aphrodite = await _heroes.GetByNameAsync("Aphrodite", ct)
        ?? throw new InvalidOperationException("Missing Hero: Aphrodite");

    var aphroditeProg = _factory.Create();
    aphroditeProg.Define(
        heroId: aphrodite.Id,
        healthPerLevel: 114f,
        manaPerLevel: 48f,
        attackDamagePerLevel: 4.8f,
        magicDamagePerLevel: 6.0f,
        armorPerLevel: 2.8f,
        magicResistancePerLevel: 3.2f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.015f,
        resourceRegenerationPerLevel: 0.55f,
        createdBy: system);
    await _repo.AddAsync(aphroditeProg, ct);

    // =========================================================
    // GUANYIN
    // =========================================================
    var guanyin = await _heroes.GetByNameAsync("Guanyin", ct)
        ?? throw new InvalidOperationException("Missing Hero: Guanyin");

    var guanyinProg = _factory.Create();
    guanyinProg.Define(
        heroId: guanyin.Id,
        healthPerLevel: 120f,
        manaPerLevel: 48f,
        attackDamagePerLevel: 4.8f,
        magicDamagePerLevel: 6.0f,
        armorPerLevel: 2.8f,
        magicResistancePerLevel: 3.2f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.015f,
        resourceRegenerationPerLevel: 0.55f,
        createdBy: system);
    await _repo.AddAsync(guanyinProg, ct);

    // =========================================================
    // BRIGID
    // =========================================================
    var brigid = await _heroes.GetByNameAsync("Brigid", ct)
        ?? throw new InvalidOperationException("Missing Hero: Brigid");

    var brigidProg = _factory.Create();
    brigidProg.Define(
        heroId: brigid.Id,
        healthPerLevel: 114f,
        manaPerLevel: 48f,
        attackDamagePerLevel: 4.8f,
        magicDamagePerLevel: 6.4f,
        armorPerLevel: 2.8f,
        magicResistancePerLevel: 3.2f,
        attackSpeedPerLevel: 0.012f,
        castSpeedPerLevel: 0.015f,
        resourceRegenerationPerLevel: 0.55f,
        createdBy: system);
    await _repo.AddAsync(brigidProg, ct);

    // =========================================================
    // ARTEMIS
    // =========================================================
    var artemis = await _heroes.GetByNameAsync("Artemis", ct)
        ?? throw new InvalidOperationException("Missing Hero: Artemis");

    var artemisProg = _factory.Create();
    artemisProg.Define(
        heroId: artemis.Id,
        healthPerLevel: 103f,
        manaPerLevel: 36f,
        attackDamagePerLevel: 8.8f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.4f,
        magicResistancePerLevel: 2.2f,
        attackSpeedPerLevel: 0.03f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.35f,
        createdBy: system);
    await _repo.AddAsync(artemisProg, ct);

    // =========================================================
    // RAMA
    // =========================================================
    var rama = await _heroes.GetByNameAsync("Rama", ct)
        ?? throw new InvalidOperationException("Missing Hero: Rama");

    var ramaProg = _factory.Create();
    ramaProg.Define(
        heroId: rama.Id,
        healthPerLevel: 108f,
        manaPerLevel: 36f,
        attackDamagePerLevel: 9.3f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.4f,
        magicResistancePerLevel: 2.2f,
        attackSpeedPerLevel: 0.03f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.35f,
        createdBy: system);
    await _repo.AddAsync(ramaProg, ct);

    // =========================================================
    // HOUYI
    // =========================================================
    var houyi = await _heroes.GetByNameAsync("Houyi", ct)
        ?? throw new InvalidOperationException("Missing Hero: Houyi");

    var houyiProg = _factory.Create();
    houyiProg.Define(
        heroId: houyi.Id,
        healthPerLevel: 108f,
        manaPerLevel: 36f,
        attackDamagePerLevel: 8.8f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.4f,
        magicResistancePerLevel: 2.2f,
        attackSpeedPerLevel: 0.028f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.35f,
        createdBy: system);
    await _repo.AddAsync(houyiProg, ct);

    // =========================================================
    // ULLR
    // =========================================================
    var ullr = await _heroes.GetByNameAsync("Ullr", ct)
        ?? throw new InvalidOperationException("Missing Hero: Ullr");

    var ullrProg = _factory.Create();
    ullrProg.Define(
        heroId: ullr.Id,
        healthPerLevel: 103f,
        manaPerLevel: 36f,
        attackDamagePerLevel: 8.8f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.4f,
        magicResistancePerLevel: 2.2f,
        attackSpeedPerLevel: 0.03f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.35f,
        createdBy: system);
    await _repo.AddAsync(ullrProg, ct);

    // =========================================================
    // NEITH
    // =========================================================
    var neith = await _heroes.GetByNameAsync("Neith", ct)
        ?? throw new InvalidOperationException("Missing Hero: Neith");

    var neithProg = _factory.Create();
    neithProg.Define(
        heroId: neith.Id,
        healthPerLevel: 108f,
        manaPerLevel: 36f,
        attackDamagePerLevel: 8.8f,
        magicDamagePerLevel: 0f,
        armorPerLevel: 2.4f,
        magicResistancePerLevel: 2.2f,
        attackSpeedPerLevel: 0.03f,
        castSpeedPerLevel: 0.01f,
        resourceRegenerationPerLevel: 0.35f,
        createdBy: system);
    await _repo.AddAsync(neithProg, ct);

    await _uow.CommitAsync(ct);
  }
}