using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Heroes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Queries.Heroes.Classification.MythologyTypes;

public sealed record GetMythologyTypeByNameQuery(string Name)
    : IQuery<MythologyTypeDto>;