
using HeroService.Contracts.Commands.Modifiers;
using HeroService.Contracts.DTOs.Modifiers;

namespace HeroService.Client.Http.Abstractions;

public interface ISkillModifierClient
{
  Task<Guid> CreateAsync(
      CreateSkillModifierCommand command,
      CancellationToken cancellationToken = default);


  Task UpdateAsync(
      Guid id,
      UpdateSkillModifierCommand command,
      CancellationToken cancellationToken = default);


  Task<SkillModifierDto?> GetBySkillAsync(
      Guid skillId,
      CancellationToken cancellationToken = default);


  Task<IReadOnlyList<SkillModifierDto>> GetActiveAsync(
      CancellationToken cancellationToken = default);
}