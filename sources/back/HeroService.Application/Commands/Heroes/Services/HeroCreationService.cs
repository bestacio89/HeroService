using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Errors;
using Franz.Common.Mediator.Context;
using HeroService.Application.Commands.Heroes.Services;
using HeroService.Contracts.DTOs.Requests;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Affiliations.Classifications;
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

  private readonly IEntityRepository<MythologyType, Guid> _mythologies;
  private readonly IEntityRepository<HeroClass, Guid> _heroClasses;
  private readonly IEntityRepository<OriginArchetype, Guid> _archetypes;
  private readonly IEntityRepository<OriginCulture, Guid> _cultures;

  private readonly ISkillRepository _skills;

  private readonly IHeroUniquenessValidator _uniquenessValidator;

  public HeroCreationService(
      IEntityFactory<Guid, Hero> heroFactory,
      IEntityFactory<Guid, HeroBaseStats> baseStatsFactory,
      IEntityFactory<Guid, HeroLore> loreFactory,

      IEntityRepository<Hero, Guid> heroes,
      IEntityRepository<HeroBaseStats, Guid> baseStatsRepository,
      IEntityRepository<HeroLore, Guid> loreRepository,


      IEntityRepository<MythologyType, Guid> mythologies,
      IEntityRepository<HeroClass, Guid> heroClasses,
      IEntityRepository<OriginArchetype, Guid> archetypes,
     IEntityRepository<OriginCulture, Guid> cultures,
      ISkillRepository skills,

      IHeroUniquenessValidator uniquenessValidator)
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

    _uniquenessValidator = uniquenessValidator;
  }

  public async Task<Guid> CreateAsync(
    HeroCreateRequestDto request,
    CancellationToken cancellationToken)
  {
    var userId = MediatorContext.Current.UserId ?? "system";

    // =====================================================
    // 0. HARD PRECONDITION (via validator - NOT repository)
    // =====================================================
    await _uniquenessValidator.EnsureUniqueHeroNameAsync(
        request.Name,
        cancellationToken);
    if (string.IsNullOrWhiteSpace(request.Name))
      throw new BusinessException("422", "Please provide a valid hero name.");
    // =====================================================
    // 1. Resolve reference data
    // =====================================================

    var mythology = await _mythologies.GetByIdAsync(request.MythologyTypeId, cancellationToken)
        ?? throw new BusinessException("422", "All Heroes Belong to a Mythology. Please provide a valid Mythology.");
    var heroClass = await _heroClasses.GetByIdAsync(request.HeroClassId, cancellationToken)
        ?? throw new BusinessException("422", "All Heroes Posses a Class. Please provide a valid Class.");

    var archetype = await _archetypes.GetByIdAsync(request.OriginArchetypeId, cancellationToken)
        ?? throw new BusinessException("422", "All Heroes Belong to an Archetype. Please provide a valid Archetype.");

    var culture = await _cultures.GetByIdAsync(request.OriginArchetypeId, cancellationToken)
        ?? throw new BusinessException("422", "All Heroes Belong to a Cutural Region. Please provide a valid Culture.");

    // =====================================================
    // 2. Create aggregate root
    // =====================================================

    var hero = _heroFactory.Create();

    // =====================================================
    // 3. Affiliation
    // =====================================================

    var affiliation = new HeroAffiliation(
        archetype.Id,
        mythology.Id,
        culture.Id
    );

    // =====================================================
    // 4. Base Stats
    // =====================================================

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

    // =====================================================
    // 5. Lore
    // =====================================================

    var lore = _loreFactory.Create();

    lore.Define(
        hero.Id,
        request.Lore.Title,
        request.Lore.Description,
        request.Lore.BackgroundStory,
        userId
    );

    // =====================================================
    // 6. Skills
    // =====================================================

    var requestedSkillIds = new[]
    {
      request.SkillKit.PassiveSkillId,
      request.SkillKit.PrimarySkillId,
      request.SkillKit.SecondarySkillId,
      request.SkillKit.TertiarySkillId,
      request.SkillKit.UltimateSkillId
    };

    var resolvedSkills = await _skills.GetByIdsAsync(requestedSkillIds, cancellationToken);

    var missingSkillIds = requestedSkillIds
        .Distinct()
        .Except(resolvedSkills.Select(skill => skill.Id))
        .ToList();

    if (missingSkillIds.Count > 0)
      throw new BusinessException("422", $"Please provide valid skills. Unknown skill id(s): {string.Join(", ", missingSkillIds)}.");

    var skillKit = new HeroSkillKit(
        request.SkillKit.PassiveSkillId,
        request.SkillKit.PrimarySkillId,
        request.SkillKit.SecondarySkillId,
        request.SkillKit.TertiarySkillId,
        request.SkillKit.UltimateSkillId
    );

    // =====================================================
    // 7. Initialize Hero
    // =====================================================

    hero.Define(
        request.Name,
        heroClass.Id,
        affiliation,
        baseStats,
        userId
    );

    hero.SetSkillKit(skillKit);

    // =====================================================
    // 8. Persist
    // =====================================================

    await _heroes.AddAsync(hero, cancellationToken);
    await _baseStatsRepository.AddAsync(baseStats, cancellationToken);
    await _loreRepository.AddAsync(lore, cancellationToken);

    return hero.Id;
  }
}