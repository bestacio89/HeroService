using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Domain.IdGenerators;
using Franz.Common.Mapping.Extensions;
using Franz.Common.Mediator.Extensions;
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
    collection.AddScoped<IHeroCreationService, HeroCreationService>();
    collection.AddScoped<ISkillCreationService, SkillCreationService>();
    collection.AddScoped<GameVersionService>();
    collection.AddScoped<SnapshotResolver>();
    collection.AddScoped<HeroSnapshotResolutionService>();
    collection.AddScoped<IHeroUniquenessValidator, HeroUniquenessValidator>();
    collection.AddFranzTransactionPipeline();

    return collection;
  }
}
