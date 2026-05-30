using Franz.Common.Mediator.Dispatchers;
using HeroService.Contracts.Commands.Skills;
using HeroService.Contracts.Queries.Skills;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.Api.Controllers;

[ApiController]
[Route("api/v1/skills")]
public sealed class SkillController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public SkillController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  // =========================================================
  // CREATE SKILL
  // =========================================================
  [HttpPost]
  public async Task<ActionResult<Guid>> CreateSkill(
      [FromBody] CreateSkillCommand command,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(command, cancellationToken);
    return Ok(result);
  }

  // =========================================================
  // ADD EFFECT
  // =========================================================
  [HttpPost("{skillId:guid}/effects")]
  public async Task<ActionResult> AddEffect(
      Guid skillId,
      [FromBody] CreateSkillEffectCommand command,
      CancellationToken cancellationToken)
  {
    var enriched = command with { SkillId = skillId };

    var result = await _dispatcher.SendAsync(enriched, cancellationToken);

    if (!result.IsSuccess)
      return BadRequest(result.Error);

    return Ok(result.Value);
  }

  // =========================================================
  // UPDATE LORE
  // =========================================================
  [HttpPut("{skillLoreId:guid}/lore")]
  public async Task<ActionResult> UpdateLore(
      Guid skillLoreId,
      [FromBody] UpdateSkillLoreCommand command,
      CancellationToken cancellationToken)
  {
    var enriched = command with { SkillLoreId = skillLoreId };

    await _dispatcher.SendAsync(enriched, cancellationToken);

    return NoContent();
  }

  // =========================================================
  // GET ALL SKILLS
  // =========================================================
  [HttpGet]
  public async Task<ActionResult<IReadOnlyCollection<SkillDto>>> GetAll(
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetAllSkillsQuery(),
        cancellationToken);

    return Ok(result);
  }

  // =========================================================
  // GET SKILL BY NAME
  // =========================================================
  [HttpGet("by-name/{name}")]
  public async Task<ActionResult<SkillDto>> GetByName(
      string name,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetSkillByNameQuery(name),
        cancellationToken);

    if (result is null)
      return NotFound();

    return Ok(result);
  }

  // =========================================================
  // GET SKILL DETAILS
  // =========================================================
  [HttpGet("{skillId:guid}")]
  public async Task<ActionResult<SkillDto>> GetDetails(
      Guid skillId,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetSkillDetailsQuery(skillId),
        cancellationToken);

    if (result is null)
      return NotFound();

    return Ok(result);
  }

  // =========================================================
  // GET SKILL LORE
  // =========================================================
  [HttpGet("{skillId:guid}/lore")]
  public async Task<ActionResult<SkillLoreDto>> GetLore(
      Guid skillId,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetSkillLoreQuery(skillId),
        cancellationToken);

    if (result is null)
      return NotFound();

    return Ok(result);
  }

  // =========================================================
  // GET SKILL BASE STATS
  // =========================================================
  [HttpGet("{skillId:guid}/base-stats")]
  public async Task<ActionResult<SkillBaseStatsDto>> GetBaseStats(
      Guid skillId,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetSkillBaseStatsQuery(skillId),
        cancellationToken);

    if (result is null)
      return NotFound();

    return Ok(result);
  }

  // =========================================================
  // GET BY TYPE
  // =========================================================
  [HttpGet("by-type/{skillType}")]
  public async Task<ActionResult<IReadOnlyCollection<SkillDto>>> GetByType(
      SkillType skillType,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetSkillsByTypeQuery(skillType),
        cancellationToken);

    return Ok(result);
  }
}