using HeroService.Client.Http.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class Index : ComponentBase
{
  [Inject]
  private IHeroClient HeroClient { get; set; } = default!;

  [Inject]
  private IDialogService DialogService { get; set; } = default!;

  [Inject]
  private ISnackbar Snackbar { get; set; } = default!;

  [Inject]
  private ILogger<Index> Logger { get; set; } = default!;

  private List<HeroDto> _heroes = [];
  private IReadOnlyList<HeroDto> _filteredHeroes = [];
  private readonly HashSet<Guid> _openingHeroIds = [];
  private string? _search;
  private bool _isLoading = true;
  private string? _errorMessage;
  private int _classCount;
  private int _mythologyCount;

  private IReadOnlyList<HeroDto> FilteredHeroes => _filteredHeroes;

  protected override Task OnInitializedAsync() => LoadHeroesAsync();

  private async Task LoadHeroesAsync()
  {
    _isLoading = true;
    _errorMessage = null;

    try
    {
      var result = await HeroClient.BrowseAsync();
      _heroes = result?.ToList() ?? [];
      _classCount = _heroes
          .Select(hero => hero.Class?.Name)
          .Where(name => !string.IsNullOrWhiteSpace(name))
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .Count();
      _mythologyCount = _heroes
          .Select(hero => hero.Affiliation?.Mythology?.Name)
          .Where(name => !string.IsNullOrWhiteSpace(name))
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .Count();
      ApplyFilter();
    }
    catch (OperationCanceledException)
    {
      _errorMessage = "The request timed out or was cancelled. Retry when the service is available.";
    }
    catch (HttpRequestException exception)
    {
      Logger.LogWarning(exception, "The HeroService API could not be reached while loading the hero catalogue.");
      _errorMessage = "The HeroService API could not be reached. Check the service connection and try again.";
    }
    catch (Exception exception)
    {
      Logger.LogError(exception, "Unexpected failure while loading the hero catalogue.");
      _errorMessage = "An unexpected error occurred while loading the catalogue.";
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
    if (string.IsNullOrWhiteSpace(_search))
    {
      _filteredHeroes = _heroes;
      return;
    }

    _filteredHeroes = _heroes
        .Where(hero =>
            (hero.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (hero.Class?.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (hero.Affiliation?.Mythology?.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (hero.Affiliation?.Archetype?.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false))
        .ToArray();
  }

  private void ClearSearch()
  {
    _search = null;
    ApplyFilter();
  }

  private async Task OpenDetailsAsync(HeroDto hero)
  {
    if (!_openingHeroIds.Add(hero.Id))
      return;

    try
    {
      var details = await HeroClient.GetByIdAsync(hero.Id);

      if (details is null)
      {
        Snackbar.Add("Hero details were not found. The catalogue may have changed.", Severity.Warning);
        return;
      }

      var parameters = new DialogParameters<HeroDetailsDialog>
      {
        { dialog => dialog.Hero, details }
      };

      await DialogService.ShowAsync<HeroDetailsDialog>(details.Name, parameters);
    }
    catch (OperationCanceledException)
    {
      Snackbar.Add("The request for hero details timed out. Try again.", Severity.Warning);
    }
    catch (HttpRequestException exception)
    {
      Logger.LogWarning(exception, "Unable to load details for hero {HeroId}.", hero.Id);
      Snackbar.Add("Hero details could not be loaded. Check the service connection and try again.", Severity.Error);
    }
    catch (Exception exception)
    {
      Logger.LogError(exception, "Unexpected failure while loading details for hero {HeroId}.", hero.Id);
      Snackbar.Add("An unexpected error occurred while loading hero details.", Severity.Error);
    }
    finally
    {
      _openingHeroIds.Remove(hero.Id);
    }
  }
}
