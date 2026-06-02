using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HeroService.Testing.ArchitectureTests;
public class PersistenceArchitectureTests : BaseArchitectureTest
{
  private Assembly[] LoadAssembliesWithPattern(string assemblyPattern)
  {
    var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    var matchingAssemblies = loadedAssemblies
        .Where(assembly => Regex.IsMatch(assembly.FullName, assemblyPattern))
        .ToArray();

    return matchingAssemblies;
  }

  [Fact]
  public void PersistenceAssemblyExistence()
  {
    Assert.NotNull(PersistenceAssembly);
  }

  [Fact]
  public void PersistenceAssemblyDependencies()
  {
    var persistenceTypes = LoadAssembliesWithPattern(@".*\.Persistence\..*")
        .SelectMany(assembly => assembly.GetTypes());

    foreach (var type in persistenceTypes)
    {
      var dependencies = type.GetTypeInfo().ImplementedInterfaces;
      Assert.True(dependencies.Any(dep => dep.FullName.EndsWith("Domain.dll") || dep.FullName.EndsWith("Contracts.dll")));
    }
  }

  [Fact]
  public void PersistenceLayerDependencies_AreCorrect()
  {
    ReportArchitectureContext();

    ArchRuleDefinition
        .Classes()
        .That()
        .ResideInAssembly(PersistenceAssembly)
        .Should()
        .OnlyDependOnTypesThat()
        // Whitelist all Franz.Common and Microsoft infra
        .ResideInNamespaceMatching("Franz\\.Common\\..*")
        .OrShould().ResideInNamespaceMatching("Microsoft\\..*") // Catch EF Core & DI in one

        // Domain & Contracts
        .OrShould().ResideInNamespaceMatching("HeroService\\.Contracts\\..*")
        .OrShould().ResideInNamespaceMatching("HeroService\\.Domain\\..*")
        .OrShould().ResideInNamespaceMatching("HeroService\\.Persistence.*")

        // Broad BCL Whitelist
        .OrShould().ResideInNamespaceMatching("System.*")
        .OrShould().ResideInNamespaceMatching("System")

        .Because("Persistence layer must rely on EF Core, Domain, Contracts, and BCL plumbing.")
        .WithoutRequiringPositiveResults()
        .Check(BaseArchitecture);
  }




  [Fact]
  public void RepositoryImplementationsExist()
  {
    var persistenceTypes = LoadAssembliesWithPattern(@".*\.Persistence\..*")
        .SelectMany(assembly => assembly.GetTypes());

    foreach (var type in persistenceTypes)
    {
      if (type.Name.EndsWith("Repository") && type.IsClass)
      {
        // Check for implementation of a repository interface
        var repositoryInterface = type.GetInterfaces()
            .FirstOrDefault(interf => interf.Name.EndsWith("Repository"));

        Assert.NotNull(repositoryInterface);
      }
    }
  }
}
