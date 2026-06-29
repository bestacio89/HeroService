
using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.EntityFramework;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Pipelines.Core;
using HeroService.Contracts.Commands.Skills;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;
using Microsoft.Azure.Cosmos;


namespace HeroService.Application.Commands.Skills;

public sealed class DefineSkillScalingCommandHandler : ICommandHandler<DefineSkillScalingCommand, Guid>
{
  private readonly IEntityFactory<Guid, SkillScalingModifier> _factory;
  private readonly IEntityRepository<SkillScalingModifier, Guid> _repo;
  private readonly IUnitOfWork _uow;
  private readonly IFranzMapper _mapper;

  public DefineSkillScalingCommandHandler(
      IEntityFactory<Guid, SkillScalingModifier> factory,
      IEntityRepository<SkillScalingModifier, Guid> repo,
      IUnitOfWork uow,
      IFranzMapper mapper)
  {
    _factory = factory;
    _repo = repo;
    _uow = uow;
    _mapper = mapper;
  }

  public async Task<Guid> Handle(DefineSkillScalingCommand request, CancellationToken ct)
  {
    var existing = (await _repo.GetAllAsync(ct)).FirstOrDefault(s => s.SkillId == request.SkillId);
    if (existing != null)
      throw new InvalidOperationException($"Scaling profile already defined for Skill: {request.SkillId}");

    var profile = _factory.Create();

    profile.Define(
        request.SkillId,
        request.AttackDamageRatio,
        request.AbilityPowerRatio,
        request.MaxHealthRatio,
        request.CreatedBy);

    await _repo.AddAsync(profile, ct);
    await _uow.CommitAsync(ct);

    return profile.Id;
  }
}