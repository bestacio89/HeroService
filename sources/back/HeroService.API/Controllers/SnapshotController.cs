using Franz.Common.Business.Repositories;
using HeroService.Application.Heroes.Versioned.Snapshotting;
using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.Api.Controllers;

[ApiController]
[Route("api/snapshots")]
public sealed class SnapshotController : ControllerBase
{
  private readonly SnapshotResolver _snapshotResolver;
  private readonly IEntityRepository<Hero,Guid> _heroRepository;
  private readonly ISkillRepository _skillRepository;
  private readonly ISkillBaseStatsRepository _skillBaseStatsRepository;
  private readonly IHeroModifierRepository _heroModifierRepository;
  private readonly ISkillModifierRepository _skillModifierRepository;

  public SnapshotController(
      SnapshotResolver snapshotResolver,
      IEntityRepository<Hero,Guid> heroRepository,
      ISkillRepository skillRepository,
      ISkillBaseStatsRepository skillBaseStatsRepository,
      IHeroModifierRepository heroModifierRepository,
      ISkillModifierRepository skillModifierRepository)
  {
    _snapshotResolver = snapshotResolver;
    _heroRepository = heroRepository;
    _skillRepository = skillRepository;
    _skillBaseStatsRepository = skillBaseStatsRepository;
    _heroModifierRepository = heroModifierRepository;
    _skillModifierRepository = skillModifierRepository;
  }

  [HttpPost("hero")]
  public async Task<ActionResult<HeroSnapshot>> CreateHeroSnapshot(
      [FromBody] HeroSnapshotRequest request,
      CancellationToken ct)
  {
    // 1. Load hero aggregate
    var hero = await _heroRepository.GetByIdAsync(request.HeroId, ct);

    // 2. Load related data
    var baseStats = hero.BaseStats;

    var heroModifier = await _heroModifierRepository.GetByHeroIdsAsync(request.HeroId, ct);

    var skills = await _skillRepository.GetByHeroIdAsync(request.HeroId, ct);

    var skillBaseStats = await _skillBaseStatsRepository
        .GetBySkillIdsAsync(skills.Select(s => s.Id).ToList(), ct);

    var skillModifiers = await _skillModifierRepository
        .GetBySkillIdsAsync(skills.Select(s => s.Id).ToList(), ct);

    // 3. Resolve snapshot (pure computation)
    var snapshot = _snapshotResolver.ResolveHero(
        hero.Id,
        request.GameVersionId,
        baseStats,
        heroModifier,
        skills,
        skillBaseStats,
        skillModifiers
    );

    return Ok(snapshot);
  }
}