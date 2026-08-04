using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;
using Microsoft.AspNetCore.Components;

namespace HeroService.Admin.Components.Pages.Admin.Skills;

public sealed partial class SkillEffectsEditor : ComponentBase
{
  [Parameter]
  [EditorRequired]
  public List<SkillEffectDto> Effects { get; set; } = new();


  [Parameter]
  public EventCallback<List<SkillEffectDto>> EffectsChanged { get; set; }


  private List<SkillEffectDto> _effects = new();


  protected override void OnParametersSet()
  {
    _effects = Effects ?? [];
  }


  private async Task AddEffect()
  {
    _effects.Add(new SkillEffectDto
    {
      EffectType = EffectType.Damage,
      TargetType = TargetType.Enemy,
      StackType = StackType.None,
      MaxStacks = 1,
      BuffType = null,
      DebuffType = null
    });


    await Notify();
  }


  private async Task RemoveEffect(int index)
  {
    if (index < 0 || index >= _effects.Count)
    {
      return;
    }


    _effects.RemoveAt(index);

    await Notify();
  }


  private async Task ChangeEffectType(
      int index,
      EffectType value)
  {
    if (index < 0 || index >= _effects.Count)
    {
      return;
    }


    var effect = _effects[index];

    effect.EffectType = value;


    switch (value)
    {
      case EffectType.Buff:

        effect.DebuffType = null;

        effect.BuffType ??=
            Enum.GetValues<BuffType>().First();

        break;


      case EffectType.Debuff:

        effect.BuffType = null;

        effect.DebuffType ??=
            Enum.GetValues<DebuffType>().First();

        break;


      default:

        effect.BuffType = null;
        effect.DebuffType = null;

        break;
    }


    await Notify();
  }


  private async Task Notify()
  {
    if (EffectsChanged.HasDelegate)
    {
      await EffectsChanged.InvokeAsync(_effects);
    }
  }
}