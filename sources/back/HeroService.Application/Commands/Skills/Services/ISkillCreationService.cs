using HeroService.Contracts.Commands.Skills;

namespace HeroService.Application.Commands.Skills.Services;

public interface ISkillCreationService 
{
  Task<Guid> CreateAsync(
      CreateSkillCommand request,
      CancellationToken cancellationToken);
}