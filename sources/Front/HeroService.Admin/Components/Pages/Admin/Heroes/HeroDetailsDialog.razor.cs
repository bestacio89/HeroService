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
    var skillKit = Hero.SkillKit;
    if (skillKit is null)
    {
      IsLoadingSkills = false;
      return;
    }

    try
    {
      var skillIds = new[]
      {
        skillKit.PassiveSkillId,
        skillKit.PrimarySkillId,
        skillKit.SecondarySkillId,
        skillKit.TertiarySkillId,
        skillKit.UltimateSkillId
      };

      var skills = await SkillClient.GetLookupsAsync(skillIds);
      var namesById = skills.ToDictionary(skill => skill.Id, skill => skill.Name);

      PassiveSkillName = ResolveName(skillKit.PassiveSkillId, namesById);
      PrimarySkillName = ResolveName(skillKit.PrimarySkillId, namesById);
      SecondarySkillName = ResolveName(skillKit.SecondarySkillId, namesById);
      TertiarySkillName = ResolveName(skillKit.TertiarySkillId, namesById);
      UltimateSkillName = ResolveName(skillKit.UltimateSkillId, namesById);
    }
    finally
    {
      IsLoadingSkills = false;
    }
  }

  private static string ResolveName(
      Guid skillId,
      IReadOnlyDictionary<Guid, string> namesById) =>
      skillId != Guid.Empty && namesById.TryGetValue(skillId, out var name)
          ? name
          : "(unknown)";

  private void Close()
  {
    MudDialog.Close();
  }
}
