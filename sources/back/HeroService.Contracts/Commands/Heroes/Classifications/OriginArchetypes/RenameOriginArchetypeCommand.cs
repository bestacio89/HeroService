using Franz.Common.Mediator.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Commands.Heroes.Classifications.OriginArchetypes;

public sealed record RenameOriginArchetypeCommand(Guid Id, string Name) : ICommand;
