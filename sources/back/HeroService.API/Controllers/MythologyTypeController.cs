using Franz.Common.Mediator.Dispatchers;
using HeroService.Contracts.Commands.Heroes.Classifications.MythologyTypes;
using HeroService.Contracts.Queries.Heroes.Classification.MythologyTypes;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.Api.Controllers;

[ApiController]
[Route("api/v1/mythology-types")]
public sealed class MythologyTypeController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public MythologyTypeController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  [HttpPost]
  public async Task<ActionResult<Guid>> Create(
      CreateMythologyTypeCommand command,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(command, cancellationToken);
    return Ok(result);
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Rename(
      Guid id,
      RenameMythologyTypeCommand command,
      CancellationToken cancellationToken)
  {
    var updated = command with { Id = id };

    await _dispatcher.SendAsync(updated, cancellationToken);
    return NoContent();
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Retire(
      Guid id,
      CancellationToken cancellationToken)
  {
    await _dispatcher.SendAsync(
        new RetireMythologyTypeCommand(id),
        cancellationToken);

    return NoContent();
  }

  [HttpGet]
  public async Task<ActionResult<IReadOnlyList<MythologyTypeDto>>> GetAll(
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetAllMythologyTypesQuery(),
        cancellationToken);

    return Ok(result);
  }

  [HttpGet("{id:guid}")]
  public async Task<ActionResult<MythologyTypeDto>> GetById(
      Guid id,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetMythologyTypeByIdQuery(id),
        cancellationToken);

    return result is null ? NotFound() : Ok(result);
  }

  [HttpGet("by-name/{name}")]
  public async Task<ActionResult<MythologyTypeDto>> GetByName(
      string name,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetMythologyTypeByNameQuery(name),
        cancellationToken);

    return result is null ? NotFound() : Ok(result);
  }
}