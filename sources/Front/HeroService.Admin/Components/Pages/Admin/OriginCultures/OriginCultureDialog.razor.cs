using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginCulture;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.OriginCultures;

public sealed partial class OriginCultureDialog : ComponentBase
{
  [Inject]
  private IOriginCultureClient Client { get; set; } = default!;


  [CascadingParameter]
  private IMudDialogInstance MudDialog { get; set; } = default!;


  [Parameter]
  public OriginCultureDto? Item { get; set; }


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
      var command = new RenameOriginCultureCommand(
          Item!.Id,
          Name);

      await Client.RenameAsync(
          Item.Id,
          command);
    }
    else
    {
      var command = new CreateOriginCultureCommand(Name);

      await Client.CreateAsync(command);
    }


    MudDialog.Close();
  }


  private void Cancel()
  {
    MudDialog.Cancel();
  }
}