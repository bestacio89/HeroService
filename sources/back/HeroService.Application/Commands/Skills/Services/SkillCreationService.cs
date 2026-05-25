using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.EntityFramework.Auditing;
using HeroService.Contracts.DTOs.Skills;
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

  public SkillCreationService(
      IEntityFactory<Guid, Skill> skillFactory,
      IEntityFactory<Guid, SkillBaseStats> baseStatsFactory,
      IEntityFactory<Guid, SkillLore> loreFactory,
      IEntityFactory<Guid, SkillEffect> effectFactory,
      IEntityRepository<Skill, Guid> skills,
      IEntityRepository<SkillBaseStats, Guid> baseStatsRepo,
      IEntityRepository<SkillLore, Guid> loreRepo,
      IEntityRepository<SkillEffect, Guid> effectsRepo,
      ICurrentUserService currentUser)
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
  }

  public async Task<Guid> CreateAsync(
      SkillDto request,
      CancellationToken ct)
  {
    var createdBy = _currentUser.UserId;

    // =====================================================
    // 1. Skill aggregate root
    // =====================================================
    var skill = _skillFactory.Create();

    skill.Define(
        request.Name,
        ParseSkillType(request.SkillType),
        createdBy
    );

    // =====================================================
    // 2. Base Stats (deterministic combat definition)
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
        createdBy
    );

    // =====================================================
    // 3. Lore (pure presentation layer)
    // =====================================================
    var lore = _loreFactory.Create();

    lore.Define(
        skill.Id,
        request.Lore.Name,
        request.Lore.Description,
        request.Lore.VisualExplanation,
        createdBy
    );

    // =====================================================
    // 4. Effects (compositional behavior definitions)
    // =====================================================
    var effects = request.Effects
        .Select(dto =>
        {
          var effect = _effectFactory.Create();

          effect.Define(
          skill.Id,
          ParseEffectType(dto.EffectType),
          dto.Magnitude,
          dto.Duration,
          dto.Radius,
          ParseTargetType(dto.TargetType),
          ParseStackType(dto.StackType),
          dto.MaxStacks,

          dto.AttackDamageRatio,   // ✔ missing before
          dto.AbilityPowerRatio,   // ✔ missing before
          null,                    // hpRatio (if not in DTO)

          dto.IsPeriodic,
          dto.IsInstant,
          dto.IsChannelled,

          createdBy
         );

          return effect;
        })
        .ToList();

    // =====================================================
    // 5. Persistence layer (single logical transaction boundary)
    // =====================================================
    await _skills.AddAsync(skill, ct);
    await _baseStatsRepo.AddAsync(baseStats, ct);
    await _loreRepo.AddAsync(lore, ct);
    await _effectsRepo.AddRangeAsync(effects, ct);

    return skill.Id;
  }

  // =====================================================
  // Mapping helpers (kept local = clean application layer)
  // =====================================================
  private static SkillType ParseSkillType(string type)
    => Enum.Parse<SkillType>(type, ignoreCase: true);

  private static EffectType ParseEffectType(string type)
    => Enum.Parse<EffectType>(type, ignoreCase: true);

  private static TargetType ParseTargetType(string type)
    => Enum.Parse<TargetType>(type, ignoreCase: true);

  private static StackType ParseStackType(string type)
    => Enum.Parse<StackType>(type, ignoreCase: true);
}