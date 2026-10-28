using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Modifiers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.SkillModifiers;

public partial class SkillModifierList
{
  [Inject]
  public ISkillModifierClient Client { get; set; } = default!;


  [Inject]
  public ISkillClient SkillClient { get; set; } = default!;


  [Inject]
  public IGameVersionClient GameVersionClient { get; set; } = default!;


  [Inject]
  public IDialogService DialogService { get; set; } = default!;

  [Inject]
  public ILogger<SkillModifierList> Logger { get; set; } = default!;


  private IReadOnlyList<SkillModifierDto> Modifiers { get; set; } = [];


  private Dictionary<Guid, string> SkillNames { get; set; } = [];


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
      var skillsTask = SkillClient.GetAllAsync();
      var versionTask = GameVersionClient.GetActiveAsync();

      await Task.WhenAll(modifiersTask, skillsTask, versionTask);

      Modifiers = await modifiersTask;
      SkillNames = (await skillsTask).ToDictionary(skill => skill.Id, skill => skill.Name);
      var version = await versionTask;
      ActiveGameVersion = version?.VersionName ?? "-";
    }
    catch (OperationCanceledException)
    {
      Modifiers = [];
      SkillNames = [];
      ErrorMessage = "The request was cancelled or timed out. Retry when the service is available.";
    }
    catch (HttpRequestException exception)
    {
      Logger.LogWarning(exception, "The HeroService API could not be reached while loading skill modifiers.");
      Modifiers = [];
      SkillNames = [];
      ErrorMessage = "The HeroService API could not be reached. Check the service connection and try again.";
    }
    catch (Exception exception)
    {
      Logger.LogError(exception, "Unexpected failure while loading skill modifiers.");
      Modifiers = [];
      SkillNames = [];
      ErrorMessage = "An unexpected error occurred while loading skill modifiers.";
    }
    finally
    {
      Loading = false;
    }
  }


  private string GetSkillName(Guid skillId)
  {
    return SkillNames.TryGetValue(
        skillId,
        out var name)
            ? name
            : skillId.ToString();
  }


  private static string DisplayMultiplier(float? value)
  {
    return value.HasValue
        ? $"{value.Value:0.##}x"
        : "-";
  }


  private async Task OpenCreateDialog()
  {
    var dialog = await DialogService.ShowAsync<CreateSkillModifierDialog>(
        "Create Skill Modifier");


    var result = await dialog.Result;


    if (!result.Canceled)
    {
      await LoadAsync();
    }
  }


  private async Task OpenEditDialog(SkillModifierDto modifier)
  {
    var parameters = new DialogParameters
    {
      ["Modifier"] = modifier
    };


    var dialog = await DialogService.ShowAsync<EditSkillModifierDialog>(
        "Edit Skill Modifier",
        parameters);


    var result = await dialog.Result;


    if (!result.Canceled)
    {
      await LoadAsync();
    }
  }
}