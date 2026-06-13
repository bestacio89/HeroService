using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Commands.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Client.Http.Abstractions;

public interface ISkillClient
{
  Task<Guid> CreateSkillAsync(
      CreateSkillCommand command,
      CancellationToken cancellationToken = default);

  Task<Guid?> AddEffectAsync(
      Guid skillId,
      CreateSkillEffectCommand command,
      CancellationToken cancellationToken = default);

  Task UpdateLoreAsync(
      Guid skillLoreId,
      UpdateSkillLoreCommand command,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<SkillDto>> GetAllAsync(
      CancellationToken cancellationToken = default);

  Task<SkillDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default);

  Task<SkillDto?> GetDetailsAsync(
      Guid skillId,
      CancellationToken cancellationToken = default);

  Task<SkillLoreDto?> GetLoreAsync(
      Guid skillId,
      CancellationToken cancellationToken = default);

  Task<SkillBaseStatsDto?> GetBaseStatsAsync(
      Guid skillId,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<SkillDto>> GetByTypeAsync(
      SkillType skillType,
      CancellationToken cancellationToken = default);
}