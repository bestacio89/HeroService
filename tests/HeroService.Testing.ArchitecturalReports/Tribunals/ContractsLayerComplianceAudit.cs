using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Franz.Common.Business.Domain;
using Franz.Common.Business.Repositories;
using Franz.Common.DependencyInjection;
using Franz.Common.Mediator.Messages;
using HeroServiceTesting;
using Xunit;

namespace HeroService.Testing.ArchitecturalReports.Layers
{
  public sealed class ContractsLayerComplianceAudit : ArchitecturalAuditBase
  {
    [Trait("Category", "ArchitecturalReport")]

    public void Contracts_Governance()
    {
      ExecuteTribunal("Contracts Layer Compliance Audit", (sb, markViolation) =>
      {
        var prefix = SolutionPrefix;

        // RULE 1 — Assembly Presence
        ExecuteRule("Assembly Presence", "Contracts assembly must be present.", () =>
        {
          Assert.NotNull(ContractsLayer);
        }, sb, markViolation);

        // RULE 2 — Query Naming
        ExecuteRule("Queries", "Queries must end with 'Query' and implement IQuery<>.", () =>
        {
          ArchRuleDefinition.Classes()
              .That().AreAssignableTo(typeof(IQuery<>))
              .Should().HaveNameEndingWith("Query")
              .Because("All query types should follow the 'SomethingQuery' naming pattern.")
              .Check(BaseArchitecture);
        }, sb, markViolation);

        // RULE 3 — Command Naming
        ExecuteRule("Commands", "Commands must end with 'Command' and implement ICommand.", () =>
        {
          ArchRuleDefinition.Classes()
              .That().AreAssignableTo(typeof(ICommand<>))
              .Or().AreAssignableTo(typeof(ICommand))
              .Should().HaveNameEndingWith("Command")
              .Because("All command types should follow the 'SomethingCommand' naming pattern.")
              .Check(BaseArchitecture);
        }, sb, markViolation);

        // RULE 4 — DTO Naming & Namespace
        ExecuteRule("DTOs", $"DTOs must end with 'Dto' and reside in {prefix}.Contracts.DTOs.", () =>
        {
          ArchRuleDefinition.Classes()
              .That().ResideInNamespaceMatching($"^{prefix}\\.Contracts\\.DTOs(\\..*)?$")
              .Should().HaveNameEndingWith("Dto")
              .Because("All DTOs must be suffixed with 'Dto' and reside in the Contracts.DTOs namespace.")
              .Check(BaseArchitecture);
        }, sb, markViolation);

        // RULE 5 — DTO Immutability
        ExecuteRule("DTO Immutability", "DTOs must be records or immutable types.", () =>
        {
          var dtoObjects = ContractsLayer.GetObjects(BaseArchitecture)
              .Where(t => t.Name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase) ||
                          t.Namespace.FullName.Contains("DTOs", StringComparison.OrdinalIgnoreCase))
              .ToList();

          var offenders = dtoObjects.Where(dto => {
            var type = GetReflectionType(dto);
            return type != null && !IsRecord(type) && HasWritableProperties(type);
          }).ToList();

          if (offenders.Any())
          {
            markViolation();
            foreach (var o in offenders) sb.AppendLine($"?? Mutable DTO detected: {o.FullName}");
          }
        }, sb, markViolation);

        // RULE 6 — Infrastructure Interfaces
        ExecuteRule("Infrastructure Interfaces", "Infrastructure interfaces must define lifetimes.", () =>
        {
          ArchRuleDefinition.Classes()
              .That().ResideInNamespaceMatching($"^{prefix}\\.Contracts\\.Infrastructure.*$")
              .Should().BeAssignableTo(typeof(IScopedDependency))
              .OrShould().BeAssignableTo(typeof(ISingletonDependency))
              .Because("Infrastructure interfaces must declare explicit lifetime scope.")
              .WithoutRequiringPositiveResults()
              .Check(BaseArchitecture);
        }, sb, markViolation);

        // RULE 7 — Custom Repositories
        ExecuteRule("Custom Repositories", "Custom repositories must declare scoped lifetime and remain independent.", () =>
        {
          ArchRuleDefinition.Classes()
              .That().ResideInNamespaceMatching($"^{prefix}.*Persistence.*$")
              .And().HaveNameEndingWith("Repository")
              .And().DoNotHaveNameMatching(".*(Aggregate|Read|Entity).*")
              .Should().BeAssignableTo(typeof(IScopedDependency))
              .AndShould().NotBeAssignableTo(typeof(IEntityRepository<,>))
              .AndShould().NotBeAssignableTo(typeof(IAggregateRepository<,>))
              .Because("Custom repositories must declare scoped lifetime and remain independent of framework abstractions.")
              .Check(BaseArchitecture);
        }, sb, markViolation);
      });
    }

    private static bool IsRecord(Type type) =>
        type.GetMethod("<Clone>$", BindingFlags.NonPublic | BindingFlags.Instance) != null ||
        type.GetMethod("PrintMembers", BindingFlags.NonPublic | BindingFlags.Instance) != null;

    private static bool HasWritableProperties(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p =>
        {
          var setMethod = p.SetMethod;
          if (setMethod == null) return false;
          return !setMethod.ReturnParameter.GetRequiredCustomModifiers()
              .Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
        });

    private static Type? GetReflectionType(IType archType)
    {
      var maybeTypeProp = archType.GetType().GetProperty("Type", BindingFlags.Public | BindingFlags.Instance);
      if (maybeTypeProp?.GetValue(archType) is Type systemType) return systemType;
      return Type.GetType(archType.FullName, throwOnError: false);
    }
  }
}