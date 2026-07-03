using HeroService.Admin.Components.Models;

namespace HeroService.Admin.Mappers;

public static class SkillMappingExtensions
{
  public static CreateSkillCommand ToCommand(this CreateSkillModel model)
      => new(
          model.Name,
          model.SkillType,
          model.Lore,
          model.BaseStats,
          model.Effects
      );
}
