
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeroService.API.Extensions
{
  public static class ApiRefitExtensions
  {
    /// <summary>
    /// Registers external Refit clients defined in appsettings.json
    /// </summary>
    /*  public static IServiceCollection AddExternalServices(
          this IServiceCollection services,
          IConfiguration configuration)
      {
        // Example: External service "Catalog"
        /*   var catalogBaseUrl = configuration["ExternalServices:Catalog:BaseUrl"];
         if (!string.IsNullOrWhiteSpace(catalogBaseUrl))
         {
           services.AddHeroServiceRefit<ICatalogClient>(
               name: "Catalog",
               baseUrl: catalogBaseUrl,
               policyName: configuration["ExternalServices:Catalog:Policy"] // optional Polly policy
           );
         }

         // Example: External service "Payments"
         var paymentsBaseUrl = configuration["ExternalServices:Payments:BaseUrl"];
         if (!string.IsNullOrWhiteSpace(paymentsBaseUrl))
         {
           services.AddHeroServiceRefit<IPaymentsClient>(
               name: "Payments",
               baseUrl: paymentsBaseUrl,
               policyName: configuration["ExternalServices:Payments:Policy"]
           );
         }

         // Add more services here as needed
         return services;
  }*/
}
}
