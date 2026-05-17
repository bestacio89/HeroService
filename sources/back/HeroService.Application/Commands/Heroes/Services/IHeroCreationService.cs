using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.DTOs.Requests;

namespace HeroService.Application.Commands.Heroes.Services;

public interface IHeroCreationService
{
  Task<Guid> CreateAsync(
      HeroCreateRequest request,
      CancellationToken cancellationToken);
}