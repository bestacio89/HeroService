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
  [Inject] private NavigationManager Nav { get; set; } = default!;
  [Inject] private IDialogService DialogService { get; set; } = default!;
  [Inject] private ISnackbar Snackbar { get; set; } = default!;
  [Inject] private ILogger<Index> Logger { get; set; } = default!;

  private IReadOnlyList<SkillDto> Items { get; set; } = [];
  private IReadOnlyList<SkillDto> FilteredItems { get; set; } = [];
  private IReadOnlyList<SkillType> SkillTypes { get; } = Enum.GetValues<SkillType>();
  private readonly HashSet<Guid> _openingSkillIds = [];
  private string? _search;
  private SkillType? _selectedType;
  private bool Loading { get; set; } = true;
  private string? ErrorMessage { get; set; }
  private int _skillTypeCount;

  protected override Task OnInitializedAsync() => LoadAsync();

  private async Task LoadAsync()
  {
    Loading = true;
    ErrorMessage = null;
    try
    {
      var result = await Client.GetAllAsync();
      Items = result?.OrderBy(skill => skill.Name, StringComparer.OrdinalIgnoreCase).ToArray() ?? [];
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

    if (!string.IsNullOrWhiteSpace(_search))
    {
      var term = _search.Trim();
      query = query.Where(skill =>
        skill.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
        skill.SkillType.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ||
        SkillTheme.GetDescription(skill.SkillType).Contains(term, StringComparison.OrdinalIgnoreCase) ||
        (skill.Effects?.Any(effect =>
          effect.EffectType.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ||
          effect.BuffType?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) == true ||
          effect.DebuffType?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) == true) ?? false));
    }

    FilteredItems = query.ToArray();
  }

  private void SelectType(SkillType? type)
  {
    _selectedType = type;
    ApplyFilter();
  }

  private void ClearFilters()
  {
    _search = null;
    _selectedType = null;
    ApplyFilter();
  }

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
