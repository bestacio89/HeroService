using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Application.Commands.Heroes.Services;

public interface IHeroUniquenessValidator
{
  Task EnsureUniqueHeroNameAsync(string name, CancellationToken ct);
}
