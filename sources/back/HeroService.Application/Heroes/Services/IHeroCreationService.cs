using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.DTOs.Requests;

namespace HeroService.Application.Heroes.Services;

public interface IHeroCreationService
{
  Task<Guid> CreateAsync(
      HeroCreateRequest request,
      CancellationToken cancellationToken);
}