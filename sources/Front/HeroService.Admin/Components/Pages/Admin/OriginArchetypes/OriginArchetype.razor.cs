using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.OriginArchetypes;

public sealed partial class OriginArchetype : ComponentBase
{
  [Inject]
  private IOriginArchetypeClient Client { get; set; } = default!;

  [Inject]
  private IDialogService DialogService { get; set; } = default!;

  [Inject]
  private ISnackbar Snackbar { get; set; } = default!;

  [Inject]
  private ILogger<OriginArchetype> Logger { get; set; } = default!;

  private IReadOnlyList<OriginArchetypeDto> Items = [];
  private bool IsLoading = true;
  private string? ErrorMessage;

  protected override Task OnInitializedAsync() => Load();

  private async Task Load()
  {
    IsLoading = true;
    ErrorMessage = null;

    try
    {
      Items = await Client.GetAllAsync();
    }
    catch (OperationCanceledException exception)
    {
      Items = [];
      ErrorMessage = "The request was cancelled or timed out. Retry when the service is available.";
      Logger.LogWarning(exception, "Loading origin archetypes was cancelled.");
    }
    catch (HttpRequestException exception)
    {
      Items = [];
      ErrorMessage = "The HeroService API could not be reached. Check the service connection and try again.";
      Logger.LogWarning(exception, "Unable to load origin archetypes from HeroService.");
    }
    catch (Exception exception)
    {
      Items = [];
      ErrorMessage = "An unexpected error occurred while loading origin archetypes.";
      Logger.LogError(exception, "Unexpected failure while loading origin archetypes.");
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task Create()
  {
    var dialog = await DialogService.ShowAsync<OriginArchetypeDialog>("Create Origin Archetype");
    var result = await dialog.Result;

    if (result is { Canceled: false })
      await Load();
  }

  private async Task Rename(OriginArchetypeDto item)
  {
    var parameters = new DialogParameters { ["Item"] = item };
    var dialog = await DialogService.ShowAsync<OriginArchetypeDialog>(
        "Rename Origin Archetype",
        parameters);
    var result = await dialog.Result;

    if (result is { Canceled: false })
      await Load();
  }

  private async Task Retire(OriginArchetypeDto item)
  {
    var confirmed = await DialogService.ShowMessageBoxAsync(
        title: "Retire origin archetype?",
        message: $"'{item.Name}' will be retired and will no longer be available for new assignments.",
        yesText: "Retire",
        cancelText: "Cancel");

    if (confirmed != true)
      return;

    try
    {
      await Client.RetireAsync(item.Id);
      Snackbar.Add($"'{item.Name}' was retired.", Severity.Success);
      await Load();
    }
    catch (OperationCanceledException exception)
    {
      Logger.LogWarning(exception, "Retiring origin archetype {OriginArchetypeId} was cancelled.", item.Id);
      Snackbar.Add("The retirement request was cancelled. No change was confirmed.", Severity.Warning);
    }
    catch (HttpRequestException exception)
    {
      Logger.LogWarning(exception, "Unable to retire origin archetype {OriginArchetypeId}.", item.Id);
      Snackbar.Add("The origin archetype could not be retired. Check the service connection and try again.", Severity.Error);
    }
    catch (Exception exception)
    {
      Logger.LogError(exception, "Unexpected failure while retiring origin archetype {OriginArchetypeId}.", item.Id);
      Snackbar.Add("An unexpected error occurred while retiring the origin archetype.", Severity.Error);
    }
  }
}
