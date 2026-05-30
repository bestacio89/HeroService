using Franz.Common.Business.Repositories;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Domain.Heroes.Versioned.GameVersion;

namespace HeroService.Application.Services.GameVersions;

public sealed class GameVersionService
{
  private readonly IGameVersionRepository _repository;
  private readonly IEntityRepository<GameVersion, Guid> _entityRepository;

  public GameVersionService(IGameVersionRepository repository, IEntityRepository<GameVersion, Guid> entityRepository)
  {
    _repository = repository;
    _entityRepository = entityRepository;
  }

  public async Task<Guid> CreateAndActivateAsync(
      string versionName,
      string createdBy,
      CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(versionName))
      throw new ArgumentException("Version name is required.");

    // 1. get active version (domain-specific query)
    var active = await _repository.GetActiveAsync(cancellationToken);

    if (active is not null)
    {
      active.Deactivate(createdBy);
      await _entityRepository.UpdateAsync(active, cancellationToken);
    }

    // 2. create new version
    var newVersion = new GameVersion(versionName, createdBy);
    newVersion.Activate(createdBy);

    await _entityRepository.AddAsync(newVersion, cancellationToken);

    return newVersion.Id;
  }
}