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
/// Seeds the full 30-hero roster (5 per class x 6 classes), translated and
/// mapped from "codex_heros_v17".
///
/// NOTE: HeroBaseStats.Define requires baseIgnoreEnemyDefense (0.0-1.0), which
/// the codex does not provide (its 16-stat table predates this field). Every
/// hero defaults to 0.0 below -- flag for design review, not an invented
/// balance choice.
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
    var mythCeltic = await _mythologies.GetByNameAsync("Celtic", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Celtic");
    var mythChinese = await _mythologies.GetByNameAsync("Chinese", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Chinese");
    var mythEgyptian = await _mythologies.GetByNameAsync("Egyptian", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Egyptian");
    var mythGreek = await _mythologies.GetByNameAsync("Greek", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Greek");
    var mythHindu = await _mythologies.GetByNameAsync("Hindu", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Hindu");
    var mythJapanese = await _mythologies.GetByNameAsync("Japanese", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Japanese");
    var mythMaya = await _mythologies.GetByNameAsync("Maya", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Maya");
    var mythMesopotamian = await _mythologies.GetByNameAsync("Mesopotamian", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Mesopotamian");
    var mythNorse = await _mythologies.GetByNameAsync("Norse", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Norse");
    var mythYoruba = await _mythologies.GetByNameAsync("Yoruba", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Yoruba");

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
    var cultureAmericas = await _cultures.GetByNameAsync("Americas", ct)
        ?? throw new InvalidOperationException("Missing Culture: Americas");
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

    thor.Define(
        "Thor",
        classWarrior.Id,
        new HeroAffiliation(archDivine.Id, mythNorse.Id, cultureEurope.Id),
        thorStats,
        system);

    thor.SetSkillKit(new HeroSkillKit(
        GetSkill("Static Charge").Id,
        GetSkill("Thunder Strike").Id,
        GetSkill("Hammer's Call").Id,
        GetSkill("Mjolnir Blows").Id,
        GetSkill("Asgard's Wrath").Id));

    var thorLore = CreateLore(thor.Id,
        "Defender of Midgard",
        "God of thunder, son of Odin, defender of Midgard. His hammer Mjolnir commands lightning and always returns to his hand.",
        "Melee initiator: engages from range with his hammer, stuns, then locks down the area. Rewarded for landing Stun into Damage in a short window.",
        system);

    await PersistHero(thor, thorLore, ct);

    // =========================================================
    // ARES
    // =========================================================
    var ares = _heroFactory.Create();

    var aresStats = _statsFactory.Create();
    aresStats.Define(
        ares.Id,
        2800f, 380f,
        83f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.95f, 1.0f,
        0.08f, 1.8f,
        29f, 25f, 0.02f,
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
        GetSkill("Spear Blows").Id,
        GetSkill("War's Fury").Id));

    var aresLore = CreateLore(ares.Id,
        "Lord of Carnage",
        "God of war and carnage, father of Terror (Phobos) and Fear. He revels in the din of battle.",
        "Sustain bruiser: heals off his own hits, terrifies groups, and enters a rage. Rewarded for three consecutive damage hits.",
        system);

    await PersistHero(ares, aresLore, ct);

    // =========================================================
    // GUAN YU
    // =========================================================
    var guanYu = _heroFactory.Create();

    var guanYuStats = _statsFactory.Create();
    guanYuStats.Define(
        guanYu.Id,
        2900f, 380f,
        73f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.88f, 1.0f,
        0.06f, 1.75f,
        32f, 28f, 0.03f,
        1.05f,
        3.55f, 1.8f,
        0.0f, 7.5f,
        system);

    guanYu.Define(
        "Guan Yu",
        classWarrior.Id,
        new HeroAffiliation(archDivine.Id, mythChinese.Id, cultureAsia.Id),
        guanYuStats,
        system);

    guanYu.SetSkillKit(new HeroSkillKit(
        GetSkill("Unshakeable Loyalty").Id,
        GetSkill("Green Dragon Blade").Id,
        GetSkill("Red Hare Charge").Id,
        GetSkill("Halberd Sweep").Id,
        GetSkill("Oath of the Three Brothers").Id));

    var guanYuLore = CreateLore(guanYu.Id,
        "The Loyal General",
        "Legendary deified general, symbol of loyalty and righteousness, wielding the Green Dragon halberd and riding Red Hare.",
        "Tenacious frontline fighter: the lower his health, the more he resists; charges on horseback. Rewarded for landing Stun into Damage in a short window.",
        system);

    await PersistHero(guanYu, guanYuLore, ct);

    // =========================================================
    // OGUN
    // =========================================================
    var ogun = _heroFactory.Create();

    var ogunStats = _statsFactory.Create();
    ogunStats.Define(
        ogun.Id,
        2820f, 390f,
        82f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.9f, 1.0f,
        0.07f, 1.78f,
        31f, 27f, 0.025f,
        1.0f,
        3.58f, 1.8f,
        0.0f, 7.2f,
        system);

    ogun.Define(
        "Ogun",
        classWarrior.Id,
        new HeroAffiliation(archDivine.Id, mythYoruba.Id, cultureAfrica.Id),
        ogunStats,
        system);

    ogun.SetSkillKit(new HeroSkillKit(
        GetSkill("Living Metal").Id,
        GetSkill("Iron Edge").Id,
        GetSkill("Blacksmith's Stride").Id,
        GetSkill("Machete Blows").Id,
        GetSkill("Forge's Wrath").Id));

    var ogunLore = CreateLore(ogun.Id,
        "The Iron Orisha",
        "Orisha of iron, war, and the forge. He opens paths and arms heroes; his metal never dulls.",
        "Attrition fighter: every hit wears down armor, then breaks it in an area. Rewarded for landing Debuff into Damage in a short window.",
        system);

    await PersistHero(ogun, ogunLore, ct);

    // =========================================================
    // SUSANOO
    // =========================================================
    var susanoo = _heroFactory.Create();

    var susanooStats = _statsFactory.Create();
    susanooStats.Define(
        susanoo.Id,
        2700f, 410f,
        78f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.0f, 1.05f,
        0.1f, 1.8f,
        28f, 26f, 0.0f,
        1.0f,
        3.68f, 1.9f,
        0.05f, 7.4f,
        system);

    susanoo.Define(
        "Susanoo",
        classWarrior.Id,
        new HeroAffiliation(archDivine.Id, mythJapanese.Id, cultureAsia.Id),
        susanooStats,
        system);

    susanoo.SetSkillKit(new HeroSkillKit(
        GetSkill("Breath of Battle").Id,
        GetSkill("Gale Blade").Id,
        GetSkill("Typhoon Step").Id,
        GetSkill("Kusanagi Strikes").Id,
        GetSkill("Orochi").Id));

    var susanooLore = CreateLore(susanoo.Id,
        "Storm-Slayer of Orochi",
        "God of storms and the sea, brother of Amaterasu. He slew the eight-headed serpent Yamata-no-Orochi and drew the sword Kusanagi from it.",
        "Mobile duelist: chains wind strikes and summons the great serpent. Rewarded for three consecutive damage hits.",
        system);

    await PersistHero(susanoo, susanooLore, ct);

    // =========================================================
    // LOKI
    // =========================================================
    var loki = _heroFactory.Create();

    var lokiStats = _statsFactory.Create();
    lokiStats.Define(
        loki.Id,
        2380f, 370f,
        92f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.08f, 1.0f,
        0.15f, 2.0f,
        22f, 20f, 0.0f,
        1.0f,
        3.78f, 1.8f,
        0.05f, 6.4f,
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
        GetSkill("Sneaking Daggers").Id,
        GetSkill("Trickster's Verdict").Id));

    var lokiLore = CreateLore(loki.Id,
        "The Trickster God",
        "God of mischief and shapeshifting, father of Fenrir, Hel, and Jormungandr. No one ever truly knows where he stands.",
        "Stealth assassin: vanishes, poisons, then teleports onto his prey to finish them. Rewarded for landing Debuff into Damage in a short window.",
        system);

    await PersistHero(loki, lokiLore, ct);

    // =========================================================
    // SET
    // =========================================================
    var set = _heroFactory.Create();

    var setStats = _statsFactory.Create();
    setStats.Define(
        set.Id,
        2420f, 390f,
        92f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.02f, 1.0f,
        0.12f, 1.9f,
        23f, 22f, 0.0f,
        1.0f,
        3.72f, 1.9f,
        0.05f, 6.6f,
        system);

    set.Define(
        "Set",
        classAssassin.Id,
        new HeroAffiliation(archDivine.Id, mythEgyptian.Id, cultureAfrica.Id),
        setStats,
        system);

    set.SetSkillKit(new HeroSkillKit(
        GetSkill("Desert's Blood").Id,
        GetSkill("Sandstorm").Id,
        GetSkill("Chaos Mist").Id,
        GetSkill("Khopesh Blows").Id,
        GetSkill("Set's Judgment").Id));

    var setLore = CreateLore(set.Id,
        "Lord of the Storm",
        "God of chaos, storms, and the desert, killer of Osiris. The sand obeys his fury.",
        "Poison assassin: marks a target, weakens it, then executes it in the storm. Rewarded for landing Debuff into Damage in a short window.",
        system);

    await PersistHero(set, setLore, ct);

    // =========================================================
    // KALI
    // =========================================================
    var kali = _heroFactory.Create();

    var kaliStats = _statsFactory.Create();
    kaliStats.Define(
        kali.Id,
        2400f, 380f,
        92f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.05f, 1.0f,
        0.14f, 1.95f,
        22f, 21f, 0.0f,
        1.0f,
        3.75f, 1.8f,
        0.05f, 6.5f,
        system);

    kali.Define(
        "Kali",
        classAssassin.Id,
        new HeroAffiliation(archDivine.Id, mythHindu.Id, cultureAsia.Id),
        kaliStats,
        system);

    kali.SetSkillKit(new HeroSkillKit(
        GetSkill("Blood Rapture").Id,
        GetSkill("Dance of Blades").Id,
        GetSkill("Goddess's Leap").Id,
        GetSkill("Many Blades").Id,
        GetSkill("Destructive Fury").Id));

    var kaliLore = CreateLore(kali.Id,
        "Destroyer of Illusion",
        "Goddess of destruction and time, terrible dancer who tramples illusion. Her fury purifies as much as it destroys.",
        "Melee assassin: whirls through the fragile and hounds a single target. Rewarded for four consecutive damage hits.",
        system);

    await PersistHero(kali, kaliLore, ct);

    // =========================================================
    // CAMAZOTZ
    // =========================================================
    var camazotz = _heroFactory.Create();

    var camazotzStats = _statsFactory.Create();
    camazotzStats.Define(
        camazotz.Id,
        2450f, 370f,
        86f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.04f, 1.0f,
        0.12f, 1.9f,
        23f, 21f, 0.0f,
        1.0f,
        3.76f, 1.8f,
        0.05f, 6.5f,
        system);

    camazotz.Define(
        "Camazotz",
        classAssassin.Id,
        new HeroAffiliation(archDivine.Id, mythMaya.Id, cultureAmericas.Id),
        camazotzStats,
        system);

    camazotz.SetSkillKit(new HeroSkillKit(
        GetSkill("Nocturnal Flight").Id,
        GetSkill("Blood Swarm").Id,
        GetSkill("Raptor Dive").Id,
        GetSkill("Claws and Fangs").Id,
        GetSkill("Feast of Xibalba").Id));

    var camazotzLore = CreateLore(camazotz.Id,
        "Bat Lord of Xibalba",
        "Bat god of the night and death, guardian of the caves of Xibalba. He descends on his prey in the dark.",
        "Night assassin: dives on an isolated target, drains it, and heals off it. Rewarded for four consecutive damage hits.",
        system);

    await PersistHero(camazotz, camazotzLore, ct);

    // =========================================================
    // NYX
    // =========================================================
    var nyx = _heroFactory.Create();

    var nyxStats = _statsFactory.Create();
    nyxStats.Define(
        nyx.Id,
        2400f, 390f,
        92f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.03f, 1.02f,
        0.12f, 1.9f,
        22f, 21f, 0.0f,
        1.0f,
        3.8f, 1.8f,
        0.08f, 6.4f,
        system);

    nyx.Define(
        "Nyx",
        classAssassin.Id,
        new HeroAffiliation(archDivine.Id, mythGreek.Id, cultureMediterranean.Id),
        nyxStats,
        system);

    nyx.SetSkillKit(new HeroSkillKit(
        GetSkill("Nightfall Veil").Id,
        GetSkill("Blade of Darkness").Id,
        GetSkill("Shadow Step").Id,
        GetSkill("Shadow Blades").Id,
        GetSkill("Everlasting Night").Id));

    var nyxLore = CreateLore(nyx.Id,
        "Primordial Goddess of Night",
        "Primordial goddess of Night, so ancient even Zeus fears her. Darkness is her domain.",
        "Control assassin: roots a target in shadow then strikes, and blinds whole areas. Rewarded for landing Root into Damage in a short window.",
        system);

    await PersistHero(nyx, nyxLore, ct);

    // =========================================================
    // ZEUS
    // =========================================================
    var zeus = _heroFactory.Create();

    var zeusStats = _statsFactory.Create();
    zeusStats.Define(
        zeus.Id,
        2185f, 520f,
        52f, 95f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.15f,
        0.0f, 1.8f,
        20f, 26f, 0.0f,
        1.0f,
        3.5f, 5.5f,
        0.05f, 8.0f,
        system);

    zeus.Define(
        "Zeus",
        classMage.Id,
        new HeroAffiliation(archDivine.Id, mythGreek.Id, cultureMediterranean.Id),
        zeusStats,
        system);

    zeus.SetSkillKit(new HeroSkillKit(
        GetSkill("Celestial Charge").Id,
        GetSkill("Targeted Bolt").Id,
        GetSkill("Olympus Gale").Id,
        GetSkill("Sparks").Id,
        GetSkill("Sky's Wrath").Id));

    var zeusLore = CreateLore(zeus.Id,
        "King of Olympus",
        "King of Olympus, god of the sky and lightning. His wrath splits the heavens, and no mortal survives it.",
        "Zone mage: strikes from range, chains lightning, and pins down groups. Rewarded for hitting 3 different effect types within 8 seconds.",
        system);

    await PersistHero(zeus, zeusLore, ct);

    // =========================================================
    // RA
    // =========================================================
    var ra = _heroFactory.Create();

    var raStats = _statsFactory.Create();
    raStats.Define(
        ra.Id,
        2480f, 520f,
        52f, 83f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.15f,
        0.0f, 1.8f,
        20f, 26f, 0.0f,
        1.0f,
        3.5f, 5.5f,
        0.05f, 8.0f,
        system);

    ra.Define(
        "Ra",
        classMage.Id,
        new HeroAffiliation(archDivine.Id, mythEgyptian.Id, cultureAfrica.Id),
        raStats,
        system);

    ra.SetSkillKit(new HeroSkillKit(
        GetSkill("Solar Fire").Id,
        GetSkill("Solar Ray").Id,
        GetSkill("Ascension").Id,
        GetSkill("Rays").Id,
        GetSkill("Noon Judgment").Id));

    var raLore = CreateLore(ra.Id,
        "The Sun King",
        "God of the sun, creator and king of the gods, who crosses the sky by day and the underworld by night.",
        "Burn mage: sets lines of solar fire and briefly heals his allies. Rewarded for hitting 3 different effect types within 8 seconds.",
        system);

    await PersistHero(ra, raLore, ct);

    // =========================================================
    // AGNI
    // =========================================================
    var agni = _heroFactory.Create();

    var agniStats = _statsFactory.Create();
    agniStats.Define(
        agni.Id,
        2300f, 520f,
        52f, 88f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.21f,
        0.0f, 1.8f,
        20f, 26f, 0.0f,
        1.0f,
        3.5f, 5.5f,
        0.05f, 8.0f,
        system);

    agni.Define(
        "Agni",
        classMage.Id,
        new HeroAffiliation(archDivine.Id, mythHindu.Id, cultureAsia.Id),
        agniStats,
        system);

    agni.SetSkillKit(new HeroSkillKit(
        GetSkill("Combustion").Id,
        GetSkill("Flame Javelin").Id,
        GetSkill("Blazing Trail").Id,
        GetSkill("Ember Spit").Id,
        GetSkill("Pillar of Fire").Id));

    var agniLore = CreateLore(agni.Id,
        "The Fire Messenger",
        "God of fire, messenger between men and gods, present in every flame and every sacrifice.",
        "Combustion mage: builds up embers and triggers delayed explosions. Rewarded for four consecutive damage hits.",
        system);

    await PersistHero(agni, agniLore, ct);

    // =========================================================
    // RAIJIN
    // =========================================================
    var raijin = _heroFactory.Create();

    var raijinStats = _statsFactory.Create();
    raijinStats.Define(
        raijin.Id,
        2300f, 490f,
        52f, 93f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.15f,
        0.0f, 1.8f,
        20f, 26f, 0.0f,
        1.0f,
        3.5f, 5.5f,
        0.05f, 8.0f,
        system);

    raijin.Define(
        "Raijin",
        classMage.Id,
        new HeroAffiliation(archDivine.Id, mythJapanese.Id, cultureAsia.Id),
        raijinStats,
        system);

    raijin.SetSkillKit(new HeroSkillKit(
        GetSkill("Storm's Rhythm").Id,
        GetSkill("Thunderclap").Id,
        GetSkill("Thunder Step").Id,
        GetSkill("Drum Rolls").Id,
        GetSkill("Drum Fury").Id));

    var raijinLore = CreateLore(raijin.Id,
        "Drummer of the Storm",
        "God of thunder and lightning, who beats his drums to make the storm roll across the sky.",
        "Tempo mage: strikes to the drum, stuns, and accelerates his own spells. Rewarded for landing Stun into Damage in a short window.",
        system);

    await PersistHero(raijin, raijinLore, ct);

    // =========================================================
    // MARDUK
    // =========================================================
    var marduk = _heroFactory.Create();

    var mardukStats = _statsFactory.Create();
    mardukStats.Define(
        marduk.Id,
        2300f, 560f,
        52f, 88f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.15f,
        0.0f, 1.8f,
        20f, 26f, 0.0f,
        1.0f,
        3.5f, 5.5f,
        0.06f, 8.0f,
        system);

    marduk.Define(
        "Marduk",
        classMage.Id,
        new HeroAffiliation(archDivine.Id, mythMesopotamian.Id, cultureAsia.Id),
        mardukStats,
        system);

    marduk.SetSkillKit(new HeroSkillKit(
        GetSkill("Tablets of Destiny").Id,
        GetSkill("Net of the Winds").Id,
        GetSkill("Primordial Breath").Id,
        GetSkill("Shards of Power").Id,
        GetSkill("Seal of Tiamat").Id));

    var mardukLore = CreateLore(marduk.Id,
        "King of Babylon",
        "God-king of Babylon, slayer of the primordial dragon Tiamat, from whom he shaped the world. He bears the net of the winds and the tablets of destiny.",
        "Control mage: chains restraints and strikes immobilized targets. Rewarded for landing Root into Damage in a short window.",
        system);

    await PersistHero(marduk, mardukLore, ct);

    // =========================================================
    // HERAKLES
    // =========================================================
    var herakles = _heroFactory.Create();

    var heraklesStats = _statsFactory.Create();
    heraklesStats.Define(
        herakles.Id,
        3200f, 400f,
        69f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.0f,
        0.0f, 1.8f,
        40f, 38f, 0.06f,
        1.35f,
        3.5f, 1.8f,
        0.0f, 7.5f,
        system);

    herakles.Define(
        "Herakles",
        classTank.Id,
        new HeroAffiliation(archDemigod.Id, mythGreek.Id, cultureMediterranean.Id),
        heraklesStats,
        system);

    herakles.SetSkillKit(new HeroSkillKit(
        GetSkill("Hero's Endurance").Id,
        GetSkill("Nemean Grip").Id,
        GetSkill("Lion's Charge").Id,
        GetSkill("Club Blows").Id,
        GetSkill("Twelve Labors").Id));

    var heraklesLore = CreateLore(herakles.Id,
        "The Twelve Labors",
        "Deified hero of the twelve labors, gifted with superhuman strength. He wears the hide of the Nemean Lion, which no weapon can pierce.",
        "Initiator: grabs a target from range, pulls it in, then hardens in melee. Rewarded for landing Pull into Damage in a short window.",
        system);

    await PersistHero(herakles, heraklesLore, ct);

    // =========================================================
    // YMIR
    // =========================================================
    var ymir = _heroFactory.Create();

    var ymirStats = _statsFactory.Create();
    ymirStats.Define(
        ymir.Id,
        3520f, 400f,
        65f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.0f,
        0.0f, 1.8f,
        45f, 38f, 0.06f,
        1.35f,
        3.2f, 1.8f,
        0.0f, 7.5f,
        system);

    ymir.Define(
        "Ymir",
        classTank.Id,
        new HeroAffiliation(archDivine.Id, mythNorse.Id, cultureEurope.Id),
        ymirStats,
        system);

    ymir.SetSkillKit(new HeroSkillKit(
        GetSkill("Flesh of Ice").Id,
        GetSkill("Glacial Shard").Id,
        GetSkill("Wall of Frost").Id,
        GetSkill("Frost Fists").Id,
        GetSkill("Grip of Frost").Id));

    var ymirLore = CreateLore(ymir.Id,
        "The Primordial Frost Giant",
        "Primordial frost giant, born from the melting of Ginnungagap, whose body shaped the world. Absolute cold is his flesh.",
        "Ice wall: raises walls, freezes enemies, and protects the backline. Rewarded for landing Slow into Damage in a short window.",
        system);

    await PersistHero(ymir, ymirLore, ct);

    // =========================================================
    // GEB
    // =========================================================
    var geb = _heroFactory.Create();

    var gebStats = _statsFactory.Create();
    gebStats.Define(
        geb.Id,
        3200f, 400f,
        65f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.0f,
        0.0f, 1.8f,
        46f, 38f, 0.06f,
        1.35f,
        3.3f, 1.8f,
        0.0f, 7.5f,
        system);

    geb.Define(
        "Geb",
        classTank.Id,
        new HeroAffiliation(archDivine.Id, mythEgyptian.Id, cultureAfrica.Id),
        gebStats,
        system);

    geb.SetSkillKit(new HeroSkillKit(
        GetSkill("Stone Skin").Id,
        GetSkill("Telluric Shard").Id,
        GetSkill("Earthen Aegis").Id,
        GetSkill("Stone Fists").Id,
        GetSkill("Cataclysm").Id));

    var gebLore = CreateLore(geb.Id,
        "Lord of the Earth",
        "God of the Earth, whose laughter causes earthquakes. He holds up the sky and shelters the living.",
        "Earthen protector: hurls stone, shakes the ground, and shields an ally. Rewarded for landing Stun into Damage in a short window.",
        system);

    await PersistHero(geb, gebLore, ct);

    // =========================================================
    // KUMBHAKARNA
    // =========================================================
    var kumbhakarna = _heroFactory.Create();

    var kumbhakarnaStats = _statsFactory.Create();
    kumbhakarnaStats.Define(
        kumbhakarna.Id,
        3580f, 400f,
        65f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.78f, 1.0f,
        0.0f, 1.8f,
        42f, 38f, 0.06f,
        1.35f,
        3.5f, 1.8f,
        0.0f, 7.5f,
        system);

    kumbhakarna.Define(
        "Kumbhakarna",
        classTank.Id,
        new HeroAffiliation(archDivine.Id, mythHindu.Id, cultureAsia.Id),
        kumbhakarnaStats,
        system);

    kumbhakarna.SetSkillKit(new HeroSkillKit(
        GetSkill("Giant's Slumber").Id,
        GetSkill("Great Sweep").Id,
        GetSkill("Ponderous Stride").Id,
        GetSkill("Massive Backhand").Id,
        GetSkill("Terrible Awakening").Id));

    var kumbhakarnaLore = CreateLore(kumbhakarna.Id,
        "The Sleeping Giant",
        "Giant of the Ramayana, brother of Ravana, cursed to sleep for six months at a time. Once woken, his strength is irresistible.",
        "Sleeping colossus: the longer he stands still, the harder he hits on waking; sweeps in an area. Rewarded for landing Stun into Damage in a short window.",
        system);

    await PersistHero(kumbhakarna, kumbhakarnaLore, ct);

    // =========================================================
    // GILGAMESH
    // =========================================================
    var gilgamesh = _heroFactory.Create();

    var gilgameshStats = _statsFactory.Create();
    gilgameshStats.Define(
        gilgamesh.Id,
        3200f, 400f,
        68f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.0f,
        0.0f, 1.8f,
        42f, 40f, 0.06f,
        1.35f,
        3.5f, 1.8f,
        0.0f, 7.5f,
        system);

    gilgamesh.Define(
        "Gilgamesh",
        classTank.Id,
        new HeroAffiliation(archDemigod.Id, mythMesopotamian.Id, cultureAsia.Id),
        gilgameshStats,
        system);

    gilgamesh.SetSkillKit(new HeroSkillKit(
        GetSkill("Two-Thirds Divine").Id,
        GetSkill("Celestial Slash").Id,
        GetSkill("Charge of Uruk").Id,
        GetSkill("Royal Blows").Id,
        GetSkill("King's Judgment").Id));

    var gilgameshLore = CreateLore(gilgamesh.Id,
        "King of Uruk",
        "Two-thirds-divine king of Uruk, hero of the oldest epic. In his quest for immortality, he slew the Bull of Heaven.",
        "Warrior-king: engages, taunts enemies onto himself, and protects his line. Rewarded for landing Taunt into Damage in a short window.",
        system);

    await PersistHero(gilgamesh, gilgameshLore, ct);

    // =========================================================
    // FREYJA
    // =========================================================
    var freyja = _heroFactory.Create();

    var freyjaStats = _statsFactory.Create();
    freyjaStats.Define(
        freyja.Id,
        2500f, 500f,
        55f, 55f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.1f,
        0.0f, 1.8f,
        26f, 30f, 0.02f,
        1.5f,
        3.6f, 5.0f,
        0.05f, 9.0f,
        system);

    freyja.Define(
        "Freyja",
        classSupport.Id,
        new HeroAffiliation(archDivine.Id, mythNorse.Id, cultureEurope.Id),
        freyjaStats,
        system);

    freyja.SetSkillKit(new HeroSkillKit(
        GetSkill("Vanir's Favor").Id,
        GetSkill("Valkyries' Blessing").Id,
        GetSkill("Falcon Flight").Id,
        GetSkill("Golden Shards").Id,
        GetSkill("Dawn of Folkvangr").Id));

    var freyjaLore = CreateLore(freyja.Id,
        "Chooser of the Slain",
        "Goddess of love, fertility, and seidr magic, riding a chariot pulled by cats. She wears the necklace Brisingamen and a cloak of hawk feathers.",
        "Enchantress: strengthens and heals allies, slows enemies with a breath. Rewarded for landing Heal into Buff in a short window.",
        system);

    await PersistHero(freyja, freyjaLore, ct);

    // =========================================================
    // ISIS
    // =========================================================
    var isis = _heroFactory.Create();

    var isisStats = _statsFactory.Create();
    isisStats.Define(
        isis.Id,
        2500f, 500f,
        55f, 55f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.1f,
        0.0f, 1.8f,
        25f, 30f, 0.02f,
        1.5f,
        3.6f, 5.0f,
        0.05f, 9.9f,
        system);

    isis.Define(
        "Isis",
        classSupport.Id,
        new HeroAffiliation(archDivine.Id, mythEgyptian.Id, cultureAfrica.Id),
        isisStats,
        system);

    isis.SetSkillKit(new HeroSkillKit(
        GetSkill("Ancestral Magic").Id,
        GetSkill("Healing Wing").Id,
        GetSkill("Veil of Isis").Id,
        GetSkill("Breath of Life").Id,
        GetSkill("Osiris's Resurrection").Id));

    var isisLore = CreateLore(isis.Id,
        "Goddess of Magic and Healing",
        "Goddess of magic, motherhood, and healing, who restored Osiris to life. The greatest sorceress of the pantheon.",
        "Ultimate healer: powerful heals, shields, and a decisive resurrection. Rewarded for three consecutive heals.",
        system);

    await PersistHero(isis, isisLore, ct);

    // =========================================================
    // APHRODITE
    // =========================================================
    var aphrodite = _heroFactory.Create();

    var aphroditeStats = _statsFactory.Create();
    aphroditeStats.Define(
        aphrodite.Id,
        2375f, 500f,
        55f, 55f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.19f,
        0.0f, 1.8f,
        26f, 30f, 0.02f,
        1.5f,
        3.6f, 5.0f,
        0.05f, 9.0f,
        system);

    aphrodite.Define(
        "Aphrodite",
        classSupport.Id,
        new HeroAffiliation(archDivine.Id, mythGreek.Id, cultureMediterranean.Id),
        aphroditeStats,
        system);

    aphrodite.SetSkillKit(new HeroSkillKit(
        GetSkill("Grace").Id,
        GetSkill("Embrace").Id,
        GetSkill("Flight of Doves").Id,
        GetSkill("Ardent Kisses").Id,
        GetSkill("Intoxicating Charm").Id));

    var aphroditeLore = CreateLore(aphrodite.Id,
        "Goddess of Desire",
        "Goddess of love and beauty, born of the sea foam. Her girdle inspires an irresistible desire.",
        "Bond support: links to an ally to heal and strengthen them, charms enemies. Rewarded for hitting Heal into Buff within a short window.",
        system);

    await PersistHero(aphrodite, aphroditeLore, ct);

    // =========================================================
    // GUANYIN
    // =========================================================
    var guanyin = _heroFactory.Create();

    var guanyinStats = _statsFactory.Create();
    guanyinStats.Define(
        guanyin.Id,
        2500f, 500f,
        55f, 55f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.1f,
        0.0f, 1.8f,
        26f, 30f, 0.02f,
        1.6f,
        3.6f, 5.0f,
        0.05f, 9.0f,
        system);

    guanyin.Define(
        "Guanyin",
        classSupport.Id,
        new HeroAffiliation(archDivine.Id, mythChinese.Id, cultureAsia.Id),
        guanyinStats,
        system);

    guanyin.SetSkillKit(new HeroSkillKit(
        GetSkill("Compassion").Id,
        GetSkill("Willow Water").Id,
        GetSkill("Lotus Step").Id,
        GetSkill("Jade Droplets").Id,
        GetSkill("Ocean of Mercy").Id));

    var guanyinLore = CreateLore(guanyin.Id,
        "Bodhisattva of Compassion",
        "Bodhisattva of compassion, who hears the cries of the world. She pours water from her jade vial that washes away all suffering.",
        "Compassionate protector: purifies, heals from range, and pacifies fights. Rewarded for three consecutive heals.",
        system);

    await PersistHero(guanyin, guanyinLore, ct);

    // =========================================================
    // BRIGID
    // =========================================================
    var brigid = _heroFactory.Create();

    var brigidStats = _statsFactory.Create();
    brigidStats.Define(
        brigid.Id,
        2375f, 500f,
        55f, 58f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        0.82f, 1.1f,
        0.0f, 1.8f,
        26f, 30f, 0.02f,
        1.5f,
        3.6f, 5.0f,
        0.05f, 9.0f,
        system);

    brigid.Define(
        "Brigid",
        classSupport.Id,
        new HeroAffiliation(archDivine.Id, mythCeltic.Id, cultureEurope.Id),
        brigidStats,
        system);

    brigid.SetSkillKit(new HeroSkillKit(
        GetSkill("Eternal Flame").Id,
        GetSkill("Forge Shield").Id,
        GetSkill("Ember Breath").Id,
        GetSkill("Sacred Embers").Id,
        GetSkill("Inspiring Blaze").Id));

    var brigidLore = CreateLore(brigid.Id,
        "Flame of the Forge",
        "Celtic goddess of fire, healing, and poetry, patron of blacksmiths. Her flame inspires as much as it protects.",
        "Forge support: flame shields, combat buffs, and area burn. Rewarded for hitting Shield into Buff within a short window.",
        system);

    await PersistHero(brigid, brigidLore, ct);

    // =========================================================
    // ARTEMIS
    // =========================================================
    var artemis = _heroFactory.Create();

    var artemisStats = _statsFactory.Create();
    artemisStats.Define(
        artemis.Id,
        2185f, 400f,
        85f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.17f, 1.0f,
        0.15f, 2.0f,
        22f, 20f, 0.0f,
        1.0f,
        3.6f, 6.0f,
        0.0f, 6.5f,
        system);

    artemis.Define(
        "Artemis",
        classRanger.Id,
        new HeroAffiliation(archDivine.Id, mythGreek.Id, cultureMediterranean.Id),
        artemisStats,
        system);

    artemis.SetSkillKit(new HeroSkillKit(
        GetSkill("Huntress's Eye").Id,
        GetSkill("Piercing Arrow").Id,
        GetSkill("Doe's Leap").Id,
        GetSkill("Lunar Shots").Id,
        GetSkill("Arrow of the Moon").Id));

    var artemisLore = CreateLore(artemis.Id,
        "Goddess of the Hunt",
        "Goddess of the hunt and the moon, sister of Apollo. Her arrows never miss their marked target.",
        "Huntress: marks her prey, tracks it from range, and fells it with a deadly shot. Rewarded for four consecutive damage hits.",
        system);

    await PersistHero(artemis, artemisLore, ct);

    // =========================================================
    // RAMA
    // =========================================================
    var rama = _heroFactory.Create();

    var ramaStats = _statsFactory.Create();
    ramaStats.Define(
        rama.Id,
        2300f, 400f,
        90f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.1f, 1.0f,
        0.15f, 2.0f,
        22f, 20f, 0.0f,
        1.0f,
        3.6f, 6.0f,
        0.0f, 6.5f,
        system);

    rama.Define(
        "Rama",
        classRanger.Id,
        new HeroAffiliation(archDemigod.Id, mythHindu.Id, cultureAsia.Id),
        ramaStats,
        system);

    rama.SetSkillKit(new HeroSkillKit(
        GetSkill("Prince's Precision").Id,
        GetSkill("Blazing Shaft").Id,
        GetSkill("Prince's Step").Id,
        GetSkill("Arrows of Kodanda").Id,
        GetSkill("Brahmastra").Id));

    var ramaLore = CreateLore(rama.Id,
        "The Perfect Archer",
        "Seventh avatar of Vishnu, exiled prince of the Ramayana, archer without equal. His bow Kodanda and Brahma's weapon overthrow demons.",
        "Heroic archer: piercing shots at very long range and a final divine arrow. Rewarded for four consecutive damage hits.",
        system);

    await PersistHero(rama, ramaLore, ct);

    // =========================================================
    // HOUYI
    // =========================================================
    var houyi = _heroFactory.Create();

    var houyiStats = _statsFactory.Create();
    houyiStats.Define(
        houyi.Id,
        2300f, 400f,
        85f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.03f, 1.0f,
        0.15f, 2.0f,
        22f, 20f, 0.0f,
        1.0f,
        3.6f, 6.5f,
        0.0f, 6.5f,
        system);

    houyi.Define(
        "Houyi",
        classRanger.Id,
        new HeroAffiliation(archDivine.Id, mythChinese.Id, cultureAsia.Id),
        houyiStats,
        system);

    houyi.SetSkillKit(new HeroSkillKit(
        GetSkill("Nine Suns").Id,
        GetSkill("Solar Arrow").Id,
        GetSkill("Hunter's Roll").Id,
        GetSkill("Burning Arrows").Id,
        GetSkill("Volley of Ten Suns").Id));

    var houyiLore = CreateLore(houyi.Id,
        "Slayer of Nine Suns",
        "The divine archer who shot down nine of the ten suns to save the Earth from drought. His red bow never misses.",
        "Solar archer: burning shots, fearsome range, and a decisive volley. Rewarded for four consecutive damage hits.",
        system);

    await PersistHero(houyi, houyiLore, ct);

    // =========================================================
    // ULLR
    // =========================================================
    var ullr = _heroFactory.Create();

    var ullrStats = _statsFactory.Create();
    ullrStats.Define(
        ullr.Id,
        2185f, 400f,
        85f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.1f, 1.0f,
        0.15f, 2.0f,
        22f, 20f, 0.0f,
        1.0f,
        3.9f, 6.0f,
        0.0f, 6.5f,
        system);

    ullr.Define(
        "Ullr",
        classRanger.Id,
        new HeroAffiliation(archDivine.Id, mythNorse.Id, cultureEurope.Id),
        ullrStats,
        system);

    ullr.SetSkillKit(new HeroSkillKit(
        GetSkill("Winter's Favor").Id,
        GetSkill("Piercing Shaft").Id,
        GetSkill("Icy Glide").Id,
        GetSkill("Yew Arrows").Id,
        GetSkill("Duelist's Challenge").Id));

    var ullrLore = CreateLore(ullr.Id,
        "God of the Bow and Ski",
        "God of the bow, skiing, and the hunt, invoked in duels. He glides over the snow and never misses his shot.",
        "Snow hunter: alternates ranged bow and melee blades. Rewarded for four consecutive damage hits.",
        system);

    await PersistHero(ullr, ullrLore, ct);

    // =========================================================
    // NEITH
    // =========================================================
    var neith = _heroFactory.Create();

    var neithStats = _statsFactory.Create();
    neithStats.Define(
        neith.Id,
        2300f, 400f,
        85f, 0f,
        0.0f, // baseIgnoreEnemyDefense -- not present in codex, defaulted (see class remarks)
        1.1f, 1.0f,
        0.15f, 2.0f,
        22f, 20f, 0.0f,
        1.0f,
        3.6f, 6.0f,
        0.08f, 6.5f,
        system);

    neith.Define(
        "Neith",
        classRanger.Id,
        new HeroAffiliation(archDivine.Id, mythEgyptian.Id, cultureAfrica.Id),
        neithStats,
        system);

    neith.SetSkillKit(new HeroSkillKit(
        GetSkill("Thread of Fate").Id,
        GetSkill("Hunting Shaft").Id,
        GetSkill("Weaver's Step").Id,
        GetSkill("Woven Arrows").Id,
        GetSkill("Web of the World").Id));

    var neithLore = CreateLore(neith.Id,
        "The Weaver-Huntress",
        "Goddess of the hunt, war, and weaving, creator who wove the world. Her bow and arrows open the way.",
        "Weaver-huntress: entangles from range with her threads and strikes trapped targets. Rewarded for landing Root into Damage in a short window.",
        system);

    await PersistHero(neith, neithLore, ct);

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