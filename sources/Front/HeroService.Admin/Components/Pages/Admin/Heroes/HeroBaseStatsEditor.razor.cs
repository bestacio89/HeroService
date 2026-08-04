using HeroService.Contracts.DTOs.Requests;
using Microsoft.AspNetCore.Components;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class HeroBaseStatsEditor : ComponentBase
{
  [Parameter]
  [EditorRequired]
  public HeroCreateRequestDto Model { get; set; } = default!;
}