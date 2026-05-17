using Franz.Common.Mediator.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Commands.Heroes.Classifications.MythologyTypes;

public sealed record RenameMythologyTypeCommand(Guid Id, string Name) : ICommand;
