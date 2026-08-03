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

/// <summary>
/// Version-scoped balance multipliers for all 30 heroes, seeded against the
/// active GameVersion. Neutral (1.0f; 0.0f additive for IgnoreEnemyDefense) --
/// the codex supplies base stats, not a balance-patch delta. This is the
/// "as shipped, unpatched" baseline; a future rebalance seeder for a new
/// GameVersion is where real per-hero multipliers belong.
/// </summary>
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
        ?? throw new InvalidOperationException("No active GameVersion found.");

    var heroes = await _heroes.GetAllAsync(ct);

    if (heroes.Count == 0)
      return;

    var system = "seed-system";

    // =========================================================
    // THOR
    // =========================================================
    var thor = heroes.First(h => h.Name == "Thor");

    var thorMod = _heroModifierFactory.Create();
    thorMod.Define(
        version.Id,
        thor.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(thorMod, ct);

    // =========================================================
    // ARES
    // =========================================================
    var ares = heroes.First(h => h.Name == "Ares");

    var aresMod = _heroModifierFactory.Create();
    aresMod.Define(
        version.Id,
        ares.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(aresMod, ct);

    // =========================================================
    // GUAN YU
    // =========================================================
    var guanYu = heroes.First(h => h.Name == "Guan Yu");

    var guanYuMod = _heroModifierFactory.Create();
    guanYuMod.Define(
        version.Id,
        guanYu.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(guanYuMod, ct);

    // =========================================================
    // OGUN
    // =========================================================
    var ogun = heroes.First(h => h.Name == "Ogun");

    var ogunMod = _heroModifierFactory.Create();
    ogunMod.Define(
        version.Id,
        ogun.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(ogunMod, ct);

    // =========================================================
    // SUSANOO
    // =========================================================
    var susanoo = heroes.First(h => h.Name == "Susanoo");

    var susanooMod = _heroModifierFactory.Create();
    susanooMod.Define(
        version.Id,
        susanoo.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(susanooMod, ct);

    // =========================================================
    // LOKI
    // =========================================================
    var loki = heroes.First(h => h.Name == "Loki");

    var lokiMod = _heroModifierFactory.Create();
    lokiMod.Define(
        version.Id,
        loki.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(lokiMod, ct);

    // =========================================================
    // SET
    // =========================================================
    var set = heroes.First(h => h.Name == "Set");

    var setMod = _heroModifierFactory.Create();
    setMod.Define(
        version.Id,
        set.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(setMod, ct);

    // =========================================================
    // KALI
    // =========================================================
    var kali = heroes.First(h => h.Name == "Kali");

    var kaliMod = _heroModifierFactory.Create();
    kaliMod.Define(
        version.Id,
        kali.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(kaliMod, ct);

    // =========================================================
    // CAMAZOTZ
    // =========================================================
    var camazotz = heroes.First(h => h.Name == "Camazotz");

    var camazotzMod = _heroModifierFactory.Create();
    camazotzMod.Define(
        version.Id,
        camazotz.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(camazotzMod, ct);

    // =========================================================
    // NYX
    // =========================================================
    var nyx = heroes.First(h => h.Name == "Nyx");

    var nyxMod = _heroModifierFactory.Create();
    nyxMod.Define(
        version.Id,
        nyx.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(nyxMod, ct);

    // =========================================================
    // ZEUS
    // =========================================================
    var zeus = heroes.First(h => h.Name == "Zeus");

    var zeusMod = _heroModifierFactory.Create();
    zeusMod.Define(
        version.Id,
        zeus.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(zeusMod, ct);

    // =========================================================
    // RA
    // =========================================================
    var ra = heroes.First(h => h.Name == "Ra");

    var raMod = _heroModifierFactory.Create();
    raMod.Define(
        version.Id,
        ra.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(raMod, ct);

    // =========================================================
    // AGNI
    // =========================================================
    var agni = heroes.First(h => h.Name == "Agni");

    var agniMod = _heroModifierFactory.Create();
    agniMod.Define(
        version.Id,
        agni.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(agniMod, ct);

    // =========================================================
    // RAIJIN
    // =========================================================
    var raijin = heroes.First(h => h.Name == "Raijin");

    var raijinMod = _heroModifierFactory.Create();
    raijinMod.Define(
        version.Id,
        raijin.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(raijinMod, ct);

    // =========================================================
    // MARDUK
    // =========================================================
    var marduk = heroes.First(h => h.Name == "Marduk");

    var mardukMod = _heroModifierFactory.Create();
    mardukMod.Define(
        version.Id,
        marduk.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(mardukMod, ct);

    // =========================================================
    // HERAKLES
    // =========================================================
    var herakles = heroes.First(h => h.Name == "Herakles");

    var heraklesMod = _heroModifierFactory.Create();
    heraklesMod.Define(
        version.Id,
        herakles.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(heraklesMod, ct);

    // =========================================================
    // YMIR
    // =========================================================
    var ymir = heroes.First(h => h.Name == "Ymir");

    var ymirMod = _heroModifierFactory.Create();
    ymirMod.Define(
        version.Id,
        ymir.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(ymirMod, ct);

    // =========================================================
    // GEB
    // =========================================================
    var geb = heroes.First(h => h.Name == "Geb");

    var gebMod = _heroModifierFactory.Create();
    gebMod.Define(
        version.Id,
        geb.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(gebMod, ct);

    // =========================================================
    // KUMBHAKARNA
    // =========================================================
    var kumbhakarna = heroes.First(h => h.Name == "Kumbhakarna");

    var kumbhakarnaMod = _heroModifierFactory.Create();
    kumbhakarnaMod.Define(
        version.Id,
        kumbhakarna.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(kumbhakarnaMod, ct);

    // =========================================================
    // GILGAMESH
    // =========================================================
    var gilgamesh = heroes.First(h => h.Name == "Gilgamesh");

    var gilgameshMod = _heroModifierFactory.Create();
    gilgameshMod.Define(
        version.Id,
        gilgamesh.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(gilgameshMod, ct);

    // =========================================================
    // FREYJA
    // =========================================================
    var freyja = heroes.First(h => h.Name == "Freyja");

    var freyjaMod = _heroModifierFactory.Create();
    freyjaMod.Define(
        version.Id,
        freyja.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(freyjaMod, ct);

    // =========================================================
    // ISIS
    // =========================================================
    var isis = heroes.First(h => h.Name == "Isis");

    var isisMod = _heroModifierFactory.Create();
    isisMod.Define(
        version.Id,
        isis.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(isisMod, ct);

    // =========================================================
    // APHRODITE
    // =========================================================
    var aphrodite = heroes.First(h => h.Name == "Aphrodite");

    var aphroditeMod = _heroModifierFactory.Create();
    aphroditeMod.Define(
        version.Id,
        aphrodite.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(aphroditeMod, ct);

    // =========================================================
    // GUANYIN
    // =========================================================
    var guanyin = heroes.First(h => h.Name == "Guanyin");

    var guanyinMod = _heroModifierFactory.Create();
    guanyinMod.Define(
        version.Id,
        guanyin.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(guanyinMod, ct);

    // =========================================================
    // BRIGID
    // =========================================================
    var brigid = heroes.First(h => h.Name == "Brigid");

    var brigidMod = _heroModifierFactory.Create();
    brigidMod.Define(
        version.Id,
        brigid.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(brigidMod, ct);

    // =========================================================
    // ARTEMIS
    // =========================================================
    var artemis = heroes.First(h => h.Name == "Artemis");

    var artemisMod = _heroModifierFactory.Create();
    artemisMod.Define(
        version.Id,
        artemis.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(artemisMod, ct);

    // =========================================================
    // RAMA
    // =========================================================
    var rama = heroes.First(h => h.Name == "Rama");

    var ramaMod = _heroModifierFactory.Create();
    ramaMod.Define(
        version.Id,
        rama.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(ramaMod, ct);

    // =========================================================
    // HOUYI
    // =========================================================
    var houyi = heroes.First(h => h.Name == "Houyi");

    var houyiMod = _heroModifierFactory.Create();
    houyiMod.Define(
        version.Id,
        houyi.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(houyiMod, ct);

    // =========================================================
    // ULLR
    // =========================================================
    var ullr = heroes.First(h => h.Name == "Ullr");

    var ullrMod = _heroModifierFactory.Create();
    ullrMod.Define(
        version.Id,
        ullr.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(ullrMod, ct);

    // =========================================================
    // NEITH
    // =========================================================
    var neith = heroes.First(h => h.Name == "Neith");

    var neithMod = _heroModifierFactory.Create();
    neithMod.Define(
        version.Id,
        neith.Id,
        1.00f, // health
        1.00f, // mana
        1.00f, // attack damage
        1.00f, // magic damage
        0.00f, // ignore enemy defense (additive, not multiplicative)
        1.00f, // attack speed
        1.00f, // cast speed
        1.00f, // crit chance
        1.00f, // crit damage multiplier
        1.00f, // armor
        1.00f, // magic resistance
        1.00f, // damage reduction
        1.00f, // shield strength multiplier
        1.00f, // movement speed
        1.00f, // attack range
        1.00f, // cooldown reduction
        1.00f, // resource regeneration
        system);
    await _modifiers.AddAsync(neithMod, ct);

    await _uow.CommitAsync(ct);
  }
}