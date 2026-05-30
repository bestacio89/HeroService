using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

public sealed class SkillModifierSeeder : ISeeder2
{
  public int Order => 5;
  private readonly IEntityFactory<Guid, SkillModifier> _factory;
  private readonly IEntityRepository<SkillModifier, Guid> _repo;
  private readonly ISkillRepository _skills;
  private readonly IGameVersionRepository _versions;
  private readonly IUnitOfWork _uow;

  public SkillModifierSeeder(
      IEntityFactory<Guid, SkillModifier> factory,
      IEntityRepository<SkillModifier, Guid> repo,
      ISkillRepository skills,
      IGameVersionRepository versions,
      IUnitOfWork uow)
  {
    _factory = factory;
    _repo = repo;
    _skills = skills;
    _versions = versions;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _repo.GetAllAsync(ct)).Any())
      return;

    var system = "seed-system";

    var version = await _versions.GetByNameAsync("1.0.0", ct)
        ?? throw new InvalidOperationException("Missing GameVersion 1.0.0");

    // =====================================================
    // LOAD SKILLS (FAIL FAST)
    // =====================================================

    // THOR
    var mjolnir = await GetSkill("Mjolnir Strike", ct);
    var thunderLeap = await GetSkill("Thunder Leap", ct);
    var stormAura = await GetSkill("Storm Aura", ct);
    var lightningChain = await GetSkill("Lightning Chain", ct);
    var thorUlt = await GetSkill("God of Thunder", ct);

    // HERAKLES
    var lionsMight = await GetSkill("Lion's Might", ct);
    var hydraStrike = await GetSkill("Hydra Strike", ct);
    var titanGrip = await GetSkill("Titan Grip", ct);
    var laborsRush = await GetSkill("Labors Rush", ct);
    var heraUlt = await GetSkill("Divine Endurance", ct);

    // =====================================================
    // THOR MODIFIERS
    // =====================================================

    Create(version.Id, mjolnir.Id,
      dmg: 1.10f, cd: 0.95f);

    Create(version.Id, thunderLeap.Id,
      dmg: 1.00f, cc: 1.10f, range: 1.05f);

    Create(version.Id, stormAura.Id,
      heal: 1.00f, shield: 1.15f);

    Create(version.Id, lightningChain.Id,
      dmg: 1.05f, range: 1.10f);

    Create(version.Id, thorUlt.Id,
      dmg: 1.20f, cd: 1.10f, cc: 1.15f);

    // =====================================================
    // HERAKLES MODIFIERS
    // =====================================================

    Create(version.Id, lionsMight.Id,
      dmg: 1.05f, shield: 1.10f);

    Create(version.Id, hydraStrike.Id,
      dmg: 1.15f);

    Create(version.Id, titanGrip.Id,
      shield: 1.25f);

    Create(version.Id, laborsRush.Id,
      cd: 0.90f, range: 1.05f);

    Create(version.Id, heraUlt.Id,
      shield: 1.30f, cd: 1.15f);

    await _uow.CommitAsync(ct);
  }

  private SkillModifier Create(
      Guid versionId,
      Guid skillId,
      float cd = 1f,
      float mana = 1f,
      float dmg = 1f,
      float heal = 1f,
      float shield = 1f,
      float cast = 1f,
      float channel = 1f,
      float cc = 1f,
      float range = 1f)
  {
    var mod = _factory.Create();

    mod.Define(
      versionId,
      skillId,
      cd,
      mana,
      dmg,
      heal,
      shield,
      cast,
      channel,
      cc,
      range,
      "seed-system"
    );

    _repo.AddAsync(mod, CancellationToken.None); // intentionally fire-and-forget style inside seeder

    return mod;
  }

  private async Task<Skill> GetSkill(string name, CancellationToken ct)
  {
    return await _skills.GetByNameAsync(name, ct)
        ?? throw new InvalidOperationException($"Missing skill: {name}");
  }
}