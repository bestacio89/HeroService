using Franz.Common.Mediator;
using Franz.Common.Mediator.Dispatchers;
using HeroService.Contracts.Commands.Modifiers;
using HeroService.Contracts.DTOs.Modifiers;
using HeroService.Contracts.Queries.Modifiers;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.Api.Controllers.Modifiers;

[ApiController]
[Route("skill-modifiers")]
public sealed class SkillModifiersController : ControllerBase
{
  private readonly IDispatcher _mediator;


  public SkillModifiersController(
      IDispatcher mediator)
  {
    _mediator = mediator;
  }


  // =========================================================
  // CREATE
  // =========================================================

  [HttpPost]
  public async Task<ActionResult<Guid>> Create(
      [FromBody] CreateSkillModifierCommand command,
      CancellationToken cancellationToken)
  {
    var id =
        await _mediator.SendAsync(
            command,
            cancellationToken);


    return Ok(id);
  }


  // =========================================================
  // UPDATE
  // =========================================================

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update(
      Guid id,
      [FromBody] UpdateSkillModifierCommand request,
      CancellationToken cancellationToken)
  {
    var command =
        new UpdateSkillModifierCommand(

        );


    await _mediator.SendAsync(
        command,
        cancellationToken);


    return NoContent();
  }

  // =========================================================
  // GET ACTIVE MODIFIER FOR SKILL
  // =========================================================

  [HttpGet("skill/{skillId:guid}")]
  public async Task<ActionResult<SkillModifierDto>> GetBySkill(
      Guid skillId,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.SendAsync(
        new GetSkillModifierBySkillQuery(skillId),
        cancellationToken);

    return Ok(result);
  }


  // =========================================================
  // GET ALL ACTIVE VERSION MODIFIERS
  // =========================================================

  [HttpGet]
  public async Task<ActionResult<IReadOnlyList<SkillModifierDto>>> GetActiveVersionModifiers(
      CancellationToken cancellationToken)
  {
    var result = await _mediator.SendAsync(
        new GetSkillModifiersByGameVersionQuery(),
        cancellationToken);

    return Ok(result);
  }

}