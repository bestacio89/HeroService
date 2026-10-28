using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.HeroClasses;

public sealed partial class Index : ComponentBase
{
  [Inject] private IHeroClassClient Client { get; set; } = default!;
  [Inject] private IDialogService DialogService { get; set; } = default!;
  [Inject] private ILogger<Index> Logger { get; set; } = default!;

  private IReadOnlyCollection<HeroClassDto> Items = [];
  private IReadOnlyList<HeroClassDto> FilteredItems = [];
  private string? _search;
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
      ApplyFilter();
    }
    catch (OperationCanceledException)
    {
      ErrorMessage = "The request was cancelled or timed out. Retry when the service is available.";
    }
    catch (HttpRequestException exception)
    {
      Logger.LogWarning(exception, "The HeroService API could not be reached while loading hero classes.");
      ErrorMessage = "The HeroService API could not be reached. Check the service connection and try again.";
    }
    catch (Exception exception)
    {
      Logger.LogError(exception, "Unexpected failure while loading hero classes.");
      ErrorMessage = "An unexpected error occurred while loading hero classes.";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private void OnSearchChanged(string? value)
  {
    _search = value;
    ApplyFilter();
  }

  private void ApplyFilter()
  {
    FilteredItems = string.IsNullOrWhiteSpace(_search)
      ? Items.ToArray()
      : Items.Where(item => item.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false).ToArray();
  }

  private void ClearSearch()
  {
    _search = null;
    ApplyFilter();
  }

  private async Task OpenCreateDialog()
  {
    var parameters = new DialogParameters
    {
      { "OnSaved", EventCallback.Factory.Create(this, Load) }
    };
    var options = new DialogOptions { CloseButton = true, FullWidth = true, MaxWidth = MaxWidth.Small };
    await DialogService.ShowAsync<HeroClassDialog>("Create Hero Class", parameters, options);
  }
}
