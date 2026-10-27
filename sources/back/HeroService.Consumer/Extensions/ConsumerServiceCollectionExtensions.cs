using Franz.Common.Business.Extensions;
using Franz.Common.EntityFramework.Auditing;
using Franz.Common.Http.EntityFramework.Extensions;
using Franz.Common.Mediator.Bootstrap;
using Franz.Common.Mediator.Extensions;
using HeroService.Application;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HeroService.Consumer.Extensions;

public static class ConsumerServiceCollectionExtensions
{
  public static IServiceCollection AddHeroServiceConsumerPersistence(
    this IServiceCollection services,
    IConfiguration configuration,
    IHostEnvironment environment)
  {
    services.RegisterApplicationServices();
    services.AddBusiness(typeof(SkillEffect).Assembly);

    services.AddRelationalDatabase<ApplicationDbContext>(environment, configuration);
    services.AddFranzAuditing();
    services.RegisterPersistenceServices<ApplicationDbContext>(configuration);

    services.AddFranzMediatorV2Default(null, typeof(SkillEffect).Assembly);

    return services;
  }
}
