using Franz.Common.Http.MultiTenancy.Accessors;
using Franz.Common.Identity;
using Franz.Common.Messaging.Contexting;
using Franz.Common.MultiTenancy;
using HeroService.Consumer.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeroService.Testing.Consumer;

public sealed class MonotenantMessagingRegistrationTests
{
  [Fact]
  public void MessagingArchitecture_RegistersIdentityAndHeadersWithoutTenantContexts()
  {
    var services = new ServiceCollection();
    var configuration = new ConfigurationBuilder().Build();

    services.AddHeroServiceMessagingArchitecture(configuration);

    Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IIdentityContextAccessor));
    Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IHeaderContextAccessor));

    Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(ITenantContextAccessor));
    Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(ITenantExecutionContextAccessor));
    Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(IDomainContextAccessor));
  }
}
