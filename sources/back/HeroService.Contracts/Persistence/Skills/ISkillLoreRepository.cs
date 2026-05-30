using Franz.Common.DependencyInjection;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.Persistence.Skills;

public interface ISkillLoreRepository
    : IScopedDependency
{
  Task<SkillLore?> GetBySkillIdAsync(
      Guid skillId,
      CancellationToken cancellationToken = default);

  Task<SkillLore?> GetLoreBySkillNameAsync(string skillName, CancellationToken cancellationToken = default);
}