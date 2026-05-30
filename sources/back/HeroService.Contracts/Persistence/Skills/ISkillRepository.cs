using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.Persistence.Skills;

public interface ISkillRepository :
    INameLookupRepository<Skill, Guid>
{
  Task<IReadOnlyList<Skill>> GetByIdsAsync(
      IEnumerable<Guid> ids,
      CancellationToken cancellationToken);
  Task<Skill?> GetDetailsAsync(
       Guid skillId,
       CancellationToken cancellationToken = default);

  Task<Skill?> GetByNameWithDetailsAsync(
      string name,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Skill>> GetAllWithDetailsAsync(
      CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Skill>> GetByTypeAsync(
      SkillType skillType,
      CancellationToken cancellationToken = default);
}