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
        .ConstructUsing(skill => new SkillDto
        {
          Id = skill.Id,

          Name = skill.Name,

          SkillType = skill.SkillType.ToString(),

          Effects = skill.Effects
              .Select(effect => new SkillEffectDto
              {
                EffectType = effect.EffectType.ToString(),

                Magnitude = effect.Magnitude,
                Duration = effect.Duration,
                Radius = effect.Radius,

                TargetType = effect.TargetType.ToString(),
                StackType = effect.StackType.ToString(),

                MaxStacks = effect.MaxStacks,

                AttackDamageRatio = effect.AttackDamageRatio,
                AbilityPowerRatio = effect.AbilityPowerRatio,
                MaxHealthRatio = effect.MaxHealthRatio,

                IsPeriodic = effect.IsPeriodic,
                IsInstant = effect.IsInstant,
                IsChannelled = effect.IsChannelled
              })
              .ToList(),

           BaseStats = skill.BaseStats != null
            ? new SkillBaseStatsDto
            {
              Cooldown = skill.BaseStats.BaseCooldown,
              ManaCost = skill.BaseStats.BaseManaCost,
              Damage = skill.BaseStats.BaseDamage,
              Healing = skill.BaseStats.BaseHealing,
              ShieldValue = skill.BaseStats.BaseShieldValue,
              CastTime = skill.BaseStats.BaseCastTime,
              ChannelDuration = skill.BaseStats.BaseChannelDuration,
              Range = skill.BaseStats.BaseRange,
              CrowdControlDuration = skill.BaseStats.BaseCrowdControlDuration
            }
            : null
       });

    // =========================================
    // SkillEffect
    // =========================================

    CreateMap<SkillEffect, SkillEffectDto>()
        .ConstructUsing(effect => new SkillEffectDto
        {
          EffectType = effect.EffectType.ToString(),

          Magnitude = effect.Magnitude,
          Duration = effect.Duration,
          Radius = effect.Radius,

          TargetType = effect.TargetType.ToString(),
          StackType = effect.StackType.ToString(),

          MaxStacks = effect.MaxStacks,

          AttackDamageRatio = effect.AttackDamageRatio,
          AbilityPowerRatio = effect.AbilityPowerRatio,
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
          CrowdControlDuration = stats.BaseCrowdControlDuration,
       
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