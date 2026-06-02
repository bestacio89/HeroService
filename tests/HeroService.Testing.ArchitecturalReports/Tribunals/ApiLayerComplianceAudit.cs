using System;
using System.Linq;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using HeroServiceTesting;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace HeroService.Testing.ArchitecturalReports.Layers;

/// <summary>
/// HeroService Tribunal — API Layer Compliance Audit
/// Mirrors ApiArchitectureTests exactly and produces a consolidated report.
/// </summary>
public sealed class ApiLayerComplianceAudit : ArchitecturalAuditBase
{
 
  [Trait("Category", "ArchitecturalReport")]
  public void Audit_ApiLayer_Compliance()
  {
    ExecuteTribunal("API Layer Compliance Audit", (sb, markViolation) =>
    {
      // -------------------------------------------------------------
      // RULE 1 — Assembly Presence
      // Mirrors: Api_Assembly_Should_Exist()
      // -------------------------------------------------------------
      ExecuteRule(
          "Assembly Presence",
          "API assembly must be detected.",
          () =>
          {
            Assert.NotNull(ApiAssembly);
          },
          sb,
          markViolation);

      // -------------------------------------------------------------
      // RULE 2 — Controller Conventions
      // Mirrors: Controllers_AreLocatedCorrectly()
      // -------------------------------------------------------------
      var apiAssembly = typeof(HeroService.Api.Controllers.SkillController).Assembly;

      var controllerRule = ArchRuleDefinition
          .Classes()
          .That()
          .ResideInAssembly(apiAssembly)
          .And()
          .HaveNameEndingWith("Controller")
          .Should()
          .BeAssignableTo(typeof(ControllerBase))
          .AndShould()
          .ResideInNamespace("HeroService.API.Controllers")
          .WithoutRequiringPositiveResults();

      ExecuteRule(
          "Controller Conventions",
          "Controllers must derive from ControllerBase and reside in HeroService.API.Controllers.",
          controllerRule,
          sb,
          markViolation);

      // -------------------------------------------------------------
      // RULE 3 — Dependency Isolation
      // Mirrors: DependencyToContractsExists()
      // -------------------------------------------------------------
      var contractObjects = ContractsLayer
          .GetObjects(BaseArchitecture)
          .Where(t =>
              t.Name.StartsWith("I", StringComparison.Ordinal) ||
              t.Name.EndsWith("Command", StringComparison.Ordinal) ||
              t.Name.EndsWith("Query", StringComparison.Ordinal))
          .ToList();

      if (!contractObjects.Any())
      {
        sb.AppendLine("[SKIP] Dependency Isolation — No contract interfaces or message definitions found.");
      }
      else
      {
        var apiClasses = BaseArchitecture
            .Classes
            .Where(t =>
                t.Assembly.Name.Equals(
                    "HeroService.API",
                    StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!apiClasses.Any())
        {
          sb.AppendLine("[SKIP] Dependency Isolation — No API layer types found.");
        }
        else
        {
          var dependencyRule = ArchRuleDefinition
              .Classes()
              .That()
              .ResideInAssembly("HeroService.API")
              .And()
              .DoNotHaveNameEndingWith("Program")
              .And()
              .DoNotHaveNameEndingWith("Startup")
              .Should()
              .OnlyDependOnTypesThat()
              .ResideInAssembly("HeroService.API")
              .OrShould()
              .ResideInAssembly("HeroService.Contracts")
              .OrShould()
              .ResideInAssembly("Franz.Common")
              .Because(
                  "API components (except composition root) should depend only on Contracts and Common abstractions.")
              .WithoutRequiringPositiveResults();

          ExecuteRule(
              "Dependency Isolation",
              "API components must not bypass the Contracts layer.",
              dependencyRule,
              sb,
              markViolation);
        }
      }

      sb.AppendLine("---------------------------------------------------------------");
      sb.AppendLine("🏛️ HeroService.API Audit Final Processing: Complete");
    });
  }
}