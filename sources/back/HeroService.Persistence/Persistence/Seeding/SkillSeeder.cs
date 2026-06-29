using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence.Persistence.Seeding;
using IUnitOfWork = Franz.Common.EntityFramework.IUnitOfWork;
namespace HeroService.Persistence.Seeding;

public sealed class SkillSeeder : ISeeder
{
  public int Order => 3;
  private readonly IEntityFactory<Guid, Skill> _skillFactory;
  private readonly IEntityFactory<Guid, SkillEffect> _effectFactory;
  private readonly IEntityFactory<Guid, SkillLore> _loreFactory;
  private readonly IEntityFactory<Guid, SkillBaseStats> _baseStatsFactory;
  private readonly IEntityRepository<SkillBaseStats, Guid> _baseStats;

  private readonly IEntityRepository<Skill, Guid> _skills;
  private readonly IEntityRepository<SkillEffect, Guid> _effects;
  private readonly IEntityRepository<SkillLore, Guid> _lore;
  private readonly IUnitOfWork _unitOfWork;
  public SkillSeeder(
      IEntityFactory<Guid, Skill> skillFactory,
      IEntityFactory<Guid, SkillEffect> effectFactory,
      IEntityFactory<Guid, SkillLore> loreFactory,
      IEntityFactory<Guid, SkillBaseStats> baseStatsFactory,
      IEntityRepository<Skill, Guid> skills,
      IEntityRepository<SkillEffect, Guid> effects,
      IEntityRepository<SkillLore, Guid> lore,
      IEntityRepository<SkillBaseStats, Guid> baseStats,
      IUnitOfWork unitOfWork)
  {
    _skillFactory = skillFactory;
    _effectFactory = effectFactory;
    _loreFactory = loreFactory;
    _baseStatsFactory = baseStatsFactory;
    _baseStats = baseStats;
    _skills = skills;
    _effects = effects;
    _lore = lore;
    _unitOfWork = unitOfWork;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    var existing = await _skills.GetAllAsync(ct);
    if (existing.Any())
      return;

    var createdBy = "seed-system";

    // =====================================================
    // ===================== THOR ==========================
    // =====================================================

    // 1. Mjolnir Strike (Primary Damage)
    var mjolnirStrike = _skillFactory.Create();
    mjolnirStrike.Define("Mjolnir Strike", SkillType.Damage, createdBy);
    
    
    var mjolnirStats = _baseStatsFactory.Create();
    mjolnirStats.Define(
        mjolnirStrike.Id,
        baseCooldown: 8f,
        baseManaCost: 60f,
        baseDamage: 150f,
        baseHealing: 0f,
        baseShieldValue: 0f,
        baseCastTime: 0.5f,
        baseChannelDuration: 0f,
        baseCrowdControlDuration: 0f,
        baseRange: 2f,
        createdBy);

    mjolnirStrike.SetBaseStats(mjolnirStats);


    var mjolnirEffect = _effectFactory.Create();
    mjolnirEffect.Define(
        mjolnirStrike.Id,
        EffectType.Damage,
        150f,
        0f,
        2f,
        TargetType.Enemy,
        StackType.None,
        1,
        1.4f,
        null,
        null,
        false,
        true,
        false,
        createdBy
    );



    mjolnirStrike.AddEffect(mjolnirEffect);

    var mjolnirLore = _loreFactory.Create();
    mjolnirLore.Define(
        mjolnirStrike.Id,
        "Thor crushes a single enemy with divine hammer force.",
        "A lightning-charged hammer impact from above.",
        createdBy
    );

    await Save(mjolnirStrike, mjolnirEffect, mjolnirLore, mjolnirStats, ct);


    // 2. Thunder Leap (Mobility + Control)
    var thunderLeap = _skillFactory.Create();
    thunderLeap.Define("Thunder Leap", SkillType.Mobility, createdBy);

    var thunderStats = _baseStatsFactory.Create();
    thunderStats.Define(
        thunderLeap.Id,
        baseCooldown: 14f,
        baseManaCost: 80f,
        baseDamage: 80f,
        baseHealing: 0f,
        baseShieldValue: 0f,
        baseCastTime: 0.3f,
        baseChannelDuration: 0f,
        baseCrowdControlDuration: 1.2f,
        baseRange: 4f,
        createdBy);

    var thunderLeapEffect = _effectFactory.Create();
    thunderLeapEffect.Define(
        thunderLeap.Id,
        EffectType.ZoneControl,
        80f,
        1.2f,
        4f,
        TargetType.AreaEnemies,
        StackType.None,
        1,
        0.6f,
        null,
        null,
        false,
        true,
        false,
        createdBy
    );

    thunderLeap.SetBaseStats(thunderStats);
    thunderLeap.AddEffect(thunderLeapEffect);
    var thunderLeapLore = _loreFactory.Create();
    thunderLeapLore.Define(
        thunderLeap.Id,
        "Thor leaps and shatters the ground upon landing.",
        "Shockwave of lightning expands outward.",
        createdBy
    );

    await Save(thunderLeap, thunderLeapEffect, thunderLeapLore, thunderStats, ct);

    // 3. Storm Aura (Buff)
    var stormAura = _skillFactory.Create();
    stormAura.Define("Storm Aura", SkillType.Buff, createdBy);
    
    var stormStats = _baseStatsFactory.Create();
    stormStats.Define(
        stormAura.Id,
        baseCooldown: 20f,
        baseManaCost: 100f,
        baseDamage: 0f,
        baseHealing: 0f,
        baseShieldValue: 0f,
        baseCastTime: 0f,
        baseChannelDuration: 0f,
        baseCrowdControlDuration: 0f,
        baseRange: 5f,
        createdBy);
    

    var stormAuraEffect = _effectFactory.Create();
    stormAuraEffect.Define(
        stormAura.Id,
        EffectType.Buff,
        25f,
        6f,
        5f,
        TargetType.Self,
        StackType.RefreshDuration,
        1,
        null,
        null,
        null,
        true,
        false,
        false,
        createdBy
    );



    stormAura.SetBaseStats(stormStats);
    stormAura.AddEffect(stormAuraEffect);

    var stormAuraLore = _loreFactory.Create();
    stormAuraLore.Define(
        stormAura.Id,
        "Thor surrounds himself with continuous storm energy.",
        "Electric aura pulses around the body.",
        createdBy
    );

    await Save(stormAura, stormAuraEffect, stormAuraLore, stormStats, ct);

    // 4. Lightning Chain (Damage Chain)
    var lightningChain = _skillFactory.Create();
    lightningChain.Define("Lightning Chain", SkillType.Damage, createdBy);

    var lightningStats = _baseStatsFactory.Create();
    lightningStats.Define(
        lightningChain.Id,
        baseCooldown: 10f,
        baseManaCost: 70f,
        baseDamage: 90f,
        baseHealing: 0f,
        baseShieldValue: 0f,
        baseCastTime: 0.4f,
        baseChannelDuration: 0f,
        baseCrowdControlDuration: 0f,
        baseRange: 0f,
        createdBy);

    

    var lightningEffect = _effectFactory.Create();
    lightningEffect.Define(
        lightningChain.Id,
        EffectType.Damage,
        90f,
        0f,
        0f,
        TargetType.Chain,
        StackType.None,
        1,
        1.0f,
        null,
        null,
        false,
        true,
        false,
        createdBy
    );


    lightningChain.SetBaseStats(lightningStats);
    lightningChain.AddEffect(lightningEffect);



    var lightningLore = _loreFactory.Create();
    lightningLore.Define(
        lightningChain.Id,
        "Lightning jumps between enemies in a devastating arc.",
        "Electric arcs chain across targets.",
        createdBy
    );

    await Save(lightningChain, lightningEffect, lightningLore, lightningStats, ct);

    // 5. God of Thunder (Ultimate)
    var thorUlt = _skillFactory.Create();
    thorUlt.Define("God of Thunder", SkillType.Ultimate, createdBy);

    var thorUltEffect = _effectFactory.Create();
    thorUltEffect.Define(
        thorUlt.Id,
        EffectType.Damage,
        300f,
        0f,
        6f,
        TargetType.AreaEnemies,
        StackType.None,
        1,
        2.5f,
        null,
        null,
        false,
        true,
        false,
        createdBy
    );
    thorUlt.AddEffect(thorUltEffect);

    var thorUltStats = _baseStatsFactory.Create();
    thorUltStats.Define(
        thorUlt.Id,
        baseCooldown: 90f,
        baseManaCost: 200f,
        baseDamage: 300f,
        baseHealing: 0f,
        baseShieldValue: 0f,
        baseCastTime: 1.0f,
        baseChannelDuration: 0f,
        baseCrowdControlDuration: 0f,
        baseRange: 6f,
    
        createdBy);
   thorUlt.SetBaseStats(thorUltStats);

    var thorUltLore = _loreFactory.Create();
    thorUltLore.Define(
        thorUlt.Id,
        "Thor unleashes full divine storm upon the battlefield.",
        "Sky fractures with continuous lightning strikes.",
        createdBy
    );

    await Save(thorUlt, thorUltEffect, thorUltLore, thorUltStats, ct);
    // =====================================================
    // =================== HERAKLES ========================
    // =====================================================

    // 1. Lion's Might (Buff)
    var lionsMight = _skillFactory.Create();
    lionsMight.Define("Lion's Might", SkillType.Buff, createdBy);
    
    var lionsStats = _baseStatsFactory.Create();
    lionsStats.Define(
        lionsMight.Id,
        baseCooldown: 15f,
        baseManaCost: 80f,
        baseDamage: 0f,
        baseHealing: 0f,
        baseShieldValue: 0f,
        baseCastTime: 0f,
        baseChannelDuration: 0f,
        baseCrowdControlDuration: 0f,
        baseRange: 0f,
        createdBy);

    lionsMight.SetBaseStats(lionsStats);

    var lionsEffect = _effectFactory.Create();
    lionsEffect.Define(
        lionsMight.Id,
        EffectType.Buff,
        40f,
        8f,
        0f,
        TargetType.Self,
        StackType.Refresh,
        1,
        null,
        null,
        0.25f,
        false,
        true,
        false,
        createdBy
    );

    lionsMight.AddEffect(lionsEffect);

    var lionsLore = _loreFactory.Create();
    lionsLore.Define(
        lionsMight.Id,
        "Herakles channels the strength of the Nemean Lion.",
        "Golden aura of overwhelming strength.",
        createdBy
    );

    await Save(lionsMight, lionsEffect, lionsLore, lionsStats, ct);

    // 2. Hydra Strike (Damage)
    var hydraStrike = _skillFactory.Create();
    hydraStrike.Define("Hydra Strike", SkillType.Damage, createdBy);

    var hydraStats = _baseStatsFactory.Create();
    hydraStats.Define(
        hydraStrike.Id,
        baseCooldown: 10f,
        baseManaCost: 70f,
        baseDamage: 170f,
        baseHealing: 0f,
        baseShieldValue: 0f,
        baseCastTime: 0.6f,
        baseChannelDuration: 0f,
        baseCrowdControlDuration: 0f,
        baseRange: 2f,
        createdBy);


    hydraStrike.SetBaseStats(hydraStats);

    var hydraEffect = _effectFactory.Create();
    hydraEffect.Define(
        hydraStrike.Id,
        EffectType.Damage,
        170f,
        0f,
        2f,
        TargetType.Enemy,
        StackType.None,
        1,
        1.5f,
        null,
        null,
        false,
        true,
        false,
        createdBy
    );

    hydraStrike.AddEffect(hydraEffect);

    var hydraLore = _loreFactory.Create();
    hydraLore.Define(
        hydraStrike.Id,
        "A crushing strike inspired by the Hydra’s relentless nature.",
        "Multiple phantom blows overlap the impact.",
        createdBy
    );

    await Save(hydraStrike, hydraEffect, hydraLore, hydraStats, ct);

    // 3. Titan Grip (Shield)
    var titanGrip = _skillFactory.Create();
    titanGrip.Define("Titan Grip", SkillType.Shield, createdBy);

    var titanEffect = _effectFactory.Create();
    titanEffect.Define(
        titanGrip.Id,
        EffectType.Shield,
        120f,
        5f,
        0f,
        TargetType.Self,
        StackType.None,
        1,
        null,
        null,
        0.4f,
        false,
        true,
        false,
        createdBy
    );
    titanGrip.AddEffect(titanEffect);

    var titanStats = _baseStatsFactory.Create();
    titanStats.Define(
        titanGrip.Id,
        baseCooldown: 18f,
        baseManaCost: 90f,
        baseDamage: 0f,
        baseHealing: 0f,
        baseShieldValue: 120f,
        baseCastTime: 0f,
        baseChannelDuration: 0f,
        baseCrowdControlDuration: 0f,
        baseRange: 0f,
        createdBy);

   titanGrip.SetBaseStats(titanStats);

    var titanLore = _loreFactory.Create();
    titanLore.Define(
        titanGrip.Id,
        "Herakles withstands any force with divine resilience.",
        "A titan-like shield manifests around him.",
        createdBy
    );

    await Save(titanGrip, titanEffect, titanLore, titanStats, ct);

    // 4. Labors Rush (Mobility)
    var laborsRush = _skillFactory.Create();
    laborsRush.Define("Labors Rush", SkillType.Mobility, createdBy);

    var rushStats = _baseStatsFactory.Create();
    rushStats.Define(
        laborsRush.Id,
        baseCooldown: 12f,
        baseManaCost: 60f,
        baseDamage: 0f,
        baseHealing: 0f,
        baseShieldValue: 0f,
        baseCastTime: 0f,
        baseChannelDuration: 0f,
        baseCrowdControlDuration: 0f,
        baseRange: 0f,
        createdBy);

    laborsRush.SetBaseStats(rushStats);

    var rushEffect = _effectFactory.Create();
    rushEffect.Define(
        laborsRush.Id,
        EffectType.Buff,
        60f,
        3f,
        0f,
        TargetType.Self,
        StackType.Refresh,
        1,
        null,
        null,
        null,
        false,
        true,
        false,
        createdBy
    );

    laborsRush.AddEffect(rushEffect);

    var rushLore = _loreFactory.Create();
    rushLore.Define(
        laborsRush.Id,
        "Herakles surges forward with unstoppable momentum.",
        "A heroic blur of motion and strength.",
        createdBy
    );

    await Save(laborsRush, rushEffect, rushLore, rushStats, ct);

    // 5. Divine Endurance (Ultimate)
    var heraUlt = _skillFactory.Create();
    heraUlt.Define("Divine Endurance", SkillType.Ultimate, createdBy);
   
    var heraUltStats = _baseStatsFactory.Create();
    heraUltStats.Define(
        heraUlt.Id,
        baseCooldown: 120f,
        baseManaCost: 200f,
        baseDamage: 0f,
        baseHealing: 0f,
        baseShieldValue: 250f,
        baseCastTime: 0f,
        baseChannelDuration: 10f,
        baseCrowdControlDuration: 0f,
        baseRange: 0f,
        createdBy);

    heraUlt.SetBaseStats(heraUltStats);

    var heraUltEffect = _effectFactory.Create();
    heraUltEffect.Define(
        heraUlt.Id,
        EffectType.Buff,
        250f,
        10f,
        0f,
        TargetType.Self,
        StackType.Refresh,
        1,
        null,
        null,
        0.6f,
        false,
        true,
        false,
        createdBy
    );
    heraUlt.AddEffect( heraUltEffect );
    

    var heraUltLore = _loreFactory.Create();
    heraUltLore.Define(
        heraUlt.Id,
        "Herakles enters a divine state of unmatched endurance.",
        "Golden aura of mythic resilience.",
        createdBy
    );

    await Save(heraUlt, heraUltEffect, heraUltLore, heraUltStats, ct);

  }

  // =========================================================
  // Helper (keeps seeder clean + consistent)
  // =========================================================
  private async Task Save(
      Skill skill,
      SkillEffect effect,
      SkillLore lore,
      SkillBaseStats stats,
      CancellationToken ct)
  {
    await _skills.AddAsync(skill, ct);
    await _lore.AddAsync(lore, ct);
   // await _effects.AddAsync(effect, ct);
   // await _baseStats.AddAsync(stats, ct);
    await _unitOfWork.CommitAsync(ct);
  }
}