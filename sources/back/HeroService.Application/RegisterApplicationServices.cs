using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Domain.IdGenerators;
using Franz.Common.Mapping.Extensions;
using HeroService.Application.Commands.Heroes.Services;
using HeroService.Application.Commands.Skills.Services;
using HeroService.Application.Heroes.Versioned.Snapshotting;
using HeroService.Application.Services.GameVersions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace HeroService.Application;
public static class ApplicationServiceCollectionExtensions
{
  public static IServiceCollection RegisterApplicationServices(this IServiceCollection collection)
  {


   collection.AddFranzMapping(Assembly.GetExecutingAssembly());
    // Hero creation
   collection.AddScoped<IHeroCreationService, HeroCreationService>();

    // Skill creation  
   collection.AddScoped<ISkillCreationService, SkillCreationService>();

    // Game version
   collection.AddScoped<GameVersionService>();

    // Snapshot resolution (two handlers depend on it)
   collection.AddScoped<SnapshotResolver>();

   collection.AddScoped<IHeroUniquenessValidator, HeroUniquenessValidator>();

    return collection;
  }

}
