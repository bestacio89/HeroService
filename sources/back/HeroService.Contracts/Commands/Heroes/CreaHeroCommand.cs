using Franz.Common.Mediator.Messages;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Commands.Heroes;

public sealed record CreateHeroCommand(
    string Name,
    Guid HeroClassId,
    OriginType OriginType,
    string MythologyCode
) : ICommand<Guid>;