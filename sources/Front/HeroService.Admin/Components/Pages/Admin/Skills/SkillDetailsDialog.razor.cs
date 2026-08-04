using HeroService.Contracts.DTOs.Skills;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Skills;

public sealed partial class SkillDetailsDialog : ComponentBase
{
  [CascadingParameter]
  private IMudDialogInstance MudDialog { get; set; } = default!;


  [Parameter]
  [EditorRequired]
  public SkillDto Skill { get; set; } = default!;


  private void Close()
  {
    MudDialog.Close();
  }
}