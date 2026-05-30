using Franz.Common.DependencyInjection;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Contracts.Persistence.Modifiers;

public interface ISkillModifierRepository : IScopedDependency
{
  Task<SkillModifier?> GetBySkillAndVersionAsync(
      Guid skillId,
      Guid gameVersionId,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyList<SkillModifier>> GetByGameVersionIdAsync(
      Guid gameVersionId,
      CancellationToken cancellationToken = default);


  Task<IReadOnlyList<SkillModifier>> GetBySkillIdsAndVersionAsync(
    IReadOnlyCollection<Guid> skillIds,
    Guid gameVersionId,
    CancellationToken cancellationToken = default);
}