using HeroService.Client.Http.Abstractions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class Index : ComponentBase
{
  [Inject]
  private IHeroClient HeroClient { get; set; } = default!;

  [Inject]
  private NavigationManager Nav { get; set; } = default!;

  [Inject]
  private IDialogService DialogService { get; set; } = default!;

  [Inject]
  private ISnackbar Snackbar { get; set; } = default!;

  private List<HeroDto> _heroes = [];

  private string? _search;

  protected override async Task OnInitializedAsync()
  {
    var result = await HeroClient.BrowseAsync();
    _heroes = result?.ToList() ?? [];
  }

  private IEnumerable<HeroDto> FilteredHeroes =>
      string.IsNullOrWhiteSpace(_search)
          ? _heroes
          : _heroes.Where(h =>
              h.Name.Contains(_search, StringComparison.OrdinalIgnoreCase) ||
              (h.Class?.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false) ||
              (h.Affiliation?.Mythology?.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false));

  private async Task OpenDetailsAsync(HeroDto hero)
  {
    var details = await HeroClient.GetByIdAsync(hero.Id);

    if (details is null)
    {
      Snackbar.Add("Hero details not found", Severity.Error);
      return;
    }

    var parameters = new DialogParameters<HeroDetailsDialog>
        {
            { x => x.Hero, details }
        };

    await DialogService.ShowAsync<HeroDetailsDialog>(
        details.Name,
        parameters);
  }
}