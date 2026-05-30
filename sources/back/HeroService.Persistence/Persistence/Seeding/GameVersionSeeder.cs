using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using HeroService.Persistence.Persistence.Seeding;

namespace HeroService.Persistence.Seeding;

public sealed class GameVersionSeeder : ISeeder2
{
  private readonly IEntityFactory<Guid, GameVersion> _gameVersionFactory;
  private readonly IEntityRepository<GameVersion, Guid> _gameVersionRepository;

  public GameVersionSeeder(
      IEntityFactory<Guid, GameVersion> gameVersionFactory,
      IEntityRepository<GameVersion, Guid> gameVersionRepository)
  {
    _gameVersionFactory = gameVersionFactory;
    _gameVersionRepository = gameVersionRepository;
  }

  public int Order => 1;

  public async Task SeedAsync(CancellationToken ct)
  {
    var existing = await _gameVersionRepository.GetAllAsync(ct);

    if (existing.Any())
      return;

    var version = _gameVersionFactory.Create();

    version.Define("1.0.0", "seed-system");
    version.Activate("seed-system");

    await _gameVersionRepository.AddAsync(version, ct);
  }
}