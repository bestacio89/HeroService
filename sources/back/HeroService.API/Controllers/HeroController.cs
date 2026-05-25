using Franz.Common.Mediator.Dispatchers;
using HeroService.Contracts.Commands.Heroes;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.DTOs.Requests;
using HeroService.Contracts.Queries.Heroes;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.API.Controllers;

[ApiController]
[Route("api/heroes")]
public sealed class HeroController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public HeroController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  // =========================================================
  // GET: api/heroes/{id}
  // =========================================================
  [HttpGet("{heroId:guid}")]
  [ProducesResponseType(typeof(HeroDetailsDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetById(
      Guid heroId,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetHeroDetailsQuery(heroId),
        cancellationToken);

    return Ok(result);
  }

  // =========================================================
  // GET: api/heroes/by-name/{name}
  // =========================================================
  [HttpGet("by-name/{name}")]
  [ProducesResponseType(typeof(HeroDetailsDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetByName(
      string name,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetHeroByNameQuery(name),
        cancellationToken);

    return Ok(result);
  }

  // =========================================================
  // POST: api/heroes
  // =========================================================
  [HttpPost]
  [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Create(
      [FromBody] HeroCreateRequest request,
      CancellationToken cancellationToken)
  {
    var id = await _dispatcher.SendAsync(
        new CreateHeroCommand(request),
        cancellationToken);

    return CreatedAtAction(
        nameof(GetById),
        new { heroId = id },
        id);
  }
}