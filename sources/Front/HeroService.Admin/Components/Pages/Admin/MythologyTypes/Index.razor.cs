using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.MythologyTypes;

public sealed partial class Index : ComponentBase
{
  [Inject] private IMythologyTypeClient MythologyTypeClient { get; set; } = default!;
  [Inject] private IDialogService DialogService { get; set; } = default!;
  [Inject] private ILogger<Index> Logger { get; set; } = default!;

  private List<MythologyTypeDto> _items = [];
  private IReadOnlyList<MythologyTypeDto> _filteredItems = [];
  private string? _search;
  private bool _isLoading = true;
  private string? _errorMessage;

  protected override Task OnInitializedAsync() => LoadAsync();

  private async Task LoadAsync()
  {
    _isLoading = true;
    _errorMessage = null;
    try
    {
      var result = await MythologyTypeClient.GetAllAsync();
      _items = result?.ToList() ?? [];
      ApplyFilter();
    }
    catch (OperationCanceledException)
    {
      _errorMessage = "The request was cancelled or timed out. Retry when the service is available.";
    }
    catch (HttpRequestException exception)
    {
      Logger.LogWarning(exception, "The HeroService API could not be reached while loading mythology types.");
      _errorMessage = "The HeroService API could not be reached. Check the service connection and try again.";
    }
    catch (Exception exception)
    {
      Logger.LogError(exception, "Unexpected failure while loading mythology types.");
      _errorMessage = "An unexpected error occurred while loading mythology types.";
    }
    finally
    {
      _isLoading = false;
    }
  }

  private void OnSearchChanged(string? value)
  {
    _search = value;
    ApplyFilter();
  }

  private void ApplyFilter()
  {
    _filteredItems = string.IsNullOrWhiteSpace(_search)
      ? _items
      : _items.Where(item => item.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false).ToArray();
  }

  private void ClearSearch()
  {
    _search = null;
    ApplyFilter();
  }

  private async Task CreateAsync()
  {
    var dialog = await DialogService.ShowAsync<MythologyTypeDialog>("Create Mythology Type");
    var result = await dialog.Result;
    if (result is { Canceled: false })
      await LoadAsync();
  }

  private async Task EditAsync(MythologyTypeDto item)
  {
    var parameters = new DialogParameters { ["Item"] = item };
    var dialog = await DialogService.ShowAsync<MythologyTypeDialog>("Rename Mythology Type", parameters);
    var result = await dialog.Result;
    if (result is { Canceled: false })
      await LoadAsync();
  }
}
