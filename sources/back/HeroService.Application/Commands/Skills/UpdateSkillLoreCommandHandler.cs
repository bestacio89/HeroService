using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Skills;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;

public sealed class UpdateSkillLoreCommandHandler
    : ICommandHandler<UpdateSkillLoreCommand>
{
  private readonly ISkillLoreRepository _repo;
  private readonly IEntityRepository<SkillLore,Guid> _genericrepo;

  public UpdateSkillLoreCommandHandler(ISkillLoreRepository repo, IEntityRepository<SkillLore, Guid> genericrepo)
  {
    _repo = repo;
    _genericrepo = genericrepo;
  }

  public async Task Handle(UpdateSkillLoreCommand command, CancellationToken ct)
  {
    var lore = await _repo.GetBySkillIdAsync(command.SkillId, ct)
        ?? throw new InvalidOperationException("SkillLore not found.");

    lore.Update(
        command.Description,
        command.VisualExplanation,
        command.UpdatedBy
    );

    await _genericrepo.UpdateAsync(lore, ct);
  }
}