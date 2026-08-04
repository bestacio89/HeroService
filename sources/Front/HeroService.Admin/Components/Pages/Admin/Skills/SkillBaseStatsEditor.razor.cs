using HeroService.Admin.Components.Models;
using Microsoft.AspNetCore.Components;

namespace HeroService.Admin.Components.Pages.Admin.Skills;

public sealed partial class SkillBaseStatsEditor : ComponentBase
{
  [Parameter]
  [EditorRequired]
  public CreateSkillModel Model { get; set; } = default!;
}