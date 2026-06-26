using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Domain.Heroes.Versioned.GameVersion;

namespace HeroService.Application.Services.GameVersions;

public sealed class GameVersionService
{
  private readonly IGameVersionRepository _repository;
  private readonly IEntityRepository<GameVersion, Guid> _entityRepository;
  private readonly IEntityFactory<Guid, GameVersion> _factory;

  public GameVersionService(
      IGameVersionRepository repository,
      IEntityRepository<GameVersion, Guid> entityRepository,
      IEntityFactory<Guid, GameVersion> factory)
  {
    _repository = repository;
    _entityRepository = entityRepository;
    _factory = factory;
  }

  public async Task<Guid> CreateAndActivateAsync(
      string versionNumber,
      string versionName,
      string createdBy,
      CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(versionNumber))
      throw new ArgumentException("Version number is required.", nameof(versionNumber));

    if (string.IsNullOrWhiteSpace(versionName))
      throw new ArgumentException("Version name is required.", nameof(versionName));

    // =========================================================
    // 1. deactivate current active version
    // =========================================================
    var active = await _repository.GetActiveAsync(cancellationToken);

    if (active is not null)
    {
      active.Deactivate(createdBy);
      await _entityRepository.UpdateAsync(active, cancellationToken);
    }

    // =========================================================
    // 2. create new version via factory
    // =========================================================
    var newVersion = _factory.Create();

    newVersion.Define(versionNumber, versionName, createdBy);
    newVersion.Activate(createdBy);

    await _entityRepository.AddAsync(newVersion, cancellationToken);

    return newVersion.Id;
  }
}