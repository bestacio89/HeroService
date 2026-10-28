using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Modifiers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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

  [Inject]
  public ILogger<HeroModifierList> Logger { get; set; } = default!;


  private IReadOnlyList<HeroModifierDto> Modifiers { get; set; }
      = [];


  private Dictionary<Guid, string> HeroNames { get; set; } = [];


  private string ActiveGameVersion { get; set; } = "-";


  private bool Loading { get; set; }

  private string? ErrorMessage { get; set; }


  protected override async Task OnInitializedAsync()
  {
    await LoadAsync();
  }


  private async Task LoadAsync()
  {
    Loading = true;
    ErrorMessage = null;

    try
    {
      var modifiersTask = Client.GetActiveAsync();
      var heroesTask = HeroClient.BrowseAsync();
      var versionTask = GameVersionClient.GetActiveAsync();

      await Task.WhenAll(modifiersTask, heroesTask, versionTask);

      Modifiers = await modifiersTask;
      HeroNames = (await heroesTask).ToDictionary(hero => hero.Id, hero => hero.Name);
      var version = await versionTask;
      ActiveGameVersion = version?.VersionName ?? "-";
    }
    catch (OperationCanceledException)
    {
      Modifiers = [];
      HeroNames = [];
      ErrorMessage = "The request was cancelled or timed out. Retry when the service is available.";
    }
    catch (HttpRequestException exception)
    {
      Logger.LogWarning(exception, "The HeroService API could not be reached while loading hero modifiers.");
      Modifiers = [];
      HeroNames = [];
      ErrorMessage = "The HeroService API could not be reached. Check the service connection and try again.";
    }
    catch (Exception exception)
    {
      Logger.LogError(exception, "Unexpected failure while loading hero modifiers.");
      Modifiers = [];
      HeroNames = [];
      ErrorMessage = "An unexpected error occurred while loading hero modifiers.";
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