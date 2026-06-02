using Franz.Common.Mediator.Dispatchers;
using HeroService.Contracts.Commands.Heroes;
using HeroService.Contracts.Queries.Heroes;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.Api.Controllers;

[ApiController]
[Route("api/v1/hero-progression")]
public sealed class HeroProgressionController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public HeroProgressionController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  [HttpGet("hero/{heroId:guid}")]
  public async Task<IActionResult> GetByHero(Guid heroId, CancellationToken ct)
  {
    var result = await _dispatcher.SendAsync(
        new GetHeroProgressionByHeroQuery(heroId),
        ct);

    return result == null ? NotFound() : Ok(result);
  }

  [HttpPost]
  public async Task<IActionResult> Define([FromBody] DefineHeroProgressionCommand command, CancellationToken ct)
  {
    var result = await _dispatcher.SendAsync(command, ct);
    return CreatedAtAction(
        nameof(GetByHero),
        new { heroId = result },
        result);
  }
}