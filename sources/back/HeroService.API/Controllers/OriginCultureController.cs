using Franz.Common.Mediator.Dispatchers;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginCulture;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Queries.Heroes.Classification.OriginCultures;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.Api.Controllers;

[ApiController]
[Route("api/v1/origin-cultures")]
public sealed class OriginCultureController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public OriginCultureController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  [HttpGet]
  public async Task<ActionResult<IReadOnlyList<OriginCultureDto>>> GetAll(
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetAllOriginCulturesQuery(),
        cancellationToken);

    return Ok(result);
  }

  [HttpGet("{id:guid}")]
  public async Task<ActionResult<OriginCultureDto>> GetById(
      Guid id,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetOriginCultureByIdQuery(id),
        cancellationToken);

    return result is null ? NotFound() : Ok(result);
  }

  [HttpGet("by-name/{name}")]
  public async Task<ActionResult<OriginCultureDto>> GetByName(
      string name,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetOriginCultureByNameQuery(name),
        cancellationToken);

    return result is null ? NotFound() : Ok(result);
  }

  [HttpPost]
  public async Task<ActionResult<Guid>> Create(
      CreateOriginCultureCommand command,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(command, cancellationToken);
    return Ok(result);
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Rename(
      Guid id,
      RenameOriginCultureCommand command,
      CancellationToken cancellationToken)
  {
    await _dispatcher.SendAsync(command with { Id = id }, cancellationToken);
    return NoContent();
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Retire(
      Guid id,
      CancellationToken cancellationToken)
  {
    await _dispatcher.SendAsync(
        new RetireOriginCultureCommand(id),
        cancellationToken);

    return NoContent();
  }
}