using Franz.Common.Mapping.Profiles;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Core.Skills;

public class HeroProfile : FranzMapProfile
{
  public HeroProfile()
  {
    CreateMap<Hero, HeroDto>()
            .ConstructUsing(h => new HeroDto(
                h.Id,
                h.Name,
                h.HeroClassId,
                new HeroClassDto(
              h.HeroClass.Id,
              h.HeroClass.Name
          ),

          new HeroAffiliationDto(
              new OriginArchetypeDto(
                  h.Affiliation.Archetype.Id,
                  h.Affiliation.Archetype.Name
              ),

              new MythologyTypeDto(
                  h.Affiliation.Mythology.Id,
                  h.Affiliation.Mythology.Name
              ),

              new OriginCultureDto(
                  h.Affiliation.OriginCulture.Id,
                  h.Affiliation.OriginCulture.Name
              )
          ),
                h.BaseStats != null ? new HeroBaseStatsDto(
                    h.BaseStats.BaseHealth,
                    h.BaseStats.BaseMana,
                    h.BaseStats.BaseAttackDamage,
                    h.BaseStats.BaseMagicDamage,
                    h.BaseStats.BaseAttackSpeed,
                    h.BaseStats.BaseCritChance,
                    h.BaseStats.BaseCritDamageMultiplier,
                    h.BaseStats.BaseArmor,
                    h.BaseStats.BaseMagicResistance,
                    h.BaseStats.BaseDamageReduction,
                    h.BaseStats.BaseShieldStrengthMultiplier,
                    h.BaseStats.BaseMovementSpeed,
                    h.BaseStats.BaseAttackRange,
                    h.BaseStats.BaseCastSpeed,
                    h.BaseStats.BaseCooldownReduction,
                    h.BaseStats.BaseResourceRegeneration
                ) : null!,
                h.SkillKit != null ? new HeroSkillKitDto(
                    h.SkillKit.PassiveSkillId,
                    h.SkillKit.PrimarySkillId,
                    h.SkillKit.SecondarySkillId,
                    h.SkillKit.TertiarySkillId,
                    h.SkillKit.UltimateSkillId
                ) : null!
            ));

    CreateMap<HeroClass, HeroClassDto>()
        .ConstructUsing(src => new HeroClassDto(
            src.Id,
            src.Name
        ));
  }
}