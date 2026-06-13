using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Commands.Heroes.Classifications.MythologyTypes;

namespace HeroService.Client.Http.Abstractions;

public interface IMythologyTypeClient
{
  Task<Guid> CreateAsync(
      CreateMythologyTypeCommand command,
      CancellationToken cancellationToken = default);

  Task RenameAsync(
      Guid id,
      RenameMythologyTypeCommand command,
      CancellationToken cancellationToken = default);

  Task RetireAsync(
      Guid id,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyList<MythologyTypeDto>> GetAllAsync(
      CancellationToken cancellationToken = default);

  Task<MythologyTypeDto?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken = default);

  Task<MythologyTypeDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default);
}