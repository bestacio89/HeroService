using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Modifiers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.SkillModifiers;

public partial class CreateSkillModifierDialog
{
  [CascadingParameter]
  private IMudDialogInstance Dialog { get; set; } = default!;


  [Inject]
  private ISkillModifierClient Client { get; set; } = default!;


  private CreateSkillModifierCommand Model { get; set; } = new();


  private bool Saving { get; set; }


  private async Task Create()
  {
    Saving = true;

    try
    {
      await Client.CreateAsync(Model);

      Dialog.Close(DialogResult.Ok(true));
    }
    finally
    {
      Saving = false;
    }
  }


  private void Cancel()
  {
    Dialog.Cancel();
  }
}