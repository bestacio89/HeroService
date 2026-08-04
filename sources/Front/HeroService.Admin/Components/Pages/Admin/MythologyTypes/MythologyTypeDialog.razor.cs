using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Heroes.Classifications.MythologyTypes;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.MythologyTypes;

public sealed partial class MythologyTypeDialog : ComponentBase
{
  [Inject]
  private IMythologyTypeClient Client { get; set; } = default!;

  [CascadingParameter]
  private IMudDialogInstance MudDialog { get; set; } = default!;

  [Parameter]
  public MythologyTypeDto? Item { get; set; }

  private string Name = string.Empty;

  private bool IsEdit => Item is not null;

  protected override void OnInitialized()
  {
    if (Item is not null)
    {
      Name = Item.Name;
    }
  }

  private async Task Save()
  {
    if (IsEdit)
    {
      var command = new RenameMythologyTypeCommand(Item!.Id, Name);

      await Client.RenameAsync(Item.Id, command);
    }
    else
    {
      var command = new CreateMythologyTypeCommand(Name);

      await Client.CreateAsync(command);
    }

    MudDialog.Close();
  }

  private void Cancel()
  {
    MudDialog.Cancel();
  }
}