using HeroService.Client.Http.Abstractions;
using HeroService.Client.Http.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace HeroService.Testing.ClientHttp;

public sealed class HeroServiceClientRegistrationTests
{
  [Fact]
  public void AddHeroServiceClients_ConfiguresJsonAcceptWithoutVersionParameters()
  {
    var services = new ServiceCollection();
    services.AddHeroServiceClients("https://hero-service.test");

    using var provider = services.BuildServiceProvider();
    using var client = provider
        .GetRequiredService<IHttpClientFactory>()
        .CreateClient(nameof(IHeroSnapshotClient));

    var accept = client.DefaultRequestHeaders.Accept.ToArray();

    Assert.Collection(
        accept,
        mediaType =>
        {
          Assert.Equal("application/json", mediaType.MediaType);
          Assert.Empty(mediaType.Parameters);
        });
  }
}
