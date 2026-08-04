using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.OriginCultures;

public sealed partial class OriginCultures : ComponentBase
{
  [Inject]
  private IOriginCultureClient Client { get; set; } = default!;


  [Inject]
  private IDialogService DialogService { get; set; } = default!;


  private IReadOnlyList<OriginCultureDto> Items = [];


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


  private async Task Create()
  {
    var dialog = await DialogService.ShowAsync<OriginCultureDialog>(
        "Create Culture");

    await dialog.Result;

    await Load();
  }


  private async Task Rename(OriginCultureDto item)
  {
    var parameters = new DialogParameters
    {
      ["Item"] = item
    };


    var dialog = await DialogService.ShowAsync<OriginCultureDialog>(
        "Rename Culture",
        parameters);


    await dialog.Result;

    await Load();
  }


  private async Task Retire(OriginCultureDto item)
  {
    await Client.RetireAsync(item.Id);

    await Load();
  }
}