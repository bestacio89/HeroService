using Franz.Common.Mediator.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Commands.Heroes.Classifications.NewFolder;

public sealed record CreateOriginArchetypeCommand(string Name) : ICommand<Guid>;
