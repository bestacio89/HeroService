using HeroService.Contracts.Commands.Heroes.Classifications.NewFolder;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginArchetype;
using HeroService.Contracts.DTOs.Heroes;

namespace HeroService.Client.Http.Abstractions;

public interface IOriginArchetypeClient
{
  Task<IReadOnlyList<OriginArchetypeDto>> GetAllAsync(
      CancellationToken cancellationToken = default);

  Task<OriginArchetypeDto?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken = default);

  Task<OriginArchetypeDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default);

  Task<Guid> CreateAsync(
      CreateOriginArchetypeCommand command,
      CancellationToken cancellationToken = default);

  Task RenameAsync(
      Guid id,
      RenameOriginArchetypeCommand command,
      CancellationToken cancellationToken = default);

  Task RetireAsync(
      Guid id,
      CancellationToken cancellationToken = default);
}