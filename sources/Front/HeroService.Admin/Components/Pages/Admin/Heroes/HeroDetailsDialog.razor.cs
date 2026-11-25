using HeroService.Client.Http.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class HeroDetailsDialog : ComponentBase, IDisposable
{
  [Inject]
  private ISkillClient SkillClient { get; set; } = default!;

  [Inject]
  private ILogger<HeroDetailsDialog> Logger { get; set; } = default!;

  [CascadingParameter]
  private IMudDialogInstance MudDialog { get; set; } = default!;

  [Parameter]
  [EditorRequired]
  public HeroDto Hero { get; set; } = default!;

  private readonly CancellationTokenSource _lifetimeCts = new();
  private bool IsLoadingSkills = true;
  private bool IsSkillLookupFailed;
  private string? SkillLookupError;

  private string PassiveSkillName = "(unassigned)";
  private string PrimarySkillName = "(unassigned)";
  private string SecondarySkillName = "(unassigned)";
  private string TertiarySkillName = "(unassigned)";
  private string UltimateSkillName = "(unassigned)";

  protected override Task OnInitializedAsync() => LoadSkillsAsync();

  private async Task LoadSkillsAsync()
  {
    if (_disposed)
      return;

    IsLoadingSkills = true;
    IsSkillLookupFailed = false;
    SkillLookupError = null;

    var skillKit = Hero.SkillKit;
    if (skillKit is null)
    {
      IsLoadingSkills = false;
      return;
    }

    var skillIds = new[]
    {
      skillKit.PassiveSkillId,
      skillKit.PrimarySkillId,
      skillKit.SecondarySkillId,
      skillKit.TertiarySkillId,
      skillKit.UltimateSkillId
    };

    try
    {
      var skills = await SkillClient.GetLookupsAsync(skillIds, _lifetimeCts.Token);
      var namesById = skills.ToDictionary(skill => skill.Id, skill => skill.Name);

      PassiveSkillName = ResolveName(skillKit.PassiveSkillId, namesById);
      PrimarySkillName = ResolveName(skillKit.PrimarySkillId, namesById);
      SecondarySkillName = ResolveName(skillKit.SecondarySkillId, namesById);
      TertiarySkillName = ResolveName(skillKit.TertiarySkillId, namesById);
      UltimateSkillName = ResolveName(skillKit.UltimateSkillId, namesById);
    }
    catch (OperationCanceledException) when (_lifetimeCts.IsCancellationRequested)
    {
      // Dialog disposal cancelled the lookup.
    }
    catch (Exception exception)
    {
      Logger.LogWarning(exception, "Unable to resolve skill names for hero {HeroId}.", Hero.Id);
      IsSkillLookupFailed = true;
      SkillLookupError = "Skill information is temporarily unavailable.";
    }
    finally
    {
      if (!_disposed)
        IsLoadingSkills = false;
    }
  }

  private static string ResolveName(
      Guid skillId,
      IReadOnlyDictionary<Guid, string> namesById) =>
      skillId != Guid.Empty && namesById.TryGetValue(skillId, out var name)
          ? name
          : "Reference unavailable";

  private void Close()
  {
    MudDialog.Close();
  }

  private readonly bool _disposed;

  public void Dispose()
  {
    if (_disposed)
      return;

    _disposed = true;
    _lifetimeCts.Cancel();
    _lifetimeCts.Dispose();
  }
}
