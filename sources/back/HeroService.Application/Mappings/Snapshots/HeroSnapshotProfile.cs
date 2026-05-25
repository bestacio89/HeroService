using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

public class HeroSnapshotProfile : FranzMapProfile
{
  public HeroSnapshotProfile()
  {
    CreateMap<HeroSnapshot, HeroSnapshotDto>()
        .ConstructUsing(src => new HeroSnapshotDto(
            src.HeroId,
            src.GameVersionId,
            new HeroStatSnapshotDto(
                src.Stats.Health,
                src.Stats.Mana,
                src.Stats.AttackDamage,
                src.Stats.AbilityPower,
                src.Stats.AttackSpeed,
                src.Stats.CastSpeed,
                src.Stats.CritChance,
                src.Stats.CritDamageMultiplier,
                src.Stats.Armor,
                src.Stats.MagicResistance,
                src.Stats.DamageReduction,
                0f, // ShieldStrengthMultiplier not in domain snapshot → default or extend domain
                src.Stats.MovementSpeed,
                src.Stats.AttackRange,
                src.Stats.CooldownReduction,
                src.Stats.ResourceRegeneration
            ),
            new HeroSkillKitSnapshotDto(
                MapSkill(src.SkillKit.Passive),
                MapSkill(src.SkillKit.Primary),
                MapSkill(src.SkillKit.Secondary),
                MapSkill(src.SkillKit.Tertiary),
                MapSkill(src.SkillKit.Ultimate)
            )
        ));
  }



}