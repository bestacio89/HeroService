using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Domain.IdGenerators;
using Franz.Common.Mapping.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace HeroService.Application;
public static class ApplicationServiceCollectionExtensions
{
  public static IServiceCollection RegisterApplicationServices(this IServiceCollection collection)
  {


   collection.AddFranzMapping(Assembly.GetExecutingAssembly());
   collection.AddSingleton<IIdGenerator<Guid>, GuidV7Generator>();
   collection.AddTransient(typeof(IEntityFactory<,>), typeof(EntityFactory<,>));

    return collection;
  }

}
