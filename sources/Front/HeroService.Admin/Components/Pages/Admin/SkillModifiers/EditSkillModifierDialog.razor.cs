using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Modifiers;
using HeroService.Contracts.DTOs.Modifiers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.SkillModifiers;

public partial class EditSkillModifierDialog
{
  [CascadingParameter]
  private IMudDialogInstance Dialog { get; set; } = default!;


  [Parameter]
  public SkillModifierDto Modifier { get; set; } = default!;


  [Inject]
  private ISkillModifierClient Client { get; set; } = default!;


  private UpdateSkillModifierCommand Model { get; set; } = new();


  private bool Saving { get; set; }


  protected override void OnInitialized()
  {
    Model = new UpdateSkillModifierCommand
    {
      SkillModifierId = Modifier.SkillId,

      CooldownMultiplier = Modifier.CooldownMultiplier,

      ManaCostMultiplier = Modifier.ManaCostMultiplier,

      DamageMultiplier = Modifier.DamageMultiplier,

      HealingMultiplier = Modifier.HealingMultiplier,

      ShieldMultiplier = Modifier.ShieldMultiplier,

      CastTimeMultiplier = Modifier.CastTimeMultiplier,

      ChannelDurationMultiplier = Modifier.ChannelDurationMultiplier,

      CrowdControlDurationMultiplier =
            Modifier.CrowdControlDurationMultiplier,

      RangeMultiplier = Modifier.RangeMultiplier
    };
  }


  private async Task Save()
  {
    Saving = true;

    try
    {
      await Client.UpdateAsync(
          Modifier.SkillId,
          Model);

      Dialog.Close(DialogResult.Ok(true));
    }
    finally
    {
      Saving = false;
    }
  }


  private void Cancel()
  {
    Dialog.Cancel();
  }
}