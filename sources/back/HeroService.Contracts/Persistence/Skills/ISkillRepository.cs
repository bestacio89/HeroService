using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.Persistence.Skills;

public interface ISkillRepository :
    INameLookupRepository<Skill, Guid>
{
  Task<IReadOnlyList<Skill>> GetByIdsAsync(
      IEnumerable<Guid> ids,
      CancellationToken cancellationToken);
}