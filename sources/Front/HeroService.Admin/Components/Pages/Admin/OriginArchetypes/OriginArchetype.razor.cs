using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.OriginArchetypes;

public sealed partial class OriginArchetype : ComponentBase
{
  [Inject]
  private IOriginArchetypeClient Client { get; set; } = default!;

  [Inject]
  private IDialogService DialogService { get; set; } = default!;


  private IReadOnlyList<OriginArchetypeDto> Items = [];

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
    var dialog = await DialogService.ShowAsync<OriginArchetypeDialog>(
        "Create Archetype");

    await dialog.Result;

    await Load();
  }


  private async Task Rename(OriginArchetypeDto item)
  {
    var parameters = new DialogParameters
    {
      ["Item"] = item
    };


    var dialog = await DialogService.ShowAsync<OriginArchetypeDialog>(
        "Rename Archetype",
        parameters);


    await dialog.Result;

    await Load();
  }


  private async Task Retire(OriginArchetypeDto item)
  {
    await Client.RetireAsync(item.Id);

    await Load();
  }
}