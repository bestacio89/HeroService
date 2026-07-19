using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.EntityFramework.Auditing;
using HeroService.Application.Commands.Skills.Services;
using HeroService.Contracts.Commands.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Commands.Skills.Services;

public sealed class SkillCreationService : ISkillCreationService
{
  private readonly IEntityFactory<Guid, Skill> _skillFactory;
  private readonly IEntityFactory<Guid, SkillBaseStats> _baseStatsFactory;
  private readonly IEntityFactory<Guid, SkillLore> _loreFactory;
  private readonly IEntityFactory<Guid, SkillEffect> _effectFactory;

  private readonly IEntityRepository<Skill, Guid> _skills;
  private readonly IEntityRepository<SkillBaseStats, Guid> _baseStatsRepo;
  private readonly IEntityRepository<SkillLore, Guid> _loreRepo;
  private readonly IEntityRepository<SkillEffect, Guid> _effectsRepo;

  private readonly ICurrentUserService _currentUser;
  private readonly ISkillUniquenessValidator _uniquenessValidator;


  public SkillCreationService(
      IEntityFactory<Guid, Skill> skillFactory,
      IEntityFactory<Guid, SkillBaseStats> baseStatsFactory,
      IEntityFactory<Guid, SkillLore> loreFactory,
      IEntityFactory<Guid, SkillEffect> effectFactory,

      IEntityRepository<Skill, Guid> skills,
      IEntityRepository<SkillBaseStats, Guid> baseStatsRepo,
      IEntityRepository<SkillLore, Guid> loreRepo,
      IEntityRepository<SkillEffect, Guid> effectsRepo,

      ICurrentUserService currentUser,
      ISkillUniquenessValidator uniquenessValidator)
  {
    _skillFactory = skillFactory;
    _baseStatsFactory = baseStatsFactory;
    _loreFactory = loreFactory;
    _effectFactory = effectFactory;

    _skills = skills;
    _baseStatsRepo = baseStatsRepo;
    _loreRepo = loreRepo;
    _effectsRepo = effectsRepo;

    _currentUser = currentUser;
    _uniquenessValidator = uniquenessValidator;
  }


  public async Task<Guid> CreateAsync(
      CreateSkillCommand request,
      CancellationToken ct)
  {
    var createdBy = _currentUser.UserId;


    // =====================================================
    // 0. VALIDATION
    // =====================================================

    await _uniquenessValidator.EnsureUniqueSkillNameAsync(
        request.Name,
        ct);


    // =====================================================
    // 1. SKILL ROOT
    // =====================================================

    var skill = _skillFactory.Create();

    skill.Define(
        request.Name,
        request.SkillType,
        createdBy);


    // =====================================================
    // 2. BASE STATS
    // =====================================================

    var baseStats = _baseStatsFactory.Create();

    baseStats.Define(
        skill.Id,

        request.BaseStats.Cooldown,
        request.BaseStats.ManaCost,

        request.BaseStats.Damage,
        request.BaseStats.Healing,
        request.BaseStats.ShieldValue,

        request.BaseStats.CastTime,
        request.BaseStats.ChannelDuration,

        request.BaseStats.CrowdControlDuration,
        request.BaseStats.Range,

        createdBy);


    // =====================================================
    // 3. LORE
    // =====================================================

    var lore = _loreFactory.Create();

    lore.Define(
        skill.Id,

        request.Lore.Description,
        request.Lore.VisualExplanation,

        createdBy);



    // =====================================================
    // 4. EFFECTS
    // =====================================================

    var effects = request.Effects
        .Select(dto =>
        {
          var effect = _effectFactory.Create();


          effect.Define(

              skill.Id,

              dto.EffectType,


              // Resolution values
              dto.Magnitude,
              dto.Duration,
              dto.Radius,


              // Targeting
              dto.TargetType,


              // Stacking
              dto.StackType,
              dto.MaxStacks,


              // Scaling
              dto.AttackDamageRatio,
              dto.MagicDamageRatio,
              dto.MaxHealthRatio,


              // Execution mode
              dto.IsPeriodic,
              dto.IsInstant,
              dto.IsChannelled,


              // State specialization
              dto.BuffType,
              dto.DebuffType,


              createdBy
          );


          skill.AddEffect(effect);


          return effect;
        })
        .ToList();



    // =====================================================
    // 5. PERSISTENCE
    // =====================================================

    await _skills.AddAsync(
        skill,
        ct);


    await _baseStatsRepo.AddAsync(
        baseStats,
        ct);


    await _loreRepo.AddAsync(
        lore,
        ct);


    await _effectsRepo.AddRangeAsync(
        effects,
        ct);


    return skill.Id;
  }
}