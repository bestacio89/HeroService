using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.Commands.Modifiers;
using HeroService.Contracts.DTOs.Modifiers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HeroService.Admin.Components.Pages.Admin.HeroModifiers;

public partial class EditHeroModifierDialog : ComponentBase
{
  [CascadingParameter]
  private IMudDialogInstance Dialog { get; set; } = default!;


  [Parameter]
  public HeroModifierDto Modifier { get; set; } = default!;


  [Inject]
  private IHeroModifierClient Client { get; set; } = default!;


  public UpdateHeroModifierCommand Model { get; set; } = new();


  private bool Saving { get; set; }


  protected override void OnInitialized()
  {
    Model = new UpdateHeroModifierCommand
    {
      HeroModifierId = Modifier.HeroId,

      HealthMultiplier = Modifier.HealthMultiplier,
      ManaMultiplier = Modifier.ManaMultiplier,

      AttackDamageMultiplier = Modifier.AttackDamageMultiplier,
      MagicDamageMultiplier = Modifier.MagicDamageMultiplier,
      IgnoreEnemyDefenseMultiplier = Modifier.IgnoreEnemyDefenseMultiplier,

      AttackSpeedMultiplier = Modifier.AttackSpeedMultiplier,
      CastSpeedMultiplier = Modifier.CastSpeedMultiplier,

      CritChanceMultiplier = Modifier.CritChanceMultiplier,
      CritDamageMultiplier = Modifier.CritDamageMultiplier,

      ArmorMultiplier = Modifier.ArmorMultiplier,
      MagicResistanceMultiplier = Modifier.MagicResistanceMultiplier,
      DamageReductionMultiplier = Modifier.DamageReductionMultiplier,

      ShieldStrengthMultiplier = Modifier.ShieldStrengthMultiplier,

      MovementSpeedMultiplier = Modifier.MovementSpeedMultiplier,
      AttackRangeMultiplier = Modifier.AttackRangeMultiplier,

      CooldownReductionMultiplier = Modifier.CooldownReductionMultiplier,
      ResourceRegenerationMultiplier = Modifier.ResourceRegenerationMultiplier
    };
  }


  private async Task Save()
  {
    Saving = true;

    try
    {
      await Client.UpdateAsync(
          Modifier.HeroId,
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