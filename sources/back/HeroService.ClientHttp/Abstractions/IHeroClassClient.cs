using HeroService.Contracts.Commands.Heroes;
using HeroService.Contracts.DTOs.Heroes;

namespace HeroService.Client.Http.Abstractions;

public interface IHeroClassClient
{
  // READ
  Task<IReadOnlyCollection<HeroClassDto>> GetAllAsync(CancellationToken ct = default);

  Task<HeroClassDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

  Task<HeroClassDto?> GetByNameAsync(string name, CancellationToken ct = default);

  // WRITE (matches controller POST)
  Task<Guid> CreateAsync(CreateHeroClassCommand command, CancellationToken ct = default);
}