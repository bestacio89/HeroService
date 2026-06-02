using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

public sealed class SkillScalingSeeder : ISeeder2
{
  public int Order => 7;

  private readonly IEntityFactory<Guid, SkillScalingModifier> _factory;
  private readonly IEntityRepository<SkillScalingModifier, Guid> _repo;
  private readonly ISkillRepository _skills; // Domain interface inheriting from INameLookupRepository
  private readonly IUnitOfWork _uow;

  public SkillScalingSeeder(
      IEntityFactory<Guid, SkillScalingModifier> factory,
      IEntityRepository<SkillScalingModifier, Guid> repo,
      ISkillRepository skills,
      IUnitOfWork uow)
  {
    _factory = factory;
    _repo = repo;
    _skills = skills;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    // Idempotency Boundary Check
    if ((await _repo.GetAllAsync(ct)).Any())
      return;

    var system = "seed-system";

    // THOR SKILLS (Resolved cleanly via the inherited INameLookupRepository method)
    var mjolnir = await GetSkillOrFailAsync("Mjolnir Strike", ct);
    var thunderLeap = await GetSkillOrFailAsync("Thunder Leap", ct);
    var stormAura = await GetSkillOrFailAsync("Storm Aura", ct);
    var lightningChain = await GetSkillOrFailAsync("Lightning Chain", ct);
    var thorUlt = await GetSkillOrFailAsync("God of Thunder", ct);

    // HERAKLES SKILLS
    var lionsMight = await GetSkillOrFailAsync("Lion's Might", ct);
    var hydraStrike = await GetSkillOrFailAsync("Hydra Strike", ct);
    var titanGrip = await GetSkillOrFailAsync("Titan Grip", ct);
    var laborsRush = await GetSkillOrFailAsync("Labors Rush", ct);
    var heraUlt = await GetSkillOrFailAsync("Divine Endurance", ct);

    // =========================================================
    // THOR SCALING PROFILES (AD & AP Focus)
    // =========================================================
    await CreateAsync(mjolnir.Id, attackDamageRatio: 1.2f, abilityPowerRatio: 0.2f, maxHealthRatio: 0.0f, system, ct);
    await CreateAsync(thunderLeap.Id, attackDamageRatio: 0.8f, abilityPowerRatio: 0.4f, maxHealthRatio: 0.0f, system, ct);
    await CreateAsync(stormAura.Id, attackDamageRatio: 0.0f, abilityPowerRatio: 0.6f, maxHealthRatio: 0.05f, system, ct);
    await CreateAsync(lightningChain.Id, attackDamageRatio: 0.3f, abilityPowerRatio: 0.9f, maxHealthRatio: 0.0f, system, ct);
    await CreateAsync(thorUlt.Id, attackDamageRatio: 1.5f, abilityPowerRatio: 1.1f, maxHealthRatio: 0.0f, system, ct);

    // =========================================================
    // HERAKLES SCALING PROFILES (AD & Max Health/Tank Focus)
    // =========================================================
    await CreateAsync(lionsMight.Id, attackDamageRatio: 0.9f, abilityPowerRatio: 0.0f, maxHealthRatio: 0.08f, system, ct);
    await CreateAsync(hydraStrike.Id, attackDamageRatio: 1.1f, abilityPowerRatio: 0.0f, maxHealthRatio: 0.04f, system, ct);
    await CreateAsync(titanGrip.Id, attackDamageRatio: 0.5f, abilityPowerRatio: 0.0f, maxHealthRatio: 0.12f, system, ct);
    await CreateAsync(laborsRush.Id, attackDamageRatio: 0.7f, abilityPowerRatio: 0.0f, maxHealthRatio: 0.02f, system, ct);
    await CreateAsync(heraUlt.Id, attackDamageRatio: 0.0f, abilityPowerRatio: 0.0f, maxHealthRatio: 0.20f, system, ct);

    // Single Transaction Commit Boundary
    await _uow.CommitAsync(ct);
  }

  private async Task CreateAsync(
      Guid skillId,
      float attackDamageRatio,
      float abilityPowerRatio,
      float maxHealthRatio,
      string createdBy,
      CancellationToken ct)
  {
    var profile = _factory.Create();
    profile.Define(skillId, attackDamageRatio, abilityPowerRatio, maxHealthRatio, createdBy);
    await _repo.AddAsync(profile, ct);
  }

  private async Task<Skill> GetSkillOrFailAsync(string name, CancellationToken ct) =>
      await _skills.GetByNameAsync(name, ct)
      ?? throw new InvalidOperationException($"Domain validation failed during seed. Missing Skill: {name}");
}