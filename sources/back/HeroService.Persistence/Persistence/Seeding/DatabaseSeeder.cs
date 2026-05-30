using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Persistence.Persistence.Seeding;

public sealed class DatabaseSeeder
{
  private readonly IEnumerable<ISeeder2> _seeders;

  public DatabaseSeeder(IEnumerable<ISeeder2> seeders)
  {
    _seeders = seeders;
  }

  public async Task RunAsync(CancellationToken ct)
  {
    foreach (var seeder in _seeders.OrderBy(x => x.Order))
    {
      await seeder.SeedAsync(ct);
    }
  }
}