using HeroService.Domain.Heroes.Core;

namespace HeroService.Contracts.Persistence.Heroes;

public interface IHeroRepository : INameLookupRepository<Hero, Guid>
{
  // =========================================================
  // SINGLE HERO
  // =========================================================

  Task<Hero?> GetDetailsAsync(
      Guid heroId,
      CancellationToken cancellationToken = default);

  Task<Hero?> GetByNameWithDetailsAsync(
      string name,
      CancellationToken cancellationToken = default);

  // =========================================================
  // FILTERED COLLECTIONS
  // =========================================================
  Task<IReadOnlyCollection<Hero>> GetByClassAsync(
      Guid heroClassId,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Hero>> GetByMythologyAsync(
      Guid mythologyTypeId,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Hero>> GetByCultureAsync(
      Guid cultureId,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Hero>> GetByArchetypeAsync(
      Guid archetypeId,
      CancellationToken cancellationToken = default);

  // =========================================================
  // FULL BROWSING
  // =========================================================

  Task<IReadOnlyCollection<Hero>> GetAllWithDetailsAsync(
      CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Hero>> BrowseAsync(
    Guid? heroClassId,
    Guid? mythologyTypeId,
    Guid? cultureId,
    Guid? archetypeId,
    CancellationToken ct = default);
}