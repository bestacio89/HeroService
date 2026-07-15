using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Modifiers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.HeroModifiers;

public partial class HeroModifierList
{
  [Inject]
  public IHeroModifierClient Client { get; set; } = default!;


  [Inject]
  public IHeroClient HeroClient { get; set; } = default!;


  [Inject]
  public IGameVersionClient GameVersionClient { get; set; } = default!;


  [Inject]
  public IDialogService DialogService { get; set; } = default!;


  private IReadOnlyList<HeroModifierDto> Modifiers { get; set; }
      = [];


  private Dictionary<Guid, string> HeroNames { get; set; } = [];


  private string ActiveGameVersion { get; set; } = "-";


  private bool Loading { get; set; }


  protected override async Task OnInitializedAsync()
  {
    await LoadAsync();
  }


  private async Task LoadAsync()
  {
    Loading = true;

    try
    {
      Modifiers = await Client.GetActiveAsync();


      var heroes = await HeroClient.BrowseAsync();


      HeroNames = heroes.ToDictionary(
          hero => hero.Id,
          hero => hero.Name);


      var version = await GameVersionClient.GetActiveAsync();


      ActiveGameVersion = version.VersionName ?? "-";
    }
    finally
    {
      Loading = false;
    }
  }


  private string GetHeroName(Guid heroId)
  {
    return HeroNames.TryGetValue(
        heroId,
        out var name)
            ? name
            : heroId.ToString();
  }


  private static string DisplayMultiplier(float? value)
  {
    return value.HasValue
        ? $"{value.Value:0.##}x"
        : "-";
  }


  private async Task OpenCreateDialog()
  {
    var dialog = await DialogService.ShowAsync<CreateHeroModifierDialog>(
        "Create Hero Modifier");


    var result = await dialog.Result;


    if (!result.Canceled)
    {
      await LoadAsync();
    }
  }


  private async Task OpenEditDialog(HeroModifierDto modifier)
  {
    var parameters = new DialogParameters
    {
      ["Modifier"] = modifier
    };


    var dialog = await DialogService.ShowAsync<EditHeroModifierDialog>(
        "Edit Hero Modifier",
        parameters);


    var result = await dialog.Result;


    if (!result.Canceled)
    {
      await LoadAsync();
    }
  }
}