using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.MythologyTypes;

public sealed partial class Index : ComponentBase
{
  [Inject]
  private IMythologyTypeClient MythologyTypeClient { get; set; } = default!;

  [Inject]
  private IDialogService DialogService { get; set; } = default!;

  [Inject]
  private ISnackbar Snackbar { get; set; } = default!;

  [Inject]
  private ILogger<Index> Logger { get; set; } = default!;

  private IReadOnlyList<MythologyTypeDto> Items = [];
  private bool IsLoading = true;
  private string? ErrorMessage;

  protected override Task OnInitializedAsync() => Load();

  private async Task Load()
  {
    IsLoading = true;
    ErrorMessage = null;

    try
    {
      Items = await MythologyTypeClient.GetAllAsync();
    }
    catch (OperationCanceledException exception)
    {
      Items = [];
      ErrorMessage = "The request was cancelled or timed out. Retry when the service is available.";
      Logger.LogWarning(exception, "Loading mythology types was cancelled.");
    }
    catch (HttpRequestException exception)
    {
      Items = [];
      ErrorMessage = "The HeroService API could not be reached. Check the service connection and try again.";
      Logger.LogWarning(exception, "Unable to load mythology types from HeroService.");
    }
    catch (Exception exception)
    {
      Items = [];
      ErrorMessage = "An unexpected error occurred while loading mythology types.";
      Logger.LogError(exception, "Unexpected failure while loading mythology types.");
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task Create()
  {
    var dialog = await DialogService.ShowAsync<MythologyTypeDialog>("Create Mythology Type");
    var result = await dialog.Result;

    if (result is { Canceled: false })
      await Load();
  }

  private async Task Rename(MythologyTypeDto item)
  {
    var parameters = new DialogParameters { ["Item"] = item };
    var dialog = await DialogService.ShowAsync<MythologyTypeDialog>(
        "Rename Mythology Type",
        parameters);
    var result = await dialog.Result;

    if (result is { Canceled: false })
      await Load();
  }

  private async Task Retire(MythologyTypeDto item)
  {
    var confirmed = await DialogService.ShowMessageBoxAsync(
        title: "Retire mythology type?",
        message: $"'{item.Name}' will be retired and will no longer be available for new assignments.",
        yesText: "Retire",
        cancelText: "Cancel");

    if (confirmed != true)
      return;

    try
    {
      await MythologyTypeClient.RetireAsync(item.Id);
      Snackbar.Add($"'{item.Name}' was retired.", Severity.Success);
      await Load();
    }
    catch (OperationCanceledException exception)
    {
      Logger.LogWarning(exception, "Retiring mythology type {MythologyTypeId} was cancelled.", item.Id);
      Snackbar.Add("The retirement request was cancelled. No change was confirmed.", Severity.Warning);
    }
    catch (HttpRequestException exception)
    {
      Logger.LogWarning(exception, "Unable to retire mythology type {MythologyTypeId}.", item.Id);
      Snackbar.Add("The mythology type could not be retired. Check the service connection and try again.", Severity.Error);
    }
    catch (Exception exception)
    {
      Logger.LogError(exception, "Unexpected failure while retiring mythology type {MythologyTypeId}.", item.Id);
      Snackbar.Add("An unexpected error occurred while retiring the mythology type.", Severity.Error);
    }
  }
}
