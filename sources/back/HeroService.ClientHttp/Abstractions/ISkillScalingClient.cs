using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Commands.Skills;

namespace HeroService.Client.Http.Abstractions;

public interface ISkillScalingClient
{
  Task<SkillScalingModifierDto?> GetBySkillAsync(
      Guid skillId,
      CancellationToken cancellationToken = default);

  Task<Guid> DefineAsync(
      DefineSkillScalingCommand command,
      CancellationToken cancellationToken = default);
}