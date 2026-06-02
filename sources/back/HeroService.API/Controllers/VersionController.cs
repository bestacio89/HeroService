using Franz.Common.Mediator.Dispatchers;
using HeroService.Contracts.Commands.GameVersions;
using HeroService.Contracts.Queries.GameVersions;
using HeroService.Contracts.DTOs.GameVersions;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.Api.Controllers;

[ApiController]
[Route("api/v1/game-versions")]
public sealed class GameVersionController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public GameVersionController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  // =========================================================
  // CREATE + ACTIVATE NEW VERSION
  // =========================================================
  [HttpPost]
  public async Task<ActionResult<Guid>> Create(
      [FromBody] CreateGameVersionCommand command,
      CancellationToken cancellationToken)
  {
    var id = await _dispatcher.SendAsync(command, cancellationToken);
    return Ok(id);
  }


  // =========================================================
  // GET ACTIVE VERSION
  // =========================================================
  [HttpGet("active")]
  public async Task<ActionResult<GameVersionDto?>> GetActive(
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetActiveGameVersionQuery(),
        cancellationToken);

    return Ok(result);
  }

  // =========================================================
  // GET BY NAME
  // =========================================================
  [HttpGet("by-name/{name}")]
  public async Task<ActionResult<GameVersionDto>> GetByName(
      string name,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetGameVersionByNameQuery(name),
        cancellationToken);

    if (result is null)
      return NotFound();

    return Ok(result);
  }
}