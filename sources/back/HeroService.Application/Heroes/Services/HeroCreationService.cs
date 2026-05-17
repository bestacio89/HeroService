using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using HeroService.Application.Heroes.Services;
using HeroService.Contracts.DTOs.Requests;
using HeroService.Contracts.Persistence;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Core.Skills;
using HeroService.Domain.Heroes.Skills;

public sealed class HeroCreationService : IHeroCreationService
{
  private readonly IEntityFactory<Guid, Hero> _factory;
  private readonly IEntityRepository<Hero, Guid> _heroes;

  private readonly IMythologyRepository _mythologies;
  private readonly IHeroClassRepository _heroClasses;
  private readonly IArchetypeRepository _archetypes;
  private readonly ICultureRepository _cultures;

  private readonly ISkillRepository _skills;
  public HeroCreationService(
      IEntityFactory<Guid, Hero> factory,
      IEntityRepository<Hero, Guid> heroes,
      IMythologyRepository mythologies,
      IHeroClassRepository heroClasses,
      IArchetypeRepository archetypes,
      ICultureRepository cultures,
      ISkillRepository skills)
  {
    _factory = factory;
    _heroes = heroes;
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
    // =========================
    // Resolve reference data
    // =========================

    var mythology = await _mythologies.GetByNameAsync(request.Mythology, cancellationToken)
        ?? throw new InvalidOperationException($"Mythology '{request.Mythology}' not found.");

    var heroClass = await _heroClasses.GetByNameAsync(request.HeroClass, cancellationToken)
        ?? throw new InvalidOperationException($"HeroClass '{request.HeroClass}' not found.");

    var archetype = await _archetypes.GetByNameAsync(request.Archetype, cancellationToken)
        ?? throw new InvalidOperationException($"Archetype '{request.Archetype}' not found.");

    var culture = await _cultures.GetByNameAsync(request.Culture, cancellationToken)
        ?? throw new InvalidOperationException($"Culture '{request.Culture}' not found.");

    // =========================
    // Create Hero FIRST
    // =========================

    var hero = _factory.Create();

    // =========================
    // Build Affiliation (FIXED)
    // =========================

    var affiliation = new HeroAffiliation(
        archetype,
        mythology,
        culture
    );

    // =========================
    // Build BaseStats (after Hero exists)
    // =========================

    var baseStats = new HeroBaseStats(
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
        request.BaseStats.HealthScalingPerLevel,
        request.BaseStats.ManaScalingPerLevel,
        request.BaseStats.AttackDamageScalingPerLevel,
        request.BaseStats.AbilityPowerScalingPerLevel
    );

    // =========================
    // SkillKit (needs real request input)
    // =========================

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

    // =========================
    // Initialize aggregate
    // =========================

    hero.Initialize(
        request.Name,
        heroClass.Id,
        affiliation,
        baseStats,
        createdBy: "system"
    );

    hero.SetSkillKit(skillKit);

    // =========================
    // Persist
    // =========================

    await _heroes.AddAsync(hero, cancellationToken);

    return hero.Id;
  }
}