using Franz.Common.Errors;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Commands.Skills.Services;

public sealed class SkillUniquenessValidator : ISkillUniquenessValidator
{
  private readonly ISkillRepository _skills;

  public SkillUniquenessValidator(ISkillRepository skills)
  {
    _skills = skills;
  }

  public async Task EnsureUniqueSkillNameAsync(string name, CancellationToken ct)
  {
    if (await _skills.ExistsByNameAsync(name, ct))
      throw new TechnicalException($"Skill '{name}' already exists.");
  }
}