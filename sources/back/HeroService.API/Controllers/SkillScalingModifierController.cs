using Franz.Common.Mediator.Dispatchers;
using HeroService.Contracts.Commands.Skills;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Queries.Skills;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.Api.Controllers;



[ApiController]
[Route("api/v1/skill-scaling")]
public sealed class SkillScalingModifierController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public SkillScalingModifierController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  [HttpGet("skill/{skillId:guid}")]
  public async Task<IActionResult> GetBySkill(Guid skillId, CancellationToken ct)
  {
    var result = await _dispatcher.SendAsync(
        new GetSkillScalingBySkillQuery(skillId),
        ct);

    return result == null ? NotFound() : Ok(result);
  }

  [HttpPost]
  public async Task<IActionResult> Define([FromBody] DefineSkillScalingCommand command, CancellationToken ct)
  {
    var result = await _dispatcher.SendAsync(command, ct);
    return CreatedAtAction(
        nameof(GetBySkill),
        new { skillId = result},
        result);
  }
}