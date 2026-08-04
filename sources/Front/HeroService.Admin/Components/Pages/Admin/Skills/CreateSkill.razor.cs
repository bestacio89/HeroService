using HeroService.Admin.Components.Models;
using HeroService.Admin.Mappers;
using HeroService.Client.Http.Abstractions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Skills;

public sealed partial class CreateSkill : ComponentBase
{
  [Inject]
  private ISkillClient Client { get; set; } = default!;


  [Inject]
  private NavigationManager Nav { get; set; } = default!;


  private CreateSkillModel Model = new();


  private async Task Save()
  {
    var command = Model.ToCommand();

    await Client.CreateSkillAsync(command);

    Nav.NavigateTo("/admin/skills");
  }
}