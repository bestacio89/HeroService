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
/// Version-scoped balance multipliers for all 18 heroes, seeded against the
/// active GameVersion. All multipliers are neutral (1.0f = no change from
/// HeroBaseStats; 0.0f additive for IgnoreEnemyDefense) since the codex only
/// supplies base stats, not a balance-patch delta -- this is the "1.0.0 as
/// shipped, unpatched" baseline. A future rebalance seeder for a new
/// GameVersion is where real per-hero multipliers would live.
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
    // AMUN-RA
    // =========================================================
    var amunRa = heroes.First(h => h.Name == "Amun-Ra");

    var amunRaMod = _heroModifierFactory.Create();
    amunRaMod.Define(
        version.Id,
        amunRa.Id,
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
    await _modifiers.AddAsync(amunRaMod, ct);

    // =========================================================
    // CHRONOS
    // =========================================================
    var chronos = heroes.First(h => h.Name == "Chronos");

    var chronosMod = _heroModifierFactory.Create();
    chronosMod.Define(
        version.Id,
        chronos.Id,
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
    await _modifiers.AddAsync(chronosMod, ct);

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
    // ANUBIS
    // =========================================================
    var anubis = heroes.First(h => h.Name == "Anubis");

    var anubisMod = _heroModifierFactory.Create();
    anubisMod.Define(
        version.Id,
        anubis.Id,
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
    await _modifiers.AddAsync(anubisMod, ct);

    // =========================================================
    // HEL
    // =========================================================
    var hel = heroes.First(h => h.Name == "Hel");

    var helMod = _heroModifierFactory.Create();
    helMod.Define(
        version.Id,
        hel.Id,
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
    await _modifiers.AddAsync(helMod, ct);

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
    // ENKI
    // =========================================================
    var enki = heroes.First(h => h.Name == "Enki");

    var enkiMod = _heroModifierFactory.Create();
    enkiMod.Define(
        version.Id,
        enki.Id,
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
    await _modifiers.AddAsync(enkiMod, ct);

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
    // VIDAR
    // =========================================================
    var vidar = heroes.First(h => h.Name == "Vidar");

    var vidarMod = _heroModifierFactory.Create();
    vidarMod.Define(
        version.Id,
        vidar.Id,
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
    await _modifiers.AddAsync(vidarMod, ct);

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

    await _uow.CommitAsync(ct);
  }
}