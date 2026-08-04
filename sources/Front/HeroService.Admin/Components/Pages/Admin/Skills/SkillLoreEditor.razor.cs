using HeroService.Contracts.DTOs.Skills;
using Microsoft.AspNetCore.Components;

namespace HeroService.Admin.Components.Pages.Admin.Skills;

public sealed partial class SkillLoreEditor : ComponentBase
{
  [Parameter]
  [EditorRequired]
  public SkillLoreDto Model { get; set; } = default!;
}
