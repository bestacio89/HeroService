namespace HeroService.Client.Http.Abstractions;

public interface IAdminLifecycleClient<TDto, TCreate, TRename>
{
  Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken ct = default);

  Task<TDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

  Task<TDto?> GetByNameAsync(string name, CancellationToken ct = default);

  Task<Guid> CreateAsync(TCreate command, CancellationToken ct = default);

  Task RenameAsync(Guid id, TRename command, CancellationToken ct = default);

  Task RetireAsync(Guid id, CancellationToken ct = default);
}