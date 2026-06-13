using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Application.Commands.Skills.Services;

public interface ISkillUniquenessValidator
{
  Task EnsureUniqueSkillNameAsync(string name, CancellationToken ct);
}