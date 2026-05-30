using Franz.Common.Mediator.Dispatchers;
using HeroService.Contracts.Commands.Heroes.Classifications.NewFolder;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginArchetype;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Queries.Heroes.Classification.OriginArchetypes;
using Microsoft.AspNetCore.Mvc;

namespace HeroService.Api.Controllers.Classifications;

[ApiController]
[Route("api/v1/origin-archetypes")]
public sealed class OriginArchetypeController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public OriginArchetypeController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  [HttpGet]
  public async Task<ActionResult<IReadOnlyList<OriginArchetypeDto>>> GetAll(
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetAllOriginArchetypesQuery(),
        cancellationToken);

    return Ok(result);
  }

  [HttpGet("{id:guid}")]
  public async Task<ActionResult<OriginArchetypeDto>> GetById(
      Guid id,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetOriginArchetypeByIdQuery(id),
        cancellationToken);

    return result is null ? NotFound() : Ok(result);
  }

  [HttpGet("by-name/{name}")]
  public async Task<ActionResult<OriginArchetypeDto>> GetByName(
      string name,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(
        new GetOriginArchetypeByNameQuery(name),
        cancellationToken);

    return result is null ? NotFound() : Ok(result);
  }

  [HttpPost]
  public async Task<ActionResult<Guid>> Create(
      CreateOriginArchetypeCommand command,
      CancellationToken cancellationToken)
  {
    var result = await _dispatcher.SendAsync(command, cancellationToken);
    return Ok(result);
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Rename(
      Guid id,
      RenameOriginArchetypeCommand command,
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
        new RetireOriginArchetypeCommand(id),
        cancellationToken);

    return NoContent();
  }
}