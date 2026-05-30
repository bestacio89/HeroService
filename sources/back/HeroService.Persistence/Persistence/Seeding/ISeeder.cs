using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Persistence.Persistence.Seeding;

public interface ISeeder2
{
  int Order { get; }
  Task SeedAsync(CancellationToken ct);
}