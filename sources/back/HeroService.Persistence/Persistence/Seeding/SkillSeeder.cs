using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Persistence.Seeding;

public sealed class SkillSeeder : ISeeder
{
  public int Order => 3;

  private readonly IEntityFactory<Guid, Skill> _skillFactory;
  private readonly IEntityFactory<Guid, SkillBaseStats> _baseStatsFactory;
  private readonly IEntityFactory<Guid, SkillEffect> _effectFactory;

  private readonly IEntityRepository<Skill, Guid> _skills;
  private readonly IEntityRepository<SkillBaseStats, Guid> _baseStatsRepo;
  private readonly IEntityRepository<SkillEffect, Guid> _effectRepo;

  private readonly IGameVersionRepository _versions;
  private readonly IUnitOfWork _uow;

  public SkillSeeder(
      IEntityFactory<Guid, Skill> skillFactory,
      IEntityFactory<Guid, SkillBaseStats> baseStatsFactory,
      IEntityFactory<Guid, SkillEffect> effectFactory,
      IEntityRepository<Skill, Guid> skills,
      IEntityRepository<SkillBaseStats, Guid> baseStatsRepo,
      IEntityRepository<SkillEffect, Guid> effectRepo,
      IGameVersionRepository versions,
      IUnitOfWork uow)
  {
    _skillFactory = skillFactory;
    _baseStatsFactory = baseStatsFactory;
    _effectFactory = effectFactory;

    _skills = skills;
    _baseStatsRepo = baseStatsRepo;
    _effectRepo = effectRepo;

    _versions = versions;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _skills.GetAllAsync(ct)).Any())
      return;



    var skills = new List<Skill>
    {
      CreateSkill("Mjolnir Strike", SkillType.Damage, 6f, 40f, 120f,
        EffectType.Damage, EffectType.CrowdControl),

      CreateSkill("Thunder Leap", SkillType.Mobility, 8f, 50f, 80f,
        EffectType.Mobility, EffectType.Damage),

      CreateSkill("Storm Aura", SkillType.Shield, 12f, 60f, 0f,
        EffectType.Shield, EffectType.Utility),

      CreateSkill("Lightning Chain", SkillType.Damage, 5f, 35f, 90f,
        EffectType.Damage, EffectType.ZoneControl),

      CreateSkill("God of Thunder", SkillType.Ultimate, 90f, 100f, 300f,
        EffectType.Damage, EffectType.CrowdControl),

      CreateSkill("Lion's Might", SkillType.Buff, 7f, 30f, 100f,
        EffectType.Damage, EffectType.Shield),

      CreateSkill("Hydra Strike", SkillType.Damage, 4f, 25f, 140f,
        EffectType.Damage, EffectType.DamageOverTime),

      CreateSkill("Titan Grip", SkillType.Shield, 10f, 40f, 0f,
        EffectType.Shield, EffectType.Buff),

      CreateSkill("Labors Rush", SkillType.Mobility, 9f, 45f, 0f,
        EffectType.Mobility, EffectType.Utility),

      CreateSkill("Divine Endurance", SkillType.Ultimate, 85f, 120f, 0f,
        EffectType.Shield, EffectType.Utility)
    };

    foreach (var skill in skills)
      await _skills.AddAsync(skill, ct);

    await _uow.CommitAsync(ct);
  }

  // -------------------------
  // Aggregate construction only
  // -------------------------
  private Skill CreateSkill(
      string name,
      SkillType type,
      float cooldown,
      float mana,
      float damage,
      EffectType effectA,
      EffectType effectB)
  {
    var skill = _skillFactory.Create();

    skill.Define(name, type, "skill-seeder");

    var stats = _baseStatsFactory.Create();
    stats.Define(
      skill.Id,
      cooldown,
      mana,
      damage,
      null,
      null,
      null,
      null,
      null,
      null,
      "skill-seeder"
    );

    skill.SetBaseStats(stats);

    skill.AddEffect(CreateEffect(skill.Id, effectA));
    skill.AddEffect(CreateEffect(skill.Id, effectB));

    return skill;
  }

  // -------------------------
  // Effect factory (pure)
  // -------------------------
  private SkillEffect CreateEffect(Guid skillId, EffectType type)
  {
    var effect = _effectFactory.Create();

    effect.Define(
      skillId,
      type,
      magnitude: 1f,
      duration: 0f,
      radius: 0f,
      targetType: TargetType.Enemy,
      stackType: StackType.None,
      maxStacks: 0,
      adRatio: null,
      apRatio: null,
      hpRatio: null,
      isPeriodic: false,
      isInstant: true,
      isChannelled: false,
      createdBy: "skill-seeder"
    );

    return effect;
  }
}