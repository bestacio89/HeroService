using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Contracts.Persistence.Modifiers;

public interface IHeroModifierRepository
{
  Task<HeroModifier?> GetAsync(
      Guid heroId,
      Guid gameVersionId,
      CancellationToken ct);

  Task<IReadOnlyDictionary<Guid, HeroModifier>> GetByHeroIdsAsync(
      IReadOnlyList<Guid> heroIds,
      Guid gameVersionId,
      CancellationToken ct);
}