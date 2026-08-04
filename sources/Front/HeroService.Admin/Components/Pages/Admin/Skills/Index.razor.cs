using HeroService.Admin.Components.Pages.Admin.Skills;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Skills;

public sealed partial class Index : ComponentBase
{
  [Inject]
  private ISkillClient Client { get; set; } = default!;


  [Inject]
  private NavigationManager Nav { get; set; } = default!;


  [Inject]
  private IDialogService DialogService { get; set; } = default!;


  [Inject]
  private ISnackbar Snackbar { get; set; } = default!;


  private IReadOnlyCollection<SkillDto> Items = [];


  protected override async Task OnInitializedAsync()
  {
    Items = await Client.GetAllAsync();
  }


  private void NavigateToCreate()
  {
    Nav.NavigateTo("/admin/skills/create");
  }


  private async Task OpenDetailsAsync(SkillDto skill)
  {
    var details = await Client.GetDetailsAsync(skill.Id);

    if (details is null)
    {
      Snackbar.Add(
          "Skill details not found",
          Severity.Error);

      return;
    }


    var parameters = new DialogParameters<SkillDetailsDialog>
        {
            { x => x.Skill, details }
        };


    await DialogService.ShowAsync<SkillDetailsDialog>(
        details.Name,
        parameters);
  }
}