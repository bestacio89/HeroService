using Franz.Common.Mediator.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Commands.Heroes.Classifications.OriginCulture;

public sealed record CreateOriginCultureCommand(string Name) : ICommand<Guid>;
