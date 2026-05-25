using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Context;
using HeroService.Application.Commands.Heroes.Services;
using HeroService.Contracts.DTOs.Requests;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Core.Skills;
using HeroService.Domain.Heroes.Skills;

public sealed class HeroCreationService : IHeroCreationService
{
  private readonly IEntityFactory<Guid, Hero> _heroFactory;
  private readonly IEntityFactory<Guid, HeroBaseStats> _baseStatsFactory;
  private readonly IEntityFactory<Guid, HeroLore> _loreFactory;

  private readonly IEntityRepository<Hero, Guid> _heroes;
  private readonly IEntityRepository<HeroBaseStats, Guid> _baseStatsRepository;
  private readonly IEntityRepository<HeroLore, Guid> _loreRepository;

  private readonly IMythologyRepository _mythologies;
  private readonly IHeroClassRepository _heroClasses;
  private readonly IArchetypeRepository _archetypes;
  private readonly ICultureRepository _cultures;

  private readonly ISkillRepository _skills;

  public HeroCreationService(
      IEntityFactory<Guid, Hero> heroFactory,
      IEntityFactory<Guid, HeroBaseStats> baseStatsFactory,
      IEntityFactory<Guid, HeroLore> loreFactory,

      IEntityRepository<Hero, Guid> heroes,
      IEntityRepository<HeroBaseStats, Guid> baseStatsRepository,
      IEntityRepository<HeroLore, Guid> loreRepository,

      IMythologyRepository mythologies,
      IHeroClassRepository heroClasses,
      IArchetypeRepository archetypes,
      ICultureRepository cultures,
      ISkillRepository skills)
  {
    _heroFactory = heroFactory;
    _baseStatsFactory = baseStatsFactory;
    _loreFactory = loreFactory;

    _heroes = heroes;
    _baseStatsRepository = baseStatsRepository;
    _loreRepository = loreRepository;

    _mythologies = mythologies;
    _heroClasses = heroClasses;
    _archetypes = archetypes;
    _cultures = cultures;

    _skills = skills;
  }

  public async Task<Guid> CreateAsync(
      HeroCreateRequest request,
      CancellationToken cancellationToken)
  {
    var userId = MediatorContext.Current.UserId ?? "system";

    // =========================================
    // Resolve reference data
    // =========================================

    var mythology = await _mythologies.GetByNameAsync(request.Mythology, cancellationToken)
        ?? throw new InvalidOperationException($"Mythology '{request.Mythology}' not found.");

    var heroClass = await _heroClasses.GetByNameAsync(request.HeroClass, cancellationToken)
        ?? throw new InvalidOperationException($"HeroClass '{request.HeroClass}' not found.");

    var archetype = await _archetypes.GetByNameAsync(request.Archetype, cancellationToken)
        ?? throw new InvalidOperationException($"Archetype '{request.Archetype}' not found.");

    var culture = await _cultures.GetByNameAsync(request.Culture, cancellationToken)
        ?? throw new InvalidOperationException($"Culture '{request.Culture}' not found.");

    // =========================================
    // Create aggregate root
    // =========================================

    var hero = _heroFactory.Create();

    // =========================================
    // Affiliation
    // =========================================

    var affiliation = new HeroAffiliation(
        archetype,
        mythology,
        culture
    );

    // =========================================
    // Base Stats
    // =========================================

    var baseStats = _baseStatsFactory.Create();

    baseStats.Define(
        hero.Id,
        request.BaseStats.BaseHealth,
        request.BaseStats.BaseMana,
        request.BaseStats.BaseAttackDamage,
        request.BaseStats.BaseAbilityPower,
        request.BaseStats.BaseAttackSpeed,
        request.BaseStats.BaseCritChance,
        request.BaseStats.BaseCritDamageMultiplier,
        request.BaseStats.BaseArmor,
        request.BaseStats.BaseMagicResistance,
        request.BaseStats.BaseDamageReduction,
        request.BaseStats.BaseShieldStrengthMultiplier,
        request.BaseStats.BaseMovementSpeed,
        request.BaseStats.BaseAttackRange,
        request.BaseStats.BaseCastSpeed,
        request.BaseStats.BaseCooldownReduction,
        request.BaseStats.BaseResourceRegeneration,
        userId
    );

    // =========================================
    // Lore
    // =========================================

    var lore = _loreFactory.Create();

    lore.Define(
        hero.Id,
        request.Lore.Title,
        request.Lore.Description,
        request.Lore.BackgroundStory,
        userId
    );

    // =========================================
    // Skills
    // =========================================

    var passiveSkill = await _skills.GetByNameAsync(request.SkillKit.PassiveSkill, cancellationToken)
        ?? throw new InvalidOperationException($"Skill '{request.SkillKit.PassiveSkill}' not found.");

    var primarySkill = await _skills.GetByNameAsync(request.SkillKit.PrimarySkill, cancellationToken)
        ?? throw new InvalidOperationException($"Skill '{request.SkillKit.PrimarySkill}' not found.");

    var secondarySkill = await _skills.GetByNameAsync(request.SkillKit.SecondarySkill, cancellationToken)
        ?? throw new InvalidOperationException($"Skill '{request.SkillKit.SecondarySkill}' not found.");

    var tertiarySkill = await _skills.GetByNameAsync(request.SkillKit.TertiarySkill, cancellationToken)
        ?? throw new InvalidOperationException($"Skill '{request.SkillKit.TertiarySkill}' not found.");

    var ultimateSkill = await _skills.GetByNameAsync(request.SkillKit.UltimateSkill, cancellationToken)
        ?? throw new InvalidOperationException($"Skill '{request.SkillKit.UltimateSkill}' not found.");

    var skillKit = new HeroSkillKit(
        passiveSkill.Id,
        primarySkill.Id,
        secondarySkill.Id,
        tertiarySkill.Id,
        ultimateSkill.Id
    );

    // =========================================
    // Initialize Hero
    // =========================================

    hero.Define(
        request.Name,
        heroClass.Id,
        affiliation,
        baseStats,
        userId
    );

    hero.SetSkillKit(skillKit);

    // =========================================
    // Persist all canonical entities
    // =========================================

    await _heroes.AddAsync(hero, cancellationToken);

    await _baseStatsRepository.AddAsync(baseStats, cancellationToken);

    await _loreRepository.AddAsync(lore, cancellationToken);

    return hero.Id;
  }
}