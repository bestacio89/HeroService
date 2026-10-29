using HeroService.Contracts.DTOs.Heroes;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Skills;

public sealed partial class Index : ComponentBase
{
  [Inject] private ISkillClient Client { get; set; } = default!;
  [Inject] private IHeroClient HeroClient { get; set; } = default!;
  [Inject] private NavigationManager Nav { get; set; } = default!;
  [Inject] private IDialogService DialogService { get; set; } = default!;
  [Inject] private ISnackbar Snackbar { get; set; } = default!;
  [Inject] private ILogger<Index> Logger { get; set; } = default!;

  private IReadOnlyList<SkillDto> Items { get; set; } = [];
  private IReadOnlyList<SkillDto> FilteredItems { get; set; } = [];
  private IReadOnlyList<SkillType> SkillTypes { get; } = Enum.GetValues<SkillType>();
  private IReadOnlyList<HeroDto> Heroes { get; set; } = [];
  private IReadOnlyList<HeroDto> HeroesWithSkills { get; set; } = [];
  private readonly Dictionary<Guid, IReadOnlyList<HeroDto>> _heroesBySkillId = [];
  private readonly HashSet<Guid> _openingSkillIds = [];
  private string? _search;
  private SkillType? _selectedType;
  private Guid? _selectedHeroId;
  private bool Loading { get; set; } = true;
  private string? ErrorMessage { get; set; }
  private int _skillTypeCount;
  private const int PageSize = 24;
  private int _currentPage = 1;
  private int PageCount => Math.Max(1, (FilteredItems.Count + PageSize - 1) / PageSize);
  private IReadOnlyList<SkillDto> VisibleItems => FilteredItems.Skip((_currentPage - 1) * PageSize).Take(PageSize).ToArray();

  protected override Task OnInitializedAsync() => LoadAsync();

  private async Task LoadAsync()
  {
    Loading = true;
    ErrorMessage = null;
    try
    {
      var result = await Client.GetAllAsync();
      Items = result?.OrderBy(skill => skill.Name, StringComparer.OrdinalIgnoreCase).ToArray() ?? [];
      try
      {
        Heroes = (await HeroClient.BrowseAsync()).OrderBy(hero => hero.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        BuildHeroSkillIndex();
      }
      catch (Exception exception) when (exception is not OperationCanceledException)
      {
        Logger.LogWarning(exception, "Unable to load hero associations for the skill library.");
        Heroes = [];
        HeroesWithSkills = [];
        _heroesBySkillId.Clear();
      }
      _skillTypeCount = Items.Select(skill => skill.SkillType).Distinct().Count();
      ApplyFilter();
    }
    catch (OperationCanceledException)
    {
      ErrorMessage = "The request was cancelled or timed out. Retry when the service is available.";
    }
    catch (HttpRequestException exception)
    {
      Logger.LogWarning(exception, "The HeroService API could not be reached while loading the skill library.");
      ErrorMessage = "The HeroService API could not be reached. Check the service connection and try again.";
    }
    catch (Exception exception)
    {
      Logger.LogError(exception, "Unexpected failure while loading the skill library.");
      ErrorMessage = "An unexpected error occurred while loading skills.";
    }
    finally
    {
      Loading = false;
    }
  }

  private void OnSearchChanged(string? value)
  {
    _search = value;
    ApplyFilter();
  }

  private void ApplyFilter()
  {
    IEnumerable<SkillDto> query = Items;

    if (_selectedType.HasValue)
      query = query.Where(skill => skill.SkillType == _selectedType.Value);

    if (_selectedHeroId.HasValue)
      query = query.Where(skill => _heroesBySkillId.TryGetValue(skill.Id, out var owners) && owners.Any(hero => hero.Id == _selectedHeroId.Value));

    if (!string.IsNullOrWhiteSpace(_search))
    {
      var term = _search.Trim();
      query = query.Where(skill =>
        skill.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
        skill.SkillType.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ||
        SkillTheme.GetDescription(skill.SkillType).Contains(term, StringComparison.OrdinalIgnoreCase) ||
        (_heroesBySkillId.TryGetValue(skill.Id, out var owners) && owners.Any(hero =>
          hero.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
          (hero.Class?.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))) ||
        (skill.Effects?.Any(effect =>
          effect.EffectType.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ||
          effect.BuffType?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) == true ||
          effect.DebuffType?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) == true) ?? false));
    }

    FilteredItems = query.ToArray();
    _currentPage = 1;
  }

  private void PreviousPage()
  {
    if (_currentPage > 1)
      _currentPage--;
  }

  private void NextPage()
  {
    if (_currentPage < PageCount)
      _currentPage++;
  }

  private void SelectType(SkillType? type)
  {
    _selectedType = type;
    ApplyFilter();
  }

  private void OnHeroFilterChanged(Guid? heroId)
  {
    _selectedHeroId = heroId;
    ApplyFilter();
  }

  private void ClearFilters()
  {
    _search = null;
    _selectedType = null;
    _selectedHeroId = null;
    ApplyFilter();
  }

  private void BuildHeroSkillIndex()
  {
    var index = new Dictionary<Guid, List<HeroDto>>();
    foreach (var hero in Heroes)
    {
      var kit = hero.SkillKit;
      if (kit is null)
        continue;

      foreach (var skillId in new[] { kit.PassiveSkillId, kit.PrimarySkillId, kit.SecondarySkillId, kit.TertiarySkillId, kit.UltimateSkillId }.Where(id => id != Guid.Empty).Distinct())
      {
        if (!index.TryGetValue(skillId, out var owners))
          index[skillId] = owners = [];
        owners.Add(hero);
      }
    }

    _heroesBySkillId.Clear();
    foreach (var (skillId, owners) in index)
      _heroesBySkillId[skillId] = owners.OrderBy(hero => hero.Name, StringComparer.OrdinalIgnoreCase).ToArray();

    HeroesWithSkills = Heroes.Where(hero => hero.SkillKit is not null &&
      new[] { hero.SkillKit.PassiveSkillId, hero.SkillKit.PrimarySkillId, hero.SkillKit.SecondarySkillId, hero.SkillKit.TertiarySkillId, hero.SkillKit.UltimateSkillId }
        .Any(id => id != Guid.Empty && Items.Any(skill => skill.Id == id)))
      .ToArray();
  }

  private IReadOnlyList<HeroDto> GetHeroesForSkill(Guid skillId) =>
    _heroesBySkillId.TryGetValue(skillId, out var heroes) ? heroes : [];

  private static string GetHeroLabel(HeroDto hero) =>
    string.IsNullOrWhiteSpace(hero.Class?.Name) ? hero.Name : $"{hero.Name} · {hero.Class.Name}";

  private static string FormatStat(float? value, string suffix = "") =>
      value.HasValue ? $"{value.Value:0.##}{suffix}" : "—";

  private void NavigateToCreate() => Nav.NavigateTo("/admin/skills/create");

  private async Task OpenDetailsAsync(SkillDto skill)
  {
    if (!_openingSkillIds.Add(skill.Id))
      return;

    try
    {
      var details = await Client.GetDetailsAsync(skill.Id);
      if (details is null)
      {
        Snackbar.Add("Skill details were not found. The library may have changed.", Severity.Warning);
        return;
      }

      var parameters = new DialogParameters<SkillDetailsDialog> { { x => x.Skill, details } };
      await DialogService.ShowAsync<SkillDetailsDialog>(details.Name, parameters);
    }
    catch (OperationCanceledException)
    {
      Snackbar.Add("The request for skill details was cancelled or timed out. Try again.", Severity.Warning);
    }
    catch (HttpRequestException exception)
    {
      Logger.LogWarning(exception, "Unable to load details for skill {SkillId}.", skill.Id);
      Snackbar.Add("Skill details could not be loaded. Check the service connection and try again.", Severity.Error);
    }
    catch (Exception exception)
    {
      Logger.LogError(exception, "Unexpected failure while loading details for skill {SkillId}.", skill.Id);
      Snackbar.Add("An unexpected error occurred while loading skill details.", Severity.Error);
    }
    finally
    {
      _openingSkillIds.Remove(skill.Id);
    }
  }
}
