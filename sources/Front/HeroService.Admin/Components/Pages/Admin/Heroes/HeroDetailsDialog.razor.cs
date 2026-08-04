using HeroService.Client.Http.Abstractions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Heroes;

public sealed partial class HeroDetailsDialog : ComponentBase
{
  [Inject]
  private ISkillClient SkillClient { get; set; } = default!;

  [CascadingParameter]
  private IMudDialogInstance MudDialog { get; set; } = default!;

  [Parameter]
  [EditorRequired]
  public HeroDto Hero { get; set; } = default!;

  private bool IsLoadingSkills = true;

  private string PassiveSkillName = "(unknown)";
  private string PrimarySkillName = "(unknown)";
  private string SecondarySkillName = "(unknown)";
  private string TertiarySkillName = "(unknown)";
  private string UltimateSkillName = "(unknown)";

  protected override async Task OnInitializedAsync()
  {
    var passiveTask = SkillClient.GetDetailsAsync(Hero.SkillKit.PassiveSkillId);
    var primaryTask = SkillClient.GetDetailsAsync(Hero.SkillKit.PrimarySkillId);
    var secondaryTask = SkillClient.GetDetailsAsync(Hero.SkillKit.SecondarySkillId);
    var tertiaryTask = SkillClient.GetDetailsAsync(Hero.SkillKit.TertiarySkillId);
    var ultimateTask = SkillClient.GetDetailsAsync(Hero.SkillKit.UltimateSkillId);

    await Task.WhenAll(
        passiveTask,
        primaryTask,
        secondaryTask,
        tertiaryTask,
        ultimateTask);

    PassiveSkillName = (await passiveTask)?.Name ?? "(unknown)";
    PrimarySkillName = (await primaryTask)?.Name ?? "(unknown)";
    SecondarySkillName = (await secondaryTask)?.Name ?? "(unknown)";
    TertiarySkillName = (await tertiaryTask)?.Name ?? "(unknown)";
    UltimateSkillName = (await ultimateTask)?.Name ?? "(unknown)";

    IsLoadingSkills = false;
  }

  private void Close()
  {
    MudDialog.Close();
  }
}