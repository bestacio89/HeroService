using HeroService.Contracts.Commands.Modifiers;
using Microsoft.AspNetCore.Components;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class HeroModifierStatsEditor : ComponentBase
{
  [Parameter]
  [EditorRequired]
  public CreateHeroModifierCommand Model { get; set; } = default!;
}