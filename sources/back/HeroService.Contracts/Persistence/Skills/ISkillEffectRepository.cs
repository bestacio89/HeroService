using Franz.Common.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Persistence.Skills;

public interface ISkillEffectRepository : IScopedDependency
{
  Task<IReadOnlyDictionary<Guid, List<SkillEffect>>> GetBySkillIdsAsync(
      IReadOnlyList<Guid> skillIds,
      CancellationToken ct);
}
