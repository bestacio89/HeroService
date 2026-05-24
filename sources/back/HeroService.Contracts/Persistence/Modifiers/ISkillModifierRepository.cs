using Franz.Common.DependencyInjection;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Persistence.Modifiers;

public interface ISkillModifierRepository : IScopedDependency
{
  Task<IReadOnlyDictionary<Guid, SkillModifier?>> GetBySkillIdsAsync(
      IReadOnlyList<Guid> skillIds,
      CancellationToken ct);
}
