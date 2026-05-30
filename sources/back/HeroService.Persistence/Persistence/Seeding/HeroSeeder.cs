using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
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

public sealed class HeroSeeder: ISeeder2
{
  public int Order => 4;
  private readonly IEntityFactory<Guid, Hero> _heroFactory;
  private readonly IEntityFactory<Guid, HeroBaseStats> _statsFactory;
  private readonly IEntityFactory<Guid, HeroLore> _loreFactory;

  private readonly IEntityRepository<Hero, Guid> _heroes;
  private readonly IEntityRepository<HeroBaseStats, Guid> _statsRepo;
  private readonly IEntityRepository<HeroLore, Guid> _loreRepo;

  private readonly IHeroClassRepository _classes;
  private readonly IMythologyRepository _mythologies;
  private readonly IArchetypeRepository _archetypes;
  private readonly ICultureRepository _cultures;

  private readonly ISkillRepository _skills;
  private readonly IUnitOfWork _uow;

  public HeroSeeder(
      IEntityFactory<Guid, Hero> heroFactory,
      IEntityFactory<Guid, HeroBaseStats> statsFactory,
      IEntityFactory<Guid, HeroLore> loreFactory,
      IEntityRepository<Hero, Guid> heroes,
      IEntityRepository<HeroBaseStats, Guid> statsRepo,
      IEntityRepository<HeroLore, Guid> loreRepo,
      IHeroClassRepository classes,
      IMythologyRepository mythologies,
      IArchetypeRepository archetypes,
      ICultureRepository cultures,
      ISkillRepository skills,
      IUnitOfWork uow)
  {
    _heroFactory = heroFactory;
    _statsFactory = statsFactory;
    _loreFactory = loreFactory;
    _heroes = heroes;
    _statsRepo = statsRepo;
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

    var system = "seed-system";

    // =========================================================
    // LOAD REFERENCE DATA (STRICT + FAIL FAST)
    // =========================================================

    var warrior = await _classes.GetByNameAsync("Warrior", ct)
        ?? throw new InvalidOperationException("Missing HeroClass: Warrior");

    var tank = await _classes.GetByNameAsync("Tank", ct)
        ?? throw new InvalidOperationException("Missing HeroClass: Tank");

    var greek = await _mythologies.GetByNameAsync("Greek", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Greek");

    var norse = await _mythologies.GetByNameAsync("Norse", ct)
        ?? throw new InvalidOperationException("Missing Mythology: Norse");

    var bruiser = await _archetypes.GetByNameAsync("Bruiser", ct)
        ?? throw new InvalidOperationException("Missing Archetype: Bruiser");

    var divine = await _archetypes.GetByNameAsync("Divine", ct)
        ?? throw new InvalidOperationException("Missing Archetype: Divine");

    var europe = await _cultures.GetByNameAsync("Europe", ct)
        ?? throw new InvalidOperationException("Missing Culture: Europe");

    // =========================================================
    // LOAD SKILLS (STRICT)
    // =========================================================

    Skill GetSkill(string name) =>
        _skills.GetByNameAsync(name, ct).Result
        ?? throw new InvalidOperationException($"Missing Skill: {name}");

    var mjolnir = GetSkill("Mjolnir Strike");
    var thunderLeap = GetSkill("Thunder Leap");
    var stormAura = GetSkill("Storm Aura");
    var lightningChain = GetSkill("Lightning Chain");
    var thorUlt = GetSkill("God of Thunder");

    var lionsMight = GetSkill("Lion's Might");
    var hydraStrike = GetSkill("Hydra Strike");
    var titanGrip = GetSkill("Titan Grip");
    var laborsRush = GetSkill("Labors Rush");
    var heraUlt = GetSkill("Divine Endurance");

    // =========================================================
    // THOR
    // =========================================================

    var thor = _heroFactory.Create();

    var thorStats = _statsFactory.Create();
    thorStats.Define(thor.Id,
        700, 200, 85, 40,
        1.0f, 1.0f,
        0.05f, 1.5f,
        30, 25, 0,
        1.0f,
        5, 1.0f,
        0.05f, 1.0f,
        system);

    thor.Define(
        "Thor",
        warrior.Id,
        new HeroAffiliation(divine, norse, europe),
        thorStats,
        system);

    thor.SetSkillKit(new HeroSkillKit(
        stormAura.Id,
        mjolnir.Id,
        lightningChain.Id,
        thunderLeap.Id,
        thorUlt.Id));

    await PersistHero(thor, thorStats,
        CreateLore(thor.Id,
            "God of Thunder",
            "A relentless divine warrior wielding storm power.",
            "Born of Asgard, Thor embodies raw storm fury and protection of realms.",
            system),
        ct);

    // =========================================================
    // HERAKLES
    // =========================================================

    var herakles = _heroFactory.Create();

    var heraklesStats = _statsFactory.Create();
    heraklesStats.Define(herakles.Id,
        900, 150, 95, 20,
        0.9f, 1.0f,
        0.08f, 2.0f,
        40, 35, 0.05f,
        1.3f,
        4, 1.0f,
        0.03f, 1.2f,
        system);

    herakles.Define(
        "Herakles",
        tank.Id,
        new HeroAffiliation(bruiser, greek, europe),
        heraklesStats,
        system);

    herakles.SetSkillKit(new HeroSkillKit(
        lionsMight.Id,
        hydraStrike.Id,
        titanGrip.Id,
        laborsRush.Id,
        heraUlt.Id));

    await PersistHero(herakles, heraklesStats,
        CreateLore(herakles.Id,
            "The Labors of a Demigod",
            "A relentless force of endurance and mythic strength.",
            "Herakles walks the path of divine trials, embodying resilience beyond mortal limits.",
            system),
        ct);

    await _uow.CommitAsync(ct);
  }

  private async Task PersistHero(Hero hero, HeroBaseStats stats, HeroLore lore, CancellationToken ct)
  {
    await _heroes.AddAsync(hero, ct);
    await _statsRepo.AddAsync(stats, ct);
    await _loreRepo.AddAsync(lore, ct);
  }

  private HeroLore CreateLore(Guid heroId, string title, string desc, string bg, string system)
  {
    var lore = _loreFactory.Create();
    lore.Define(heroId, title, desc, bg, system);
    return lore;
  }
}