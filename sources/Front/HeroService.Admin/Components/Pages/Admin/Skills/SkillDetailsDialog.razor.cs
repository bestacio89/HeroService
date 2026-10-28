using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Skills;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.Skills;

public sealed partial class SkillDetailsDialog : ComponentBase
{
  [CascadingParameter]
  private IMudDialogInstance MudDialog { get; set; } = default!;

  [Parameter]
  [EditorRequired]
  public SkillDto Skill { get; set; } = default!;

  [Inject]
  private ISkillClient Client { get; set; } = default!;

  [Inject]
  private ILogger<SkillDetailsDialog> Logger { get; set; } = default!;

  private SkillLoreDto? Lore { get; set; }
  private SkillBaseStatsDto? BaseStats { get; set; }
  private bool IsLoadingLore { get; set; } = true;
  private bool DetailsWarning { get; set; }

  private SkillBaseStatsDto? CurrentStats => BaseStats ?? Skill.BaseStats;

  protected override async Task OnInitializedAsync()
  {
    BaseStats = Skill.BaseStats;
    await Task.WhenAll(LoadBaseStatsAsync(), LoadLoreAsync());
  }

  private async Task LoadBaseStatsAsync()
  {
    try
    {
      BaseStats = await Client.GetBaseStatsAsync(Skill.Id) ?? Skill.BaseStats;
    }
    catch (Exception exception)
    {
      Logger.LogWarning(exception, "Unable to load base stats for skill {SkillId}; using the details payload when available.", Skill.Id);
      DetailsWarning = true;
    }
  }

  private async Task LoadLoreAsync()
  {
    IsLoadingLore = true;
    try
    {
      Lore = await Client.GetLoreAsync(Skill.Id);
    }
    catch (Exception exception)
    {
      Logger.LogWarning(exception, "Unable to load lore for skill {SkillId}.", Skill.Id);
      DetailsWarning = true;
    }
    finally
    {
      IsLoadingLore = false;
    }
  }

  private static string FormatStat(float? value, string suffix = "") =>
      value.HasValue ? $"{value.Value:0.##}{suffix}" : "—";

  private void Close()
  {
    MudDialog.Close();
  }
}
