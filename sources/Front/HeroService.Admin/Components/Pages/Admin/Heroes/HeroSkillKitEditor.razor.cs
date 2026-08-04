using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Requests;
using HeroService.Contracts.DTOs.Skills;
using Microsoft.AspNetCore.Components;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class HeroSkillKitEditor : ComponentBase
{
  [Inject]
  private ISkillClient SkillClient { get; set; } = default!;

  [Parameter]
  [EditorRequired]
  public HeroSkillKitCreateRequestDto Model { get; set; } = default!;

  private IReadOnlyCollection<SkillDto> Skills = [];

  protected override async Task OnInitializedAsync()
  {
    Skills = await SkillClient.GetAllAsync();
  }
}