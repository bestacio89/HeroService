using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Modifiers;
using Microsoft.AspNetCore.Components;
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


  private IReadOnlyList<SkillModifierDto> Modifiers { get; set; } = [];


  private Dictionary<Guid, string> SkillNames { get; set; } = [];


  private string ActiveGameVersion { get; set; } = "-";


  private bool Loading { get; set; }


  protected override async Task OnInitializedAsync()
  {
    await LoadAsync();
  }


  private async Task LoadAsync()
  {
    Loading = true;

    try
    {
      Modifiers = await Client.GetActiveAsync();


      var skills = await SkillClient.GetAllAsync();


      SkillNames = skills.ToDictionary(
          skill => skill.Id,
          skill => skill.Name);


      var version = await GameVersionClient.GetActiveAsync();


      ActiveGameVersion = version?.VersionName ?? "-";
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