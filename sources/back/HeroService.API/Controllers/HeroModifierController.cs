using Franz.Common.Mediator;
using Franz.Common.Mediator.Dispatchers;
using HeroService.Contracts.Commands.Modifiers;
using HeroService.Contracts.DTOs.Modifiers;
using HeroService.Contracts.Queries.Modifiers;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.Api.Controllers.Modifiers;

[ApiController]
[Route("hero-modifiers")]
public sealed class HeroModifiersController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public HeroModifiersController(
        IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }


    // =========================================================
    // CREATE
    // =========================================================

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateHeroModifierCommand command,
        CancellationToken cancellationToken)
    {
        var id =
            await _dispatcher.SendAsync(
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
      [FromBody] UpdateHeroModifierCommand request,
      CancellationToken cancellationToken)
  {
    request.HeroModifierId = id;

    await _dispatcher.SendAsync(
        request,
        cancellationToken);


    return NoContent();
  }

  // =========================================================
  // GET ACTIVE MODIFIER FOR HERO
  // =========================================================

  [HttpGet("hero/{heroId:guid}")]
  public async Task<ActionResult<HeroModifierDto>> GetByHero(
      Guid heroId,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetHeroModifierByHeroQuery(heroId),
        cancellationToken);

    return Ok(result);
  }


  // =========================================================
  // GET ALL ACTIVE VERSION MODIFIERS
  // =========================================================

  [HttpGet]
  public async Task<ActionResult<IReadOnlyList<HeroModifierDto>>> GetActiveVersionModifiers(
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetActiveHeroModifiersQuery(),
        cancellationToken);

    return Ok(result);
  }
}