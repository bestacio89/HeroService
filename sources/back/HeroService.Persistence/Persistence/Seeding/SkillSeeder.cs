using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Persistence.Seeding;

public sealed class SkillSeeder : ISeeder
{
  public int Order => 4;

  private readonly IEntityFactory<Guid, SkillBaseStats> _baseStatsFactory;
  private readonly IEntityRepository<SkillBaseStats, Guid> _baseStatsRepo;

  private readonly IEntityFactory<Guid, SkillEffect> _effectFactory;
  private readonly IEntityRepository<SkillEffect, Guid> _effectRepo;

  private readonly ISkillRepository _skills;
  private readonly IGameVersionRepository _versions;
  private readonly IUnitOfWork _uow;

  public SkillSeeder(
      IEntityFactory<Guid, SkillBaseStats> baseStatsFactory,
      IEntityRepository<SkillBaseStats, Guid> baseStatsRepo,
      IEntityFactory<Guid, SkillEffect> effectFactory,
      IEntityRepository<SkillEffect, Guid> effectRepo,
      ISkillRepository skills,
      IGameVersionRepository versions,
      IUnitOfWork uow)
  {
    _baseStatsFactory = baseStatsFactory;
    _baseStatsRepo = baseStatsRepo;
    _effectFactory = effectFactory;
    _effectRepo = effectRepo;
    _skills = skills;
    _versions = versions;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _baseStatsRepo.GetAllAsync(ct)).Any())
      return;

    var version = await _versions.GetByVersionNumberAsync("1.0.0", ct)
        ?? throw new InvalidOperationException("Missing GameVersion 1.0.0");

    // =====================================================
    // LOAD SKILLS
    // =====================================================

    var mjolnir = await GetSkill("Mjolnir Strike", ct);
    var thunderLeap = await GetSkill("Thunder Leap", ct);
    var stormAura = await GetSkill("Storm Aura", ct);
    var lightningChain = await GetSkill("Lightning Chain", ct);
    var thorUlt = await GetSkill("God of Thunder", ct);

    var lionsMight = await GetSkill("Lion's Might", ct);
    var hydraStrike = await GetSkill("Hydra Strike", ct);
    var titanGrip = await GetSkill("Titan Grip", ct);
    var laborsRush = await GetSkill("Labors Rush", ct);
    var heraUlt = await GetSkill("Divine Endurance", ct);

    // =====================================================
    // SKILL DEFINITIONS
    // =====================================================

    await DefineSkill(mjolnir.Id, ct,
      cooldown: 6f, mana: 40f, damage: 120f,
      effects: new[] { EffectType.Damage });

    await DefineSkill(thunderLeap.Id, ct,
      cooldown: 8f, mana: 50f, damage: 80f,
      effects: new[] { EffectType.Damage, EffectType.Mobility });

    await DefineSkill(stormAura.Id, ct,
      cooldown: 12f, mana: 60f, shield: 100f,
      effects: new[] { EffectType.Shield, EffectType.Utility });

    await DefineSkill(lightningChain.Id, ct,
      cooldown: 5f, mana: 35f, damage: 90f,
      effects: new[] { EffectType.Damage, EffectType.ZoneControl });

    await DefineSkill(thorUlt.Id, ct,
      cooldown: 90f, mana: 100f, damage: 300f,
      effects: new[]
      {
        EffectType.Damage,
        EffectType.CrowdControl,
        EffectType.ZoneControl
      });

    await DefineSkill(lionsMight.Id, ct,
      cooldown: 7f, mana: 30f, damage: 100f,
      effects: new[] { EffectType.Damage, EffectType.Shield });

    await DefineSkill(hydraStrike.Id, ct,
      cooldown: 4f, mana: 25f, damage: 140f,
      effects: new[] { EffectType.Damage });

    await DefineSkill(titanGrip.Id, ct,
      cooldown: 10f, mana: 40f, shield: 150f,
      effects: new[] { EffectType.Shield });

    await DefineSkill(laborsRush.Id, ct,
      cooldown: 9f, mana: 45f,
      effects: new[] { EffectType.Mobility, EffectType.Utility });

    await DefineSkill(heraUlt.Id, ct,
      cooldown: 85f, mana: 120f, shield: 200f,
      effects: new[] { EffectType.Shield, EffectType.Utility });

    await _uow.CommitAsync(ct);
  }

  // =====================================================
  // CORE CREATION
  // =====================================================

  private async Task DefineSkill(
      Guid skillId,
      CancellationToken ct,
      float cooldown = 0f,
      float mana = 0f,
      float damage = 0f,
      float healing = 0f,
      float shield = 0f,
      float cast = 0f,
      float channel = 0f,
      float cc = 0f,
      float range = 0f,
      EffectType[] effects = null!)
  {
    // -------------------------
    // BASE STATS
    // -------------------------
    var baseStats = _baseStatsFactory.Create();

    baseStats.Define(
      skillId,
      cooldown,
      mana,
      damage,
      healing,
      shield,
      cast,
      channel,
      cc,
      range,
      "skill-seeder"
    );

    await _baseStatsRepo.AddAsync(baseStats, ct);

    // -------------------------
    // EFFECTS
    // -------------------------
    effects ??= Array.Empty<EffectType>();

    foreach (var effect in effects)
    {
      var e = _effectFactory.Create();

      e.Define(
        skillId: skillId,
        effectType: effect,
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

      await _effectRepo.AddAsync(e, ct);
    }
  }

  private async Task<Skill> GetSkill(string name, CancellationToken ct)
  {
    return await _skills.GetByNameAsync(name, ct)
        ?? throw new InvalidOperationException($"Missing skill: {name}");
  }
}