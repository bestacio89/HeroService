using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Contracts.Persistence;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Core.Skills;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

/// <summary>
/// Seeds the full 18-hero roster, translated and mapped from the
/// "codex_complet_heros_items_v10" design document.
///
/// NOTE: HeroBaseStats.Define now requires baseIgnoreEnemyDefense (0.0-1.0),
/// which the codex does not provide (its 16-stat table predates this field).
/// Every hero defaults to 0.0 (no penetration) below -- flag for design review,
/// not an invented balance choice.
///
/// Depends on IdentitySeeder (Order 2) and SkillSeeder (Order 3) having run first.
/// </summary>
public sealed class HeroSeeder : ISeeder
{
  public int Order => 4;

  private readonly IEntityFactory<Guid, Hero> _heroFactory;
  private readonly IEntityFactory<Guid, HeroBaseStats> _statsFactory;
  private readonly IEntityFactory<Guid, HeroLore> _loreFactory;

  private readonly IEntityRepository<Hero, Guid> _heroes;
  private readonly IEntityRepository<HeroLore, Guid> _loreRepo;

  private readonly IHeroClassRepository _classes;
  private readonly IMythologyRepository _mythologies;
  private readonly IArchetypeRepository _archetypes;
  private readonly IOriginCultureRepository _cultures;

  private readonly ISkillRepository _skills;
  private readonly IUnitOfWork _uow;

  public HeroSeeder(
      IEntityFactory<Guid, Hero> heroFactory,
      IEntityFactory<Guid, HeroBaseStats> statsFactory,
      IEntityFactory<Guid, HeroLore> loreFactory,
      IEntityRepository<Hero, Guid> heroes,
      IEntityRepository<HeroLore, Guid> loreRepo,
      IHeroClassRepository classes,
      IMythologyRepository mythologies,
      IArchetypeRepository archetypes,
      IOriginCultureRepository cultures,
      ISkillRepository skills,
      IUnitOfWork uow)
  {
    _heroFactory = heroFactory;
    _statsFactory = statsFactory;
    _loreFactory = loreFactory;
    _heroes = heroes;
    _loreRepo = loreRepo;
    _classes = classes;
    _mythologies = mythologies;
    _archetypes = archetypes;
    _cultures = cultures;
    _skills = skills;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _heroes.GetAllAsync(ct)).Any())
      return;

    const string system = "seed-system";

    Skill GetSkill(string name) =>
        _skills.GetByNameAsync(name, ct).GetAwaiter().GetResult()
        ?? throw new InvalidOperationException($"Missing Skill: {name}");

    // HeroClasses
    var classAssassin = await _classes.GetByNameAsync("Assassin", ct)
        ?? throw new InvalidOperationException("Missing HeroClass: Assassin");
    var classMage = await _classes.GetByNameAsync("Mage", ct)
        ?? throw new InvalidOperationException("Missing HeroClass: Mage");
    var classRanger = await _classes.GetByNameAsync("Ranger", ct)
        ?? throw new InvalidOperationException("Missing HeroClass: Ranger");
    var classSupport = await _classes.GetByNameAsync("Support", ct)
        ?? throw new InvalidOperationException("Missing HeroClass: Support");
    var classTank = await _classes.GetByNameAsync("Tank", ct)
        ?? throw new InvalidOperationException("Missing HeroClass: Tank");
    var classWarrior = await _classes.GetByNameAsync("Warrior", ct)
        ?? throw new InvalidOperationException("Missing HeroClass: Warrior");

    // Mythologies
    var mythEgyptian = await _mythologies.GetByNameAsync("Egyptian", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Egyptian");
    var mythGreek = await _mythologies.GetByNameAsync("Greek", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Greek");
    var mythHindu = await _mythologies.GetByNameAsync("Hindu", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Hindu");
    var mythJapanese = await _mythologies.GetByNameAsync("Japanese", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Japanese");
    var mythMesopotamian = await _mythologies.GetByNameAsync("Mesopotamian", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Mesopotamian");
    var mythNorse = await _mythologies.GetByNameAsync("Norse", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Norse");

    // Origin Archetypes
    var archDemigod = await _archetypes.GetByNameAsync("Demigod", ct)
        ?? throw new InvalidOperationException("Missing Archetype: Demigod");
    var archDivine = await _archetypes.GetByNameAsync("Divine", ct)
        ?? throw new InvalidOperationException("Missing Archetype: Divine");
    var archHybrid = await _archetypes.GetByNameAsync("Hybrid", ct)
        ?? throw new InvalidOperationException("Missing Archetype: Hybrid");

    // Origin Cultures
    var cultureAfrica = await _cultures.GetByNameAsync("Africa", ct)
        ?? throw new InvalidOperationException("Missing Culture: Africa");
    var cultureAsia = await _cultures.GetByNameAsync("Asia", ct)
        ?? throw new InvalidOperationException("Missing Culture: Asia");
    var cultureEurope = await _cultures.GetByNameAsync("Europe", ct)
        ?? throw new InvalidOperationException("Missing Culture: Europe");
    var cultureMediterranean = await _cultures.GetByNameAsync("Mediterranean", ct)
        ?? throw new InvalidOperationException("Missing Culture: Mediterranean");

    // =========================================================
    // THOR
    // =========================================================
    var thor = _heroFactory.Create();

    var thorStats = _statsFactory.Create();
    thorStats.Define(
        thor.Id,
        2900f, 420f,
        72f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.85f, 1.0f,
        0.05f, 1.75f,
        32f, 28f, 0.03f,
        1.0f,
        3.55f, 1.8f,
        0.0f, 7.5f,
        system);

    thor.Define(
        "Thor",
        classWarrior.Id,
        new HeroAffiliation(archDivine.Id, mythNorse.Id, cultureEurope.Id),
        thorStats,
        system);

    thor.SetSkillKit(new HeroSkillKit(
        GetSkill("Lightning Charge").Id,
        GetSkill("Thunder Strike").Id,
        GetSkill("Hammer's Call").Id,
        GetSkill("Asgard's Guard").Id,
        GetSkill("Asgard's Wrath").Id));

    var thorLore = CreateLore(thor.Id,
        "God of Thunder",
        "God of thunder, wielding the hammer Mjolnir. He walks straight at the enemy and never retreats.",
        "The longer the fight lasts, the more lightning gathers around him. Purely aggressive: dominant in prolonged melee and lane pushing. Strong when he can stick to his target; weak against highly mobile enemies who keep their distance.",
        system);

    await PersistHero(thor, thorLore, ct);

    // =========================================================
    // ARES
    // =========================================================
    var ares = _heroFactory.Create();

    var aresStats = _statsFactory.Create();
    aresStats.Define(
        ares.Id,
        2750f, 400f,
        78f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.92f, 1.0f,
        0.08f, 1.8f,
        30f, 26f, 0.02f,
        1.0f,
        3.6f, 1.8f,
        0.0f, 7.0f,
        system);

    ares.Define(
        "Ares",
        classWarrior.Id,
        new HeroAffiliation(archDivine.Id, mythGreek.Id, cultureMediterranean.Id),
        aresStats,
        system);

    ares.SetSkillKit(new HeroSkillKit(
        GetSkill("Bloodlust").Id,
        GetSkill("Blood Harvest").Id,
        GetSkill("Butcher's Charge").Id,
        GetSkill("War Cry").Id,
        GetSkill("War's Fury").Id));

    var aresLore = CreateLore(ares.Id,
        "God of Slaughter",
        "God of brutal war, who loves carnage for its own sake. He feeds on spilled blood and grows stronger as the battle intensifies.",
        "Melee warrior who thrives in team-fight chaos. Excellent when surrounded by several enemies; vulnerable in an isolated duel or when forced to retreat.",
        system);

    await PersistHero(ares, aresLore, ct);

    // =========================================================
    // SUSANOO
    // =========================================================
    var susanoo = _heroFactory.Create();

    var susanooStats = _statsFactory.Create();
    susanooStats.Define(
        susanoo.Id,
        2600f, 440f,
        74f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.02f, 1.05f,
        0.1f, 1.8f,
        27f, 25f, 0.0f,
        1.0f,
        3.65f, 1.9f,
        0.05f, 7.5f,
        system);

    susanoo.Define(
        "Susanoo",
        classWarrior.Id,
        new HeroAffiliation(archDivine.Id, mythJapanese.Id, cultureAsia.Id),
        susanooStats,
        system);

    susanoo.SetSkillKit(new HeroSkillKit(
        GetSkill("Typhoon's Breath").Id,
        GetSkill("Gale Blade").Id,
        GetSkill("Typhoon Step").Id,
        GetSkill("Cutting Wind").Id,
        GetSkill("Orochi").Id));

    var susanooLore = CreateLore(susanoo.Id,
        "God of Storms",
        "Storm god, Amaterasu's exiled brother, impetuous and quarrelsome. Fights with a blade with the violence of a typhoon, never pausing to breathe.",
        "Combo warrior: rewards uninterrupted chains of attacks. Fearsome while he keeps the initiative; his power collapses if interrupted or kept at range.",
        system);

    await PersistHero(susanoo, susanooLore, ct);

    // =========================================================
    // LOKI
    // =========================================================
    var loki = _heroFactory.Create();

    var lokiStats = _statsFactory.Create();
    lokiStats.Define(
        loki.Id,
        2350f, 380f,
        88f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.08f, 1.0f,
        0.15f, 2.0f,
        22f, 20f, 0.0f,
        1.0f,
        3.75f, 1.8f,
        0.05f, 6.5f,
        system);

    loki.Define(
        "Loki",
        classAssassin.Id,
        new HeroAffiliation(archHybrid.Id, mythNorse.Id, cultureEurope.Id),
        lokiStats,
        system);

    loki.SetSkillKit(new HeroSkillKit(
        GetSkill("Deceit").Id,
        GetSkill("Twin Blades").Id,
        GetSkill("Shadow Flee").Id,
        GetSkill("Poisoned Dagger").Id,
        GetSkill("Trickster's Verdict").Id));

    var lokiLore = CreateLore(loki.Id,
        "The Trickster God",
        "Deceitful god, master of illusion and the knife in the back. Never fights fairly: he waits, strikes, and vanishes before any retaliation.",
        "Execution assassin: finishes off already-weakened targets and punishes poor positioning. Devastating against an isolated target; nearly useless in a frontal fight or once spotted early.",
        system);

    await PersistHero(loki, lokiLore, ct);

    // =========================================================
    // NYX
    // =========================================================
    var nyx = _heroFactory.Create();

    var nyxStats = _statsFactory.Create();
    nyxStats.Define(
        nyx.Id,
        2400f, 400f,
        82f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.05f, 1.05f,
        0.12f, 1.9f,
        23f, 22f, 0.0f,
        1.0f,
        3.85f, 1.8f,
        0.1f, 6.5f,
        system);

    nyx.Define(
        "Nyx",
        classAssassin.Id,
        new HeroAffiliation(archDivine.Id, mythGreek.Id, cultureMediterranean.Id),
        nyxStats,
        system);

    nyx.SetSkillKit(new HeroSkillKit(
        GetSkill("Child of Night").Id,
        GetSkill("Nightfall Veil").Id,
        GetSkill("Starstep").Id,
        GetSkill("Shadow Claws").Id,
        GetSkill("Everlasting Night").Id));

    var nyxLore = CreateLore(nyx.Id,
        "Primordial Goddess of Night",
        "A primordial goddess of Night, so ancient even Zeus fears her. She doesn't fight; she appears, takes a life, and dissolves into darkness.",
        "Elusive assassin built on constant repositioning. Excels at harassing and choosing her moment; extremely fragile if pinned down or denied movement.",
        system);

    await PersistHero(nyx, nyxLore, ct);

    // =========================================================
    // SET
    // =========================================================
    var set = _heroFactory.Create();

    var setStats = _statsFactory.Create();
    setStats.Define(
        set.Id,
        2500f, 420f,
        84f, 15f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.0f, 1.0f,
        0.1f, 1.9f,
        25f, 24f, 0.0f,
        1.0f,
        3.7f, 1.9f,
        0.05f, 7.0f,
        system);

    set.Define(
        "Set",
        classAssassin.Id,
        new HeroAffiliation(archDivine.Id, mythEgyptian.Id, cultureAfrica.Id),
        setStats,
        system);

    set.SetSkillKit(new HeroSkillKit(
        GetSkill("Desert Venom").Id,
        GetSkill("Desert's Breath").Id,
        GetSkill("Jackal's Hunt").Id,
        GetSkill("Corrosive Sands").Id,
        GetSkill("Curse of Chaos").Id));

    var setLore = CreateLore(set.Id,
        "God of Chaos",
        "God of chaos and the desert, killer of his own brother Osiris. Poisons his prey before patiently dismantling it.",
        "Corrosion assassin: weakens first, then strikes twice. Fearsome against targets he can wear down over time; less effective when forced to kill instantly.",
        system);

    await PersistHero(set, setLore, ct);

    // =========================================================
    // ZEUS
    // =========================================================
    var zeus = _heroFactory.Create();

    var zeusStats = _statsFactory.Create();
    zeusStats.Define(
        zeus.Id,
        2250f, 620f,
        42f, 120f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.72f, 1.15f,
        0.0f, 1.6f,
        18f, 24f, 0.0f,
        1.0f,
        3.4f, 5.8f,
        0.05f, 10.0f,
        system);

    zeus.Define(
        "Zeus",
        classMage.Id,
        new HeroAffiliation(archDivine.Id, mythGreek.Id, cultureMediterranean.Id),
        zeusStats,
        system);

    zeus.SetSkillKit(new HeroSkillKit(
        GetSkill("Static").Id,
        GetSkill("Targeted Bolt").Id,
        GetSkill("Olympus Gale").Id,
        GetSkill("Electric Arc").Id,
        GetSkill("Sky's Fury").Id));

    var zeusLore = CreateLore(zeus.Id,
        "King of Olympus",
        "King of Olympus, master of lightning. He strikes from afar, from the sky, and never descends into melee.",
        "Pure aggression and area-damage mage. Excellent at holding a position and clearing waves; extremely vulnerable if approached by an assassin.",
        system);

    await PersistHero(zeus, zeusLore, ct);

    // =========================================================
    // AMUN-RA
    // =========================================================
    var amunRa = _heroFactory.Create();

    var amunRaStats = _statsFactory.Create();
    amunRaStats.Define(
        amunRa.Id,
        2400f, 680f,
        44f, 110f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.75f, 1.1f,
        0.0f, 1.6f,
        20f, 26f, 0.02f,
        1.05f,
        3.45f, 5.5f,
        0.08f, 11.0f,
        system);

    amunRa.Define(
        "Amun-Ra",
        classMage.Id,
        new HeroAffiliation(archDivine.Id, mythEgyptian.Id, cultureAfrica.Id),
        amunRaStats,
        system);

    amunRa.SetSkillKit(new HeroSkillKit(
        GetSkill("Radiance").Id,
        GetSkill("Solar Disc").Id,
        GetSkill("Celestial Barque").Id,
        GetSkill("Sacred Burn").Id,
        GetSkill("Eternal Noon").Id));

    var amunRaLore = CreateLore(amunRa.Id,
        "The Sun King",
        "Solar god, king of the gods of Egypt. He does not strike with lightning: he radiates, and burns whatever stays too long under his gaze.",
        "Ramp-up and versatility mage: the more he varies his spells, the more powerful he becomes. Weak early in a fight, terrifying once he has had time to settle in.",
        system);

    await PersistHero(amunRa, amunRaLore, ct);

    // =========================================================
    // CHRONOS
    // =========================================================
    var chronos = _heroFactory.Create();

    var chronosStats = _statsFactory.Create();
    chronosStats.Define(
        chronos.Id,
        2300f, 650f,
        40f, 105f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.7f, 1.25f,
        0.0f, 1.6f,
        19f, 25f, 0.0f,
        1.0f,
        3.42f, 5.6f,
        0.15f, 10.5f,
        system);

    chronos.Define(
        "Chronos",
        classMage.Id,
        new HeroAffiliation(archDivine.Id, mythGreek.Id, cultureMediterranean.Id),
        chronosStats,
        system);

    chronos.SetSkillKit(new HeroSkillKit(
        GetSkill("Time Slip").Id,
        GetSkill("Hour Fracture").Id,
        GetSkill("Temporal Leap").Id,
        GetSkill("Temporal Shard").Id,
        GetSkill("Hourglass Reversal").Id));

    var chronosLore = CreateLore(chronos.Id,
        "Primordial Titan of Time",
        "Primordial Titan of Time, older than the gods. He doesn't hit harder than others: he strikes more often, while his enemies slow to a crawl.",
        "Tempo mage who manipulates the rhythm of combat. Excels at creating windows where the opponent cannot respond; weak if caught before establishing control.",
        system);

    await PersistHero(chronos, chronosLore, ct);

    // =========================================================
    // HERAKLES
    // =========================================================
    var herakles = _heroFactory.Create();

    var heraklesStats = _statsFactory.Create();
    heraklesStats.Define(
        herakles.Id,
        3400f, 450f,
        55f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.8f, 1.0f,
        0.0f, 1.5f,
        45f, 38f, 0.05f,
        1.1f,
        3.45f, 1.8f,
        0.0f, 8.0f,
        system);

    herakles.Define(
        "Herakles",
        classTank.Id,
        new HeroAffiliation(archDemigod.Id, mythGreek.Id, cultureMediterranean.Id),
        heraklesStats,
        system);

    herakles.SetSkillKit(new HeroSkillKit(
        GetSkill("Titanic Strength").Id,
        GetSkill("Nemean Grip").Id,
        GetSkill("Lion's Charge").Id,
        GetSkill("Lion's Skin").Id,
        GetSkill("Twelve Labors").Id));

    var heraklesLore = CreateLore(herakles.Id,
        "The Twelve Labors",
        "The greatest of heroes, made a god through his twelve labors. He seizes the enemy and hauls them bodily into his own camp.",
        "Initiator tank: opens fights by grabbing a key target. Excellent at creating engagements; if his grab misses, he ends up alone in enemy territory.",
        system);

    await PersistHero(herakles, heraklesLore, ct);

    // =========================================================
    // ANUBIS
    // =========================================================
    var anubis = _heroFactory.Create();

    var anubisStats = _statsFactory.Create();
    anubisStats.Define(
        anubis.Id,
        3300f, 520f,
        48f, 25f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.75f, 1.05f,
        0.0f, 1.5f,
        42f, 44f, 0.06f,
        1.3f,
        3.4f, 2.0f,
        0.05f, 9.0f,
        system);

    anubis.Define(
        "Anubis",
        classTank.Id,
        new HeroAffiliation(archDivine.Id, mythEgyptian.Id, cultureAfrica.Id),
        anubisStats,
        system);

    anubis.SetSkillKit(new HeroSkillKit(
        GetSkill("Guardian of the Threshold").Id,
        GetSkill("Judgment's Veil").Id,
        GetSkill("Passage of Shadows").Id,
        GetSkill("Funerary Scepter").Id,
        GetSkill("Weighing of Souls").Id));

    var anubisLore = CreateLore(anubis.Id,
        "Guardian of the Threshold",
        "Guardian of the threshold of the dead, weigher of souls. He does not seek battle: he stands between the enemy and those he protects.",
        "Protector tank: escorts fragile allies and absorbs the blows meant for them. Excellent alongside a carry; poor when isolated far from his team.",
        system);

    await PersistHero(anubis, anubisLore, ct);

    // =========================================================
    // HEL
    // =========================================================
    var hel = _heroFactory.Create();

    var helStats = _statsFactory.Create();
    helStats.Define(
        hel.Id,
        3550f, 480f,
        50f, 20f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.72f, 1.0f,
        0.0f, 1.5f,
        44f, 42f, 0.08f,
        1.15f,
        3.3f, 2.2f,
        0.05f, 8.5f,
        system);

    hel.Define(
        "Hel",
        classTank.Id,
        new HeroAffiliation(archHybrid.Id, mythNorse.Id, cultureEurope.Id),
        helStats,
        system);

    hel.SetSkillKit(new HeroSkillKit(
        GetSkill("Chill of the Dead").Id,
        GetSkill("Grasp of the Fallen").Id,
        GetSkill("Walk of the Dead").Id,
        GetSkill("Icy Breath").Id,
        GetSkill("Domain of Helheim").Id));

    var helLore = CreateLore(hel.Id,
        "Queen of the Dead",
        "Queen of the dead, half living and half corpse. She does not chase anyone: she closes the doors, and no one leaves her domain.",
        "Jailer tank: locks down a zone and denies the enemy any escape. Terrifying in enclosed spaces; can simply be avoided in wide open terrain.",
        system);

    await PersistHero(hel, helLore, ct);

    // =========================================================
    // ISIS
    // =========================================================
    var isis = _heroFactory.Create();

    var isisStats = _statsFactory.Create();
    isisStats.Define(
        isis.Id,
        2500f, 700f,
        38f, 85f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.72f, 1.1f,
        0.0f, 1.5f,
        24f, 30f, 0.02f,
        1.15f,
        3.45f, 5.0f,
        0.1f, 14.0f,
        system);

    isis.Define(
        "Isis",
        classSupport.Id,
        new HeroAffiliation(archDivine.Id, mythEgyptian.Id, cultureAfrica.Id),
        isisStats,
        system);

    isis.SetSkillKit(new HeroSkillKit(
        GetSkill("Ancient Magic").Id,
        GetSkill("Healing Wing").Id,
        GetSkill("Isis's Flight").Id,
        GetSkill("Ray of Light").Id,
        GetSkill("Osiris's Reprieve").Id));

    var isisLore = CreateLore(isis.Id,
        "Goddess of Magic and Healing",
        "Goddess of magic and healing, she who gathered Osiris's body back together. She keeps her team standing when everything else falls apart.",
        "Healer support: guarantees team endurance over long fights. Excellent in extended engagements; weak if the enemy can burst her allies down instantly.",
        system);

    await PersistHero(isis, isisLore, ct);

    // =========================================================
    // FREYJA
    // =========================================================
    var freyja = _heroFactory.Create();

    var freyjaStats = _statsFactory.Create();
    freyjaStats.Define(
        freyja.Id,
        2650f, 660f,
        42f, 75f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.75f, 1.05f,
        0.0f, 1.5f,
        28f, 32f, 0.03f,
        1.35f,
        3.5f, 4.5f,
        0.08f, 12.0f,
        system);

    freyja.Define(
        "Freyja",
        classSupport.Id,
        new HeroAffiliation(archDivine.Id, mythNorse.Id, cultureEurope.Id),
        freyjaStats,
        system);

    freyja.SetSkillKit(new HeroSkillKit(
        GetSkill("Valkyries' Favor").Id,
        GetSkill("Valkyries' Blessing").Id,
        GetSkill("Falcon Flight").Id,
        GetSkill("Golden Blade").Id,
        GetSkill("Field of Folkvangr").Id));

    var freyjaLore = CreateLore(freyja.Id,
        "Chooser of the Slain",
        "Goddess of love and war, who chooses half of those slain in battle. She protects before the blow lands, rather than repairing after it.",
        "Protective support: shields and prevents damage before it happens. Excellent at preparing an engagement; less useful if the team is already taking raw, sustained damage.",
        system);

    await PersistHero(freyja, freyjaLore, ct);

    // =========================================================
    // ENKI
    // =========================================================
    var enki = _heroFactory.Create();

    var enkiStats = _statsFactory.Create();
    enkiStats.Define(
        enki.Id,
        2450f, 720f,
        36f, 80f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.7f, 1.1f,
        0.0f, 1.5f,
        23f, 29f, 0.0f,
        1.1f,
        3.48f, 5.2f,
        0.15f, 13.0f,
        system);

    enki.Define(
        "Enki",
        classSupport.Id,
        new HeroAffiliation(archDivine.Id, mythMesopotamian.Id, cultureAsia.Id),
        enkiStats,
        system);

    enki.SetSkillKit(new HeroSkillKit(
        GetSkill("Water's Wisdom").Id,
        GetSkill("Stream of Wisdom").Id,
        GetSkill("Undercurrent").Id,
        GetSkill("Invigorating Wave").Id,
        GetSkill("Abzu").Id));

    var enkiLore = CreateLore(enki.Id,
        "God of Wisdom and Fresh Water",
        "God of fresh water and cunning wisdom, always finding a solution. He does not strike: he gives others the means to win.",
        "Amplifier support: makes his allies better through varied buffs. Wins fights without ever appearing on the damage chart; entirely dependent on the quality of his team.",
        system);

    await PersistHero(enki, enkiLore, ct);

    // =========================================================
    // ARTEMIS
    // =========================================================
    var artemis = _heroFactory.Create();

    var artemisStats = _statsFactory.Create();
    artemisStats.Define(
        artemis.Id,
        2300f, 400f,
        80f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.1f, 1.0f,
        0.18f, 2.0f,
        20f, 20f, 0.0f,
        1.0f,
        3.55f, 6.5f,
        0.0f, 7.0f,
        system);

    artemis.Define(
        "Artemis",
        classRanger.Id,
        new HeroAffiliation(archDivine.Id, mythGreek.Id, cultureMediterranean.Id),
        artemisStats,
        system);

    artemis.SetSkillKit(new HeroSkillKit(
        GetSkill("Huntress's Eye").Id,
        GetSkill("Lunar Arrow").Id,
        GetSkill("Doe's Leap").Id,
        GetSkill("Rain of Arrows").Id,
        GetSkill("Sacred Hunt").Id));

    var artemisLore = CreateLore(artemis.Id,
        "Goddess of the Hunt",
        "Goddess of the hunt, an infallible archer. She tracks the isolated prey and never misses her mark.",
        "Hunter ranger: punishes enemies separated from their group. Excellent at finishing off fleeing targets; struggles if caught at close range.",
        system);

    await PersistHero(artemis, artemisLore, ct);

    // =========================================================
    // VIDAR
    // =========================================================
    var vidar = _heroFactory.Create();

    var vidarStats = _statsFactory.Create();
    vidarStats.Define(
        vidar.Id,
        2450f, 380f,
        84f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.95f, 1.0f,
        0.12f, 2.1f,
        24f, 22f, 0.02f,
        1.0f,
        3.45f, 6.2f,
        0.0f, 6.5f,
        system);

    vidar.Define(
        "Vidar",
        classRanger.Id,
        new HeroAffiliation(archDivine.Id, mythNorse.Id, cultureEurope.Id),
        vidarStats,
        system);

    vidar.SetSkillKit(new HeroSkillKit(
        GetSkill("Avenging Silence").Id,
        GetSkill("Silencing Shaft").Id,
        GetSkill("Iron Stride").Id,
        GetSkill("Crushing Boot").Id,
        GetSkill("Fenrir's Vengeance").Id));

    var vidarLore = CreateLore(vidar.Id,
        "The Silent God",
        "The silent god, son of Odin, destined to avenge his father by breaking the jaws of the wolf Fenrir. He doesn't speak: he aims, and waits.",
        "Endurance ranger: ramps up slowly and becomes unstoppable in the late game. Weak and vulnerable early; must be protected until he comes online.",
        system);

    await PersistHero(vidar, vidarLore, ct);

    // =========================================================
    // RAMA
    // =========================================================
    var rama = _heroFactory.Create();

    var ramaStats = _statsFactory.Create();
    ramaStats.Define(
        rama.Id,
        2350f, 420f,
        78f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.15f, 1.0f,
        0.15f, 2.0f,
        22f, 21f, 0.0f,
        1.0f,
        3.5f, 6.0f,
        0.05f, 7.5f,
        system);

    rama.Define(
        "Rama",
        classRanger.Id,
        new HeroAffiliation(archDemigod.Id, mythHindu.Id, cultureAsia.Id),
        ramaStats,
        system);

    rama.SetSkillKit(new HeroSkillKit(
        GetSkill("Discipline").Id,
        GetSkill("Arrow of Dharma").Id,
        GetSkill("Prince's Stride").Id,
        GetSkill("Chained Shot").Id,
        GetSkill("Brahmastra").Id));

    var ramaLore = CreateLore(rama.Id,
        "The Perfect Archer",
        "Divine prince, the perfect archer of the Ramayana, an incarnation of Vishnu. His discipline is absolute: every arrow leaves at exactly the right moment.",
        "Cadence ranger: sustained, steady damage without downtime. Excellent in prolonged team fights; suffers if constantly interrupted or displaced.",
        system);

    await PersistHero(rama, ramaLore, ct);

    await _uow.CommitAsync(ct);
  }

  private async Task PersistHero(Hero hero, HeroLore lore, CancellationToken ct)
  {
    await _heroes.AddAsync(hero, ct);
    await _loreRepo.AddAsync(lore, ct);
  }

  private HeroLore CreateLore(Guid heroId, string title, string description, string background, string system)
  {
    var lore = _loreFactory.Create();
    lore.Define(heroId, title, description, background, system);
    return lore;
  }
}