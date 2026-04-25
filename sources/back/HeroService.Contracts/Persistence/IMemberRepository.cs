using Franz.Common.DependencyInjection;
using HeroService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HeroService.Contracts.Persistence;
public interface IMemberRepository: IScopedDependency
{
  Task<IReadOnlyList<Member>> GetByPredicateAsync(
      Expression<Func<Member, bool>> predicate,
      CancellationToken cancellationToken = default);
}
