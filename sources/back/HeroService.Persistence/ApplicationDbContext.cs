using Franz.Common.Business.Domain;
using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Dispatchers;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Skins;
using HeroService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence
{
  public class ApplicationDbContext : DbContextBase
  {
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDispatcher dispatcher // HeroService mediator dispatcher
    ) : base(options, dispatcher)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

    }


    public DbSet<Hero> Heroes => Set<Hero>();
    public DbSet<Skin> Skins => Set<Skin>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<SkillEffect> SkillEffects => Set<SkillEffect>();

    public DbSet<GameVersion> GameVersions => Set<GameVersion>();
    public DbSet<MythologyType> MythologyTypes => Set<MythologyType>();
    public DbSet<HeroAffiliation> HeroAffiliations => Set<HeroAffiliation>();
    public DbSet<SkillBaseStats> SkillBaseStats => Set<SkillBaseStats>();
    public DbSet<HeroBaseStats> HeroBaseStats => Set<HeroBaseStats>();
    public DbSet<HeroModifier> HeroModifiers => Set<HeroModifier>();
    public DbSet<SkillModifier> SkillModifiers => Set<SkillModifier>();
    public DbSet<HeroLore> HeroLore => Set<HeroLore>();
    public DbSet<SkillLore> SkillLore => Set<SkillLore>();
    public DbSet<HeroClass> HeroClasses => Set<HeroClass>();
    public DbSet<OriginCulture> Origincultures => Set<OriginCulture>();
    public DbSet<OriginArchetype> OriginArchetypes => Set<OriginArchetype>();

   
  }
}
