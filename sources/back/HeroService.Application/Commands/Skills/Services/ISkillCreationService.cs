using HeroService.Contracts.DTOs.Skills;

namespace HeroService.Application.Commands.Skills.Services;

public interface ISkillCreationService
{
  Task<Guid> CreateAsync(
      SkillDto request,
      CancellationToken cancellationToken);
}