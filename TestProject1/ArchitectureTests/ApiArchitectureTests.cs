using ArchUnitNET;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Franz.Common.Mediator.Messages;
using HeroServiceTesting;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using ControllerBase =  Microsoft.AspNetCore.Mvc.ControllerBase;

namespace HeroService.Testing.ArchitectureTests;
public class ApiArchitectureTests : BaseArchitectureTest
{
  [Fact]
  public void Api_Assembly_Should_Exist()
  {
    Assert.NotNull(ApiLayer); // Leverages the provider from BaseArchitectureTest
  }

  [Fact]
  public void Controllers_AreLocatedCorrectly()
  {
    // 1. Get the assembly object for your API project
    var apiAssembly = typeof(HeroService.Api.Controllers.SkillController).Assembly;

    // 2. Filter for only types in your specific assembly first
    var controllerRule = ArchRuleDefinition
        .Classes()
        .That()
        .ResideInAssembly(apiAssembly) // <-- THIS IS THE KEY
        .And()
        .HaveNameEndingWith("Controller")
        .Should()
        .BeAssignableTo(typeof(ControllerBase))
        .AndShould()
        .ResideInNamespace("HeroService.API.Controllers");

    // 3. Execute the check
    controllerRule.Check(BaseArchitecture);
  }

  [Fact]
  public void DependencyToContractsExists()
  {
    var contractObjects = ContractsLayer
        .GetObjects(BaseArchitecture)
        .Where(t =>
            t.Name.StartsWith("I", StringComparison.Ordinal) ||
            t.Name.EndsWith("Command", StringComparison.Ordinal) ||
            t.Name.EndsWith("Query", StringComparison.Ordinal))
        .ToList();

    if (!contractObjects.Any())
    {
      Console.WriteLine("?? No contract interfaces or message definitions found — skipping enforcement (template mode).");
      return;
    }

    var apiClasses = BaseArchitecture
        .Classes
        .Where(t => t.Assembly.Name.Equals("HeroService.API", StringComparison.OrdinalIgnoreCase))
        .ToList();

    if (!apiClasses.Any())
    {
      Console.WriteLine("?? No API layer types found — skipping dependency validation.");
      return;
    }

    // Build rule — pass allowed assemblies as separate params
    ArchRuleDefinition
      .Classes()
      .That()
      .ResideInAssembly("HeroService.API")
      .And()
      .DoNotHaveNameEndingWith("Program")
      .And()
      .DoNotHaveNameEndingWith("Startup")
      .Should()
      .OnlyDependOnTypesThat()
      .ResideInAssembly(
          "HeroService.API"       
      )
      .OrShould()
      .ResideInAssembly(
          "HeroService.Contracts")
      .OrShould()
      .ResideInAssembly(
          "Franz.Common")
      .Because("API components (except composition root) should depend only on Contracts and Common abstractions.")
      .WithoutRequiringPositiveResults()
      .Check(BaseArchitecture);
  }




}
