using Franz.Common.Mediator.Handlers;
using HeroService.Application.Commands.Skills.Services;
using HeroService.Contracts.Commands.Skills;

namespace HeroService.Application.Commands.Skills;

public sealed class CreateSkillCommandHandler
    : ICommandHandler<CreateSkillCommand, Guid>
{
  private readonly ISkillCreationService _creationService;

  public CreateSkillCommandHandler(
      ISkillCreationService creationService)
  {
    _creationService = creationService;
  }

  public async Task<Guid> Handle(
      CreateSkillCommand command,
      CancellationToken ct)
  {
    return await _creationService.CreateAsync(
        command.Request,
        ct);
  }
}