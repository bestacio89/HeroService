using HeroService.Contracts.DTOs.Heroes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Client.Http.Abstractions;

public interface IHeroClassClient
{
  Task<HeroClassDto?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken = default);

  Task<HeroClassDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<HeroClassDto>> GetAllAsync(
      CancellationToken cancellationToken = default);
}