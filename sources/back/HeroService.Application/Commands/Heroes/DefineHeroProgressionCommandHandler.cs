
using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.EntityFramework;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Heroes;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Domain.Heroes.Progression;


namespace HeroService.Application.Commands.Heroes;

public sealed class DefineHeroProgressionCommandHandler : ICommandHandler<DefineHeroProgressionCommand, Guid>
{
  private readonly IEntityFactory<Guid, HeroProgressionModifiers> _factory;
  private readonly IEntityRepository<HeroProgressionModifiers, Guid> _repo;
  private readonly IUnitOfWork _uow;
  private readonly IFranzMapper _mapper;

  public DefineHeroProgressionCommandHandler(
      IEntityFactory<Guid, HeroProgressionModifiers> factory,
      IEntityRepository<HeroProgressionModifiers, Guid> repo,
      IUnitOfWork uow,
      IFranzMapper mapper)
  {
    _factory = factory;
    _repo = repo;
    _uow = uow;
    _mapper = mapper;
  }

  public async Task<Guid> Handle(DefineHeroProgressionCommand request, CancellationToken ct)
  {
    var existing = (await _repo.GetAllAsync(ct)).FirstOrDefault(p => p.HeroId == request.HeroId);
    if (existing != null)
      throw new InvalidOperationException($"Progression modifiers already defined for Hero: {request.HeroId}");

    var modifiers = _factory.Create();

    modifiers.Define(
        request.HeroId,
        request.HealthPerLevel,
        request.ManaPerLevel,
        request.AttackDamagePerLevel,
        request.AbilityPowerPerLevel,
        request.ArmorPerLevel,
        request.MagicResistancePerLevel,
        request.AttackSpeedPerLevel,
        request.CastSpeedPerLevel,
        request.ResourceRegenerationPerLevel,
        request.CreatedBy);

    await _repo.AddAsync(modifiers, ct);
    await _uow.CommitAsync(ct);

    return modifiers.Id;
  }
}