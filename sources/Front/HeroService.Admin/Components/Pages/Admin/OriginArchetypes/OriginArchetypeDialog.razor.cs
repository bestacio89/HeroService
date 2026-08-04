using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginArchetypes;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.OriginArchetypes;

public sealed partial class OriginArchetypeDialog : ComponentBase
{
  [Inject]
  private IOriginArchetypeClient Client { get; set; } = default!;


  [CascadingParameter]
  private IMudDialogInstance MudDialog { get; set; } = default!;


  [Parameter]
  public OriginArchetypeDto? Item { get; set; }


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
      var command = new RenameOriginArchetypeCommand(
          Item!.Id,
          Name);

      await Client.RenameAsync(
          Item.Id,
          command);
    }
    else
    {
      var command = new CreateOriginArchetypeCommand(Name);

      await Client.CreateAsync(command);
    }


    MudDialog.Close();
  }


  private void Cancel()
  {
    MudDialog.Cancel();
  }
}