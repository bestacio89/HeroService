using Franz.Common.Mediator;
using Microsoft.AspNetCore.Mvc;
using HeroService.Contracts.Queries.Snapshots;
using HeroService.Contracts.DTOs.Snapshots;
using Franz.Common.Mediator.Dispatchers;

namespace HeroService.Api.Controllers;

[ApiController]
[Route("api/v1/snapshots/heroes")]
public sealed class HeroSnapshotController : ControllerBase
{
  private readonly IDispatcher _mediator;

  public HeroSnapshotController(IDispatcher mediator)
  {
    _mediator = mediator;
  }

  [HttpGet("{heroId:guid}/{gameVersionId:guid}")]
  public async Task<ActionResult<HeroSnapshotDto>> Get(
      Guid heroId,
      Guid gameVersionId,
      CancellationToken ct)
  {
    var result = await _mediator.SendAsync(
        new GetHeroSnapshotQuery(heroId, gameVersionId),
        ct);

    return Ok(result);
  }
}