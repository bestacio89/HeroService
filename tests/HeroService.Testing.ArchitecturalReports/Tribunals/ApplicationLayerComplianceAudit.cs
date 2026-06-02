using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Messages;
using HeroServiceTesting;
using System;
using System.Linq;
using Xunit;

namespace HeroService.Testing.ArchitecturalReports.Layers;

/// <summary>
/// HeroService Tribunal — Application Layer Compliance Audit
/// Mirrors ApplicationArchitectureTests exactly and produces a consolidated report.
/// </summary>
public sealed class ApplicationLayerComplianceAudit : ArchitecturalAuditBase
{
 
  [Trait("Category", "ArchitecturalReport")]
  public void Audit_ApplicationLayer_Compliance()
  {
    ExecuteTribunal("Application Layer Compliance Audit", (sb, markViolation) =>
    {
      // -------------------------------------------------------------
      // RULE 1 — Assembly Presence
      // Mirrors: Application_Assembly_Should_Exist
      // -------------------------------------------------------------
      ExecuteRule(
          "Assembly Presence",
          "Application assembly must exist.",
          () =>
          {
            Assert.NotNull(ApplicationAssembly);
          },
          sb,
          markViolation);

      // -------------------------------------------------------------
      // RULE 2 — Command Handlers
      // Mirrors:
      // CommandHandlers_Should_Implement_ICommandHandler_And_Follow_Naming
      // -------------------------------------------------------------
      var commandHandlers = ApplicationLayer
          .GetObjects(BaseArchitecture)
          .Where(t => t.Name.EndsWith("CommandHandler", StringComparison.OrdinalIgnoreCase))
          .ToList();

      if (commandHandlers.Any())
      {
        var commandRule = ArchRuleDefinition
            .Classes()
            .That()
            .Are(commandHandlers)
            .Should()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .OrShould()
            .ImplementInterface(typeof(ICommandHandler<>))
            .AndShould()
            .HaveNameEndingWith("CommandHandler");

        ExecuteRule(
            "Command Handlers",
            "Command handlers must implement ICommandHandler and follow naming conventions.",
            commandRule,
            sb,
            markViolation);
      }

      // -------------------------------------------------------------
      // RULE 3 — Query Handlers
      // Mirrors:
      // QueryHandlers_Should_Implement_IQueryHandler_And_Follow_Naming
      // -------------------------------------------------------------
      var queryHandlers = ApplicationLayer
          .GetObjects(BaseArchitecture)
          .Where(t => t.Name.EndsWith("QueryHandler", StringComparison.OrdinalIgnoreCase))
          .ToList();

      if (queryHandlers.Any())
      {
        var queryRule = ArchRuleDefinition
            .Classes()
            .That()
            .Are(queryHandlers)
            .Should()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .AndShould()
            .HaveNameEndingWith("QueryHandler");

        ExecuteRule(
            "Query Handlers",
            "Query handlers must implement IQueryHandler and follow naming conventions.",
            queryRule,
            sb,
            markViolation);
      }

      // -------------------------------------------------------------
      // RULE 4 — Notification Handlers
      // Mirrors:
      // NotificationHandlers_Should_Implement_INotificationHandler...
      // -------------------------------------------------------------
      if (HasEventHandlers)
      {
        var notificationRule = ArchRuleDefinition
            .Classes()
            .That()
            .ImplementInterface(typeof(INotificationHandler<>))
            .And()
            .Are(ApplicationLayer)
            .Should()
            .HaveNameEndingWith("Handler");

        ExecuteRule(
            "Notification Handlers",
            "Notification handlers must follow naming conventions.",
            notificationRule,
            sb,
            markViolation);
      }

      // -------------------------------------------------------------
      // RULE 5 — Domain Events
      // Mirrors:
      // EventHandlers_Should_Implement_IEventHandler_And_Match_DomainEvents
      // -------------------------------------------------------------
      if (HasDomainEvents)
      {
        var pureDomainEvents = DomainEventTypes
            .Where(t =>
                !t.FullName.Contains("Validation", StringComparison.OrdinalIgnoreCase) &&
                !t.FullName.Contains("Notification", StringComparison.OrdinalIgnoreCase) &&
                !t.FullName.Contains("Pipeline", StringComparison.OrdinalIgnoreCase) &&
                !t.FullName.Contains("Mediator", StringComparison.OrdinalIgnoreCase) &&
                !t.FullName.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (pureDomainEvents.Any())
        {
          var domainEventRule = ArchRuleDefinition
              .Classes()
              .That()
              .Are(pureDomainEvents)
              .Should()
              .ImplementAnyInterfacesThat()
              .HaveFullName("Franz.Common.Business.Events.IDomainEvent")
              .OrShould()
              .ImplementAnyInterfacesThat()
              .HaveFullName("Franz.Common.Business.Events.IEvent")
              .AndShould()
              .HaveNameEndingWith("Event");

          ExecuteRule(
              "Domain Events",
              "Domain events must implement IDomainEvent/IEvent and follow naming conventions.",
              domainEventRule,
              sb,
              markViolation);
        }

        if (ApplicationEventHandlerTypes.Any())
        {
          var eventHandlerRule = ArchRuleDefinition
              .Classes()
              .That()
              .Are(ApplicationEventHandlerTypes)
              .Should()
              .ImplementAnyInterfacesThat()
              .HaveFullName("Franz.Common.Mediator.Handlers.IEventHandler`1")
              .OrShould()
              .ImplementAnyInterfacesThat()
              .HaveFullName("Franz.Common.Mediator.Handlers.INotificationHandler`1")
              .AndShould()
              .HaveNameEndingWith("Handler");

          ExecuteRule(
              "Event Handlers",
              "Event handlers must implement IEventHandler or INotificationHandler.",
              eventHandlerRule,
              sb,
              markViolation);
        }
      }

      // -------------------------------------------------------------
      // RULE 6 — Dependency Purity
      // Mirrors:
      // ApplicationLayer_Should_Depend_Only_On_Allowed_Namespaces
      // -------------------------------------------------------------
      var dependencyRule = ArchRuleDefinition
          .Types()
          .That()
          .Are(ApplicationLayer)
          .Should()
          .DependOnAnyTypesThat()
          .ResideInNamespace("Franz.Common.Business.Domain")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Business.Entities")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Business.Events")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mediator")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mediator.Core")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mediator.Handlers")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mediator.Pipelines")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mediator.Pipelines.Core")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mediator.Pipelines.Logging")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mediator.Pipelines.Validation")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mediator.Pipelines.Transaction")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mediator.Validation")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mediator.Extensions")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mapping")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mapping.Abstractions")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Mapping.Core")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Franz.Common.Logging")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("System")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("System.Threading")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("System.Threading.Tasks")
          .OrShould().DependOnAnyTypesThat().ResideInNamespace("Microsoft.Extensions.DependencyInjection");

      ExecuteRule(
          "Dependency Governance",
          "Application layer must only depend on approved namespaces.",
          dependencyRule,
          sb,
          markViolation);

      sb.AppendLine("---------------------------------------------------------------");
      sb.AppendLine("??? HeroService.Application Governance check complete.");
    });
  }
}