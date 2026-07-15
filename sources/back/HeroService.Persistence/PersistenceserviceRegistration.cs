using Franz.Common.Caching.Extensions;
using Franz.Common.EntityFramework.Extensions;
using HeroService.Persistence.Persistence.Seeding;
using HeroService.Persistence.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HeroService.Persistence
{
  public static class ServiceCollectionExtensions
  {

    public static IServiceCollection AddCustomRepositoriesFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
    {
      var repoTypes = assembly
          .GetTypes()
          .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase));

      foreach (var impl in repoTypes)
      {
        var iface = impl.GetInterfaces().FirstOrDefault(i => i.Name == "I" + impl.Name);
        if (iface != null)
          services.AddScoped(iface, impl);
      }

      return services;
    }
    public static IServiceCollection RegisterPersistenceServices<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration)
    {
      // Add HeroService Redis Caching +Mediator Caching pipeline

      services
        .AddFranzRedisCaching(configuration);        
      services.AddUnitOfWork<ApplicationDbContext>();
      services.AddCustomRepositoriesFromAssembly(typeof(ApplicationDbContext).Assembly);
      services.AddEntityRepositories<ApplicationDbContext>();
      services.AddScoped<ISeeder, GameVersionSeeder>();
      services.AddScoped<ISeeder, IdentitySeeder>();
      services.AddScoped<ISeeder, SkillSeeder>();
      services.AddScoped<ISeeder, SkillModifierSeeder>();
      services.AddScoped<ISeeder, HeroSeeder>();
      services.AddScoped<ISeeder, HeroModifierSeeder>();
      services.AddScoped<DatabaseSeeder>();

      return services;
    }
  }
}
