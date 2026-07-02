using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

/// <summary>
/// Seeds the skill catalog with multi-effect behavioral richness.
///
/// THOR — Engage + Burst archetype
///   Kit behavioral sequences that item conditions reward:
///     Mobility → ZoneControl       (Thunder Leap engage)
///     Damage → CrowdControl        (Mjolnir burst into lockdown)
///     Buff → Damage                (Storm Aura empowered strike)
///     Damage → Debuff → Damage     (Lightning Chain poke)
///     Damage → ZoneControl         (God of Thunder teamfight)
///
/// HERAKLES — Sustain + Bruiser archetype
///   Kit behavioral sequences that item conditions reward:
///     Shield → Buff                (Titan Grip defensive setup)
///     Buff → Buff                  (Lion's Might double empower)
///     Damage → Debuff              (Hydra Strike anti-heal)
///     Mobility → Buff              (Labors Rush aggressive repositioning)
///     Shield → Buff                (Divine Endurance unkillable window)
///
/// Effects are ordered deliberately — EffectSequenceTracker in MatchService
/// reads the order effects fire when a skill is cast. First effect = primary
/// behavioral identity of the skill. Second effect = secondary behavioral
/// consequence that creates item condition opportunities.
/// </summary>
public sealed class SkillSeeder : ISeeder
{
  public int Order => 3;

  private readonly IEntityFactory<Guid, Skill> _skillFactory;
  private readonly IEntityFactory<Guid, SkillEffect> _effectFactory;
  private readonly IEntityFactory<Guid, SkillLore> _loreFactory;

  private readonly IEntityRepository<Skill, Guid> _skills;
  private readonly IEntityRepository<SkillEffect, Guid> _effects;
  private readonly IEntityRepository<SkillLore, Guid> _lore;
  private readonly IUnitOfWork _unitOfWork;

  public SkillSeeder(
      IEntityFactory<Guid, Skill> skillFactory,
      IEntityFactory<Guid, SkillEffect> effectFactory,
      IEntityFactory<Guid, SkillLore> loreFactory,
      IEntityRepository<Skill, Guid> skills,
      IEntityRepository<SkillEffect, Guid> effects,
      IEntityRepository<SkillLore, Guid> lore,
      IUnitOfWork unitOfWork)
  {
    _skillFactory = skillFactory;
    _effectFactory = effectFactory;
    _loreFactory = loreFactory;
    _skills = skills;
    _effects = effects;
    _lore = lore;
    _unitOfWork = unitOfWork;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _skills.GetAllAsync(ct)).Any())
      return;

    const string system = "seed-system";

    // =====================================================
    // THOR
    // =====================================================

    // 1. Mjolnir Strike — Damage → CrowdControl
    //    Primary: burst damage on single target
    //    Secondary: brief stun on impact (hammer weight)
    //    Item condition: [Damage, CrowdControl] → "strike into control" window
    await CreateAsync(ct, system,
        name: "Mjolnir Strike",
        type: SkillType.Damage,
        loreBrief: "Thor crushes a single enemy with divine hammer force.",
        loreDesc: "A lightning-charged hammer impact from above.",
        effects: new[]
        {
                Effect(EffectType.Damage,
                    magnitude: 150f, duration: 0f,   radius: 0f,
                    target: TargetType.Enemy,         stack: StackType.None,
                    adRatio: 1.4f,                    isInstant: true),

                Effect(EffectType.CrowdControl,
                    magnitude: 0f,   duration: 0.75f, radius: 0f,
                    target: TargetType.Enemy,         stack: StackType.None,
                    isInstant: false),
        });

    // 2. Thunder Leap — Mobility → ZoneControl
    //    Primary: leap to target area (repositioning)
    //    Secondary: landing shockwave denies zone
    //    Item condition: [Mobility, ZoneControl] → "engage into denial" window
    await CreateAsync(ct, system,
        name: "Thunder Leap",
        type: SkillType.Mobility,
        loreBrief: "Thor leaps and shatters the ground upon landing.",
        loreDesc: "Shockwave of lightning expands outward.",
        effects: new[]
        {
                Effect(EffectType.Mobility,
                    magnitude: 0f,   duration: 0f,   radius: 0f,
                    target: TargetType.Self,          stack: StackType.None,
                    isInstant: true),

                Effect(EffectType.ZoneControl,
                    magnitude: 80f,  duration: 1.2f, radius: 4f,
                    target: TargetType.AreaEnemies,   stack: StackType.None,
                    adRatio: 0.6f,                    isInstant: false),
        });

    // 3. Storm Aura — Buff → Damage
    //    Primary: empowers Thor (attack speed + stats)
    //    Secondary: periodic lightning damage to nearby enemies while active
    //    Item condition: [Buff, Damage] → "self-empower into aura damage" window
    await CreateAsync(ct, system,
        name: "Storm Aura",
        type: SkillType.Buff,
        loreBrief: "Thor surrounds himself with continuous storm energy.",
        loreDesc: "Electric aura pulses around the body.",
        effects: new[]
        {
                Effect(EffectType.Buff,
                    magnitude: 25f,  duration: 6f,   radius: 0f,
                    target: TargetType.Self,          stack: StackType.RefreshDuration,
                    isPeriodic: true,               isInstant: false),

                Effect(EffectType.Damage,
                    magnitude: 20f,  duration: 6f,   radius: 3f,
                    target: TargetType.AreaEnemies,   stack: StackType.None,
                    apRatio: 0.3f,                    isPeriodic: true,
                    isInstant: false),
        });

    // 4. Lightning Chain — Damage → Debuff
    //    Primary: chain lightning jumps between enemies
    //    Secondary: reduces armor/resistance briefly (electricity weakens)
    //    Item condition: [Damage, Debuff] → "damage into weaken" poke chain
    await CreateAsync(ct, system,
        name: "Lightning Chain",
        type: SkillType.Damage,
        loreBrief: "Lightning jumps between enemies in a devastating arc.",
        loreDesc: "Electric arcs chain across targets.",
        effects: new[]
        {
                Effect(EffectType.Damage,
                    magnitude: 90f,  duration: 0f,   radius: 0f,
                    target: TargetType.Chain,         stack: StackType.None,
                    adRatio: 1.0f,                    isInstant: true),

                Effect(EffectType.Debuff,
                    magnitude: 15f,  duration: 2f,   radius: 0f,
                    target: TargetType.Chain,         stack: StackType.None,
                    isInstant: false),
        });

    // 5. God of Thunder — Damage → ZoneControl
    //    Primary: massive AoE lightning damage
    //    Secondary: sustained zone denial (lightning pillars)
    //    Item condition: [Damage, ZoneControl] → "ult into teamfight control"
    await CreateAsync(ct, system,
        name: "God of Thunder",
        type: SkillType.Ultimate,
        loreBrief: "Thor unleashes full divine storm upon the battlefield.",
        loreDesc: "Sky fractures with continuous lightning strikes.",
        effects: new[]
        {
                Effect(EffectType.Damage,
                    magnitude: 300f, duration: 0f,   radius: 6f,
                    target: TargetType.AreaEnemies,   stack: StackType.None,
                    adRatio: 2.5f,                    isInstant: true),

                Effect(EffectType.ZoneControl,
                    magnitude: 0f,   duration: 4f,   radius: 6f,
                    target: TargetType.AreaEnemies,   stack: StackType.None,
                    isPeriodic: true,               isInstant: false),
        });

    // =====================================================
    // HERAKLES
    // =====================================================

    // 1. Lion's Might — Buff → Buff
    //    Primary: raw strength bonus (attack damage)
    //    Secondary: critical strike empowerment
    //    Item condition: [Buff, Buff] → "double empower before striking"
    await CreateAsync(ct, system,
        name: "Lion's Might",
        type: SkillType.Buff,
        loreBrief: "Herakles channels the strength of the Nemean Lion.",
        loreDesc: "Golden aura of overwhelming strength.",
        effects: new[]
        {
                Effect(EffectType.Buff,
                    magnitude: 40f,  duration: 8f,   radius: 0f,
                    target: TargetType.Self,          stack: StackType.Refresh,
                    hpRatio: 0.25f,                   isInstant: false),

                Effect(EffectType.Buff,
                    magnitude: 20f,  duration: 8f,   radius: 0f,
                    target: TargetType.Self,          stack: StackType.Refresh,
                    isInstant: false),
        });

    // 2. Hydra Strike — Damage → Debuff
    //    Primary: crushing blow
    //    Secondary: grievous wound (reduces enemy healing)
    //    Item condition: [Damage, Debuff] → "punish into anti-heal"
    await CreateAsync(ct, system,
        name: "Hydra Strike",
        type: SkillType.Damage,
        loreBrief: "A crushing strike inspired by the Hydra's relentless nature.",
        loreDesc: "Multiple phantom blows overlap the impact.",
        effects: new[]
        {
                Effect(EffectType.Damage,
                    magnitude: 170f, duration: 0f,   radius: 0f,
                    target: TargetType.Enemy,         stack: StackType.None,
                    adRatio: 1.5f,                    isInstant: true),

                Effect(EffectType.Debuff,
                    magnitude: 40f,  duration: 3f,   radius: 0f,
                    target: TargetType.Enemy,         stack: StackType.None,
                    isInstant: false),
        });

    // 3. Titan Grip — Shield → Buff
    //    Primary: divine shield
    //    Secondary: damage reduction while shield is active
    //    Item condition: [Shield, Buff] → "layered defense" sustain
    await CreateAsync(ct, system,
        name: "Titan Grip",
        type: SkillType.Shield,
        loreBrief: "Herakles withstands any force with divine resilience.",
        loreDesc: "A titan-like shield manifests around him.",
        effects: new[]
        {
                Effect(EffectType.Shield,
                    magnitude: 120f, duration: 5f,   radius: 0f,
                    target: TargetType.Self,          stack: StackType.None,
                    hpRatio: 0.4f,                    isInstant: false),

                Effect(EffectType.Buff,
                    magnitude: 20f,  duration: 5f,   radius: 0f,
                    target: TargetType.Self,          stack: StackType.None,
                    isInstant: false),
        });

    // 4. Labors Rush — Mobility → Buff
    //    Primary: forward charge (repositioning)
    //    Secondary: attack speed burst post-dash
    //    Item condition: [Mobility, Buff] → "aggressive repositioning into haste"
    await CreateAsync(ct, system,
        name: "Labors Rush",
        type: SkillType.Mobility,
        loreBrief: "Herakles surges forward with unstoppable momentum.",
        loreDesc: "A heroic blur of motion and strength.",
        effects: new[]
        {
                Effect(EffectType.Mobility,
                    magnitude: 0f,   duration: 0f,   radius: 0f,
                    target: TargetType.Self,          stack: StackType.Refresh,
                    isInstant: true),

                Effect(EffectType.Buff,
                    magnitude: 30f,  duration: 3f,   radius: 0f,
                    target: TargetType.Self,          stack: StackType.Refresh,
                    isInstant: false),
        });

    // 5. Divine Endurance — Shield → Buff
    //    Primary: massive divine shield (unkillable window)
    //    Secondary: regeneration while shield holds
    //    Item condition: [Shield, Buff] → "unkillable sustain" ultimate window
    await CreateAsync(ct, system,
        name: "Divine Endurance",
        type: SkillType.Ultimate,
        loreBrief: "Herakles enters a divine state of unmatched endurance.",
        loreDesc: "Golden aura of mythic resilience.",
        effects: new[]
        {
                Effect(EffectType.Shield,
                    magnitude: 250f, duration: 10f,  radius: 0f,
                    target: TargetType.Self,          stack: StackType.Refresh,
                    hpRatio: 0.6f,                    isInstant: false),

                Effect(EffectType.Buff,
                    magnitude: 50f,  duration: 10f,  radius: 0f,
                    target: TargetType.Self,          stack: StackType.Refresh,
                    isPeriodic: true,               isInstant: false),
        });

    await _unitOfWork.CommitAsync(ct);
  }

  // =========================================================
  // CREATE HELPER — skill + effects + lore in one call
  // =========================================================

  private async Task CreateAsync(
      CancellationToken ct,
      string system,
      string name,
      SkillType type,
      string loreBrief,
      string loreDesc,
      EffectParams[] effects)
  {
    var skill = _skillFactory.Create();
    skill.Define(name, type, system);
    await _skills.AddAsync(skill, ct);

    foreach (var p in effects)
    {
      var effect = _effectFactory.Create();
      effect.Define(
          skill.Id,
          p.EffectType,
          p.Magnitude,
          p.Duration,
          p.Radius,
          p.TargetType,
          p.StackType,
          p.MaxStacks,
          p.AdRatio,
          p.ApRatio,
          p.HpRatio,
          p.IsPeriodic,
          p.IsInstant,
          p.IsChannelled,
          system);

      skill.AddEffect(effect);
      await _effects.AddAsync(effect, ct);
    }

    var lore = _loreFactory.Create();
    lore.Define(skill.Id, loreBrief, loreDesc, system);
    await _lore.AddAsync(lore, ct);
  }

  // =========================================================
  // EFFECT PARAMS — lightweight value struct for seeder clarity
  // Avoids 13-argument positional calls inline
  // =========================================================

  private readonly record struct EffectParams(
      EffectType EffectType,
      float Magnitude,
      float Duration,
      float Radius,
      TargetType TargetType,
      StackType StackType,
      int MaxStacks,
      float? AdRatio,
      float? ApRatio,
      float? HpRatio,
      bool IsPeriodic,
      bool IsInstant,
      bool IsChannelled);

  private static EffectParams Effect(
      EffectType effectType,
      float magnitude,
      float duration,
      float radius,
      TargetType target,
      StackType stack,
      int maxStacks = 1,
      float? adRatio = null,
      float? apRatio = null,
      float? hpRatio = null,
      bool isPeriodic = false,
      bool isInstant = true,
      bool isChannelled = false)
      => new(effectType, magnitude, duration, radius,
             target, stack, maxStacks,
             adRatio, apRatio, hpRatio,
             isPeriodic, isInstant, isChannelled);
}