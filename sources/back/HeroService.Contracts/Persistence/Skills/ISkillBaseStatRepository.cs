using Franz.Common.DependencyInjection;
using HeroService.Domain.Heroes.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Persistence.Skills;

public interface ISkillBaseStatsRepository : IScopedDependency
{
  Task<IReadOnlyDictionary<Guid, SkillBaseStats>> GetBySkillIdsAsync(
      IReadOnlyList<Guid> skillIds,
      CancellationToken ct);
}