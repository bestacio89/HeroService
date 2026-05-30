using Franz.Common.DependencyInjection;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Contracts.Persistence.Modifiers;

public interface IHeroModifierRepository : IScopedDependency
{
  Task<HeroModifier?> GetByHeroAndVersionAsync(
      Guid heroId,
      Guid gameVersionId,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyList<HeroModifier>> GetByGameVersionIdAsync(
      Guid gameVersionId,
      CancellationToken cancellationToken = default);
}