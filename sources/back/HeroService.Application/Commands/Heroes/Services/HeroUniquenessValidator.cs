using Franz.Common.Business.Repositories;
using Franz.Common.Errors;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Domain.Heroes.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Application.Commands.Heroes.Services;

public sealed class HeroUniquenessValidator : IHeroUniquenessValidator
{
  private readonly IHeroRepository _heroes;

  public HeroUniquenessValidator(IHeroRepository heroes)
  {
    _heroes = heroes;
  }

  public async Task EnsureUniqueHeroNameAsync(string name, CancellationToken ct)
  {
        var exists = await _heroes.ExistsByNameAsync(
          name,
          ct);

    if (exists)
      throw new TechnicalException($"Hero '{name}' already exists.");
  }
}