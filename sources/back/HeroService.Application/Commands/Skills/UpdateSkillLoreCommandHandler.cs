using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Domain.Heroes.Skills;

public sealed class UpdateSkillLoreCommandHandler
    : ICommandHandler<UpdateSkillLoreCommand>
{
  private readonly IEntityRepository<SkillLore, Guid> _repo;

  public UpdateSkillLoreCommandHandler(IEntityRepository<SkillLore, Guid> repo)
  {
    _repo = repo;
  }

  public async Task Handle(UpdateSkillLoreCommand command, CancellationToken ct)
  {
    var lore = await _repo.GetByIdAsync(command.SkillLoreId, ct)
        ?? throw new InvalidOperationException("SkillLore not found.");

    lore.Update(
        command.Name,
        command.Description,
        command.VisualExplanation,
        command.UpdatedBy
    );

    await _repo.UpdateAsync(lore, ct);
  }
}