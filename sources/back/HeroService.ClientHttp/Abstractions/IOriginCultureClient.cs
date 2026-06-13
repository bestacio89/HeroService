using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Commands.Heroes.Classifications.OriginCulture;

namespace HeroService.Client.Http.Abstractions;

public interface IOriginCultureClient
{
  Task<IReadOnlyList<OriginCultureDto>> GetAllAsync(
      CancellationToken cancellationToken = default);

  Task<OriginCultureDto?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken = default);

  Task<OriginCultureDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default);

  Task<Guid> CreateAsync(
      CreateOriginCultureCommand command,
      CancellationToken cancellationToken = default);

  Task RenameAsync(
      Guid id,
      RenameOriginCultureCommand command,
      CancellationToken cancellationToken = default);

  Task RetireAsync(
      Guid id,
      CancellationToken cancellationToken = default);
}