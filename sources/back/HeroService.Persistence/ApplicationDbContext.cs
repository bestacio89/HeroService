using Franz.Common.Business.Domain;
using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Dispatchers;
using HeroService.Domain.Entities;
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

      // Configure Value Objects for Book
      modelBuilder.Entity<Book>(builder =>
      {
        builder.OwnsOne(b => b.Isbn, isbn =>
        {
          isbn.Property(p => p.Value)
              .HasColumnName("Isbn")
              .IsRequired();
        });

        builder.OwnsOne(b => b.Title, title =>
        {
          title.Property(p => p.Value)
              .HasColumnName("Title")
              .IsRequired();
        });

        builder.OwnsOne(b => b.Author, author =>
        {
          author.Property(p => p.Value)
              .HasColumnName("Author")
              .IsRequired();
        });
      });

      // Configure Value Objects for Member
      modelBuilder.Entity<Member>(builder =>
      {
        builder.OwnsOne(m => m.Name, name =>
        {
          name.Property(p => p.Value)
              .HasColumnName("Name")
              .IsRequired();
        });

        builder.OwnsOne(m => m.Email, email =>
        {
          email.Property(p => p.Value)
              .HasColumnName("Email")
              .IsRequired();
        });
      });

      // Apply seeders (if any)
      // modelBuilder.ApplyConfiguration(new BookSeeder());
      // modelBuilder.ApplyConfiguration(new MemberSeeder());
    }

    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;

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



    // Later: DbSet<Loan>, DbSet<Reservation>, etc.
  }
}
