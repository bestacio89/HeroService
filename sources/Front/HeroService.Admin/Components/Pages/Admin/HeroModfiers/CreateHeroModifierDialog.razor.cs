using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Modifiers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.HeroModifiers;

public partial class CreateHeroModifierDialog
{
  [CascadingParameter]
  private IMudDialogInstance Dialog { get; set; } = default!;


  [Inject]
  private IHeroModifierClient HeroModifierClient { get; set; } = default!;


  [Inject]
  private IGameVersionClient GameVersionClient { get; set; } = default!;


  private CreateHeroModifierCommand Model { get; set; } = new();


  private bool Saving { get; set; }


  protected override async Task OnInitializedAsync()
  {
    var activeVersion = await GameVersionClient.GetActiveAsync();

    if (activeVersion is null)
    {
      throw new InvalidOperationException(
          "Cannot create hero modifier without an active game version.");
    }

    Model.GameVersionId = activeVersion.Id;
  }


  private async Task CreateModifier()
  {
    Saving = true;

    try
    {
      await HeroModifierClient.CreateAsync(Model);

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