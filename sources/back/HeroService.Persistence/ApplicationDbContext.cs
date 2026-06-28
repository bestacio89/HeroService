using Franz.Common.Business.Domain;
using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Dispatchers;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Progression;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Skins;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HeroService.Persistence
{
  public class ApplicationDbContext : DbContextBase
  {
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDispatcher dispatcher)
        : base(options, dispatcher)
    {

    }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // =========================================================
      // HERO
      // =========================================================
      modelBuilder.Entity<Hero>()
          .HasIndex(x => x.Name)
          .IsUnique();

      // HeroAffiliation is a value object owned by Hero —
      // its three axes are proper entities referenced by FK
      modelBuilder.Entity<Hero>()
          .OwnsOne(h => h.Affiliation, aff =>
          {
            aff.HasOne(a => a.Archetype)
               .WithMany()
               .HasForeignKey("ArchetypeId")
               .IsRequired();

            aff.HasOne(a => a.Mythology)
               .WithMany()
               .HasForeignKey("MythologyId")
               .IsRequired();

            aff.HasOne(a => a.OriginCulture)
               .WithMany()
               .HasForeignKey("OriginCultureId")
               .IsRequired();
          });

      // HeroSkillKit is a value object owned by Hero —
      // five Guid FKs stored as columns on Heroes table
      modelBuilder.Entity<Hero>()
          .OwnsOne(h => h.SkillKit, sk =>
          {
            sk.Property(x => x.PassiveSkillId).IsRequired();
            sk.Property(x => x.PrimarySkillId).IsRequired();
            sk.Property(x => x.SecondarySkillId).IsRequired();
            sk.Property(x => x.TertiarySkillId).IsRequired();
            sk.Property(x => x.UltimateSkillId).IsRequired();
          });

      // =========================================================
      // SKILL
      // =========================================================
      modelBuilder.Entity<Skill>()
          .HasIndex(x => x.Name)
          .IsUnique();

      base.OnModelCreating(modelBuilder);
    }

    // =========================================================
    // ENTITY SETS
    // =========================================================

    // Core
    public DbSet<Hero> Heroes => Set<Hero>();
    public DbSet<Skin> Skins => Set<Skin>();

    // Skills
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<SkillEffect> SkillEffects => Set<SkillEffect>();
    public DbSet<SkillBaseStats> SkillBaseStats => Set<SkillBaseStats>();
    public DbSet<SkillModifier> SkillModifiers => Set<SkillModifier>();
    public DbSet<SkillLore> SkillLore => Set<SkillLore>();
    public DbSet<SkillScalingModifier> SkillScalingModifiers => Set<SkillScalingModifier>();

    // Hero detail
    public DbSet<HeroBaseStats> HeroBaseStats => Set<HeroBaseStats>();
    public DbSet<HeroModifier> HeroModifiers => Set<HeroModifier>();
    public DbSet<HeroLore> HeroLore => Set<HeroLore>();
    public DbSet<HeroClass> HeroClasses => Set<HeroClass>();
    public DbSet<HeroProgressionModifiers> HeroProgressionModifiers => Set<HeroProgressionModifiers>();

    // Affiliation reference data (lookup tables — seeded once)
    public DbSet<MythologyType> MythologyTypes => Set<MythologyType>();
    public DbSet<OriginCulture> OriginCultures => Set<OriginCulture>();
    public DbSet<OriginArchetype> OriginArchetypes => Set<OriginArchetype>();

    // Versioning
    public DbSet<GameVersion> GameVersions => Set<GameVersion>();
  }
}