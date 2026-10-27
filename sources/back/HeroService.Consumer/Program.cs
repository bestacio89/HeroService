using Franz.Common.Business.Extensions;
using Franz.Common.EntityFramework.Auditing;
using Franz.Common.Http.EntityFramework.Extensions;
using Franz.Common.Mediator.Bootstrap;
using Franz.Common.Messaging.Bootstrap.Extenstions;
using HeroService.Application;
using HeroService.Consumer.Extensions;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HeroService.Consumer;

public static class Program
{
  public static async Task Main(string[] args)
  {
    using IHost host = Host.CreateDefaultBuilder(args)
      .ConfigureAppConfiguration((context, config) =>
      {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
          .AddEnvironmentVariables();
      })
      .ConfigureServices((context, services) =>
      {
        var configuration = context.Configuration;
        var environment = context.HostingEnvironment;

        services.RegisterApplicationServices();
        services.AddBusiness(typeof(SkillEffect).Assembly);

        services.AddRelationalDatabase<ApplicationDbContext>(environment, configuration);
        services.AddFranzAuditing();
        services.RegisterPersistenceServices<ApplicationDbContext>(configuration);

        services.AddFranzMediatorV2Default(null, typeof(SkillEffect).Assembly);

        services.AddMessagingArchitecture(environment, configuration);
        services.AddKafkaMessagingServices(configuration);
      })
      .Build();

    await host.RunAsync();
  }
}
