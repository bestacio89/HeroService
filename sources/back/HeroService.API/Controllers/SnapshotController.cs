using Franz.Common.Mediator.Dispatchers;
using Microsoft.AspNetCore.Mvc;
using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Contracts.Queries.Snapshots;

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

  // --------------------------------------------------
  // SINGLE HERO SNAPSHOT
  // --------------------------------------------------
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

  // --------------------------------------------------
  // ALL HERO SNAPSHOTS (BROWSE)
  // --------------------------------------------------
  [HttpGet("browse/{gameVersionId:guid}")]
  public async Task<ActionResult<IReadOnlyList<HeroSnapshotDto>>> Browse(
      Guid gameVersionId,
      CancellationToken ct)
  {
    var result = await _mediator.SendAsync(
        new BrowseHeroSnapshotsQuery(gameVersionId),
        ct);

    return Ok(result);
  }
}