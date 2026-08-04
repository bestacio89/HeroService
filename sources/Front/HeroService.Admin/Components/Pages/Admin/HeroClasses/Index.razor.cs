using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.HeroClasses;

public sealed partial class Index : ComponentBase
{
  [Inject]
  private IHeroClassClient Client { get; set; } = default!;

  [Inject]
  private IDialogService DialogService { get; set; } = default!;

  private IReadOnlyCollection<HeroClassDto> Items = [];

  private bool IsLoading = true;

  protected override async Task OnInitializedAsync()
  {
    await Load();
  }

  private async Task Load()
  {
    IsLoading = true;

    try
    {
      Items = await Client.GetAllAsync();
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task OpenCreateDialog()
  {
    var parameters = new DialogParameters
        {
            { "OnSaved", EventCallback.Factory.Create(this, Load) }
        };

    var options = new DialogOptions
    {
      CloseButton = true,
      FullWidth = true,
      MaxWidth = MaxWidth.Small
    };

    await DialogService.ShowAsync<HeroClassDialog>(
        "Create Hero Class",
        parameters,
        options);
  }

  private void OpenDetails(HeroClassDto item)
  {
    // Placeholder for future details page/dialog.
  }
}