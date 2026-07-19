using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Mappings.Skills;

public sealed class SkillProfile : FranzMapProfile
{
  public SkillProfile()
  {
    // =========================================
    // Skill
    // =========================================
    CreateMap<Skill, SkillDto>()
        .ConstructUsing((skill, context) => new SkillDto
        {
          Id = skill.Id,
          Name = skill.Name,
          SkillType = skill.SkillType,

          // OPTIMIZATION: Delegate nested collection mapping to the engine.
          // This eliminates the manual inline LINQ lambda allocation storm and leverages
          // the pre-compiled, cached execution plan of the SkillEffect profile.
          Effects = context.Map<IReadOnlyCollection<SkillEffect>, List<SkillEffectDto>>(skill.Effects),

          BaseStats = skill.BaseStats != null
                ? context.Map<SkillBaseStats, SkillBaseStatsDto>(skill.BaseStats)
                : null
        });

    // =========================================
    // SkillEffect
    // =========================================
    CreateMap<SkillEffect, SkillEffectDto>()
        .ConstructUsing(effect => new SkillEffectDto
        {
          EffectType = effect.EffectType,
          BuffType = effect.BuffType,
          DebuffType = effect.DebuffType,
          Magnitude = effect.Magnitude,
          Duration = effect.Duration,
          Radius = effect.Radius,
          TargetType = effect.TargetType,
          StackType = effect.StackType,
          MaxStacks = effect.MaxStacks,
          AttackDamageRatio = effect.AttackDamageRatio,
          MagicDamageRatio = effect.MagicDamageRatio,
          MaxHealthRatio = effect.MaxHealthRatio,
          IsPeriodic = effect.IsPeriodic,
          IsInstant = effect.IsInstant,
          IsChannelled = effect.IsChannelled
        });

    // =========================================
    // SkillBaseStats
    // =========================================
    CreateMap<SkillBaseStats, SkillBaseStatsDto>()
        .ConstructUsing(stats => new SkillBaseStatsDto
        {
          Cooldown = stats.BaseCooldown,
          ManaCost = stats.BaseManaCost,
          Damage = stats.BaseDamage,
          Healing = stats.BaseHealing,
          ShieldValue = stats.BaseShieldValue,
          CastTime = stats.BaseCastTime,
          ChannelDuration = stats.BaseChannelDuration,
          Range = stats.BaseRange,
          CrowdControlDuration = stats.BaseCrowdControlDuration
        });

    // =========================================
    // SkillLore
    // =========================================
    CreateMap<SkillLore, SkillLoreDto>()
        .ConstructUsing(lore => new SkillLoreDto
        {
          Description = lore.Description,
          VisualExplanation = lore.VisualExplanation
        });
  }
}