using Franz.Common.Messaging.Bootstrap.Extenstions;
using HeroService.Consumer.Extensions;
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

        services.AddHeroServiceConsumerPersistence(configuration, environment);
        services.AddMessagingArchitecture(environment, configuration);
        services.AddKafkaMessagingServices(configuration, environment.EnvironmentName);
      })
      .Build();

    await host.RunAsync();
  }
}
