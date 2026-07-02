using Franz.Common.Mediator.Dispatchers;
using HeroService.Contracts.Commands.Heroes;
using HeroService.Contracts.Queries.Heroes.Classes;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.Api.Controllers;


[ApiController]
[Route("hero-classes")]
public sealed class HeroClassController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public HeroClassController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  // =========================================================
  // CREATE HERO CLASS
  // =========================================================
  [HttpPost]
  public async Task<ActionResult<Guid>> Create(
      [FromBody] CreateHeroClassCommand command,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(command, cancellationToken);
    return Ok(result);
  }

  // =========================================================
  // GET ALL HERO CLASSES
  // =========================================================
  [HttpGet]
  public async Task<ActionResult<IReadOnlyCollection<HeroClassDto>>> GetAll(
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetAllHeroClassesQuery(),
        cancellationToken);

    return Ok(result);
  }

  // =========================================================
  // GET BY ID
  // =========================================================
  [HttpGet("{id:guid}")]
  public async Task<ActionResult<HeroClassDto>> GetById(
      Guid id,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetHeroClassByIdQuery(id),
        cancellationToken);

    if (result is null)
      return NotFound();

    return Ok(result);
  }

  // =========================================================
  // GET BY NAME
  // =========================================================
  [HttpGet("by-name/{name}")]
  public async Task<ActionResult<HeroClassDto>> GetByName(
      string name,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetHeroClassByNameQuery(name),
        cancellationToken);

    if (result is null)
      return NotFound();

    return Ok(result);
  }
}