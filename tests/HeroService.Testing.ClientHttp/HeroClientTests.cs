using System.Net;
using HeroService.Client.Http.Clients;

namespace HeroService.Testing.ClientHttp;

public sealed class HeroClientTests
{
  [Fact]
  public async Task BrowseAsync_WithFilters_BuildsExpectedRequestUri()
  {
    var heroClassId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    var mythologyTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    var cultureId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    var archetypeId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    var handler = new RecordingHttpMessageHandler();
    using var httpClient = new HttpClient(handler)
    {
      BaseAddress = new Uri("https://hero-service.test/")
    };
    var sut = new HeroClient(httpClient);

    await sut.BrowseAsync(heroClassId, mythologyTypeId, cultureId, archetypeId);

    Assert.Equal(HttpMethod.Get, handler.RequestMethod);
    Assert.Equal(
      $"/api/v1/heroes?heroClassId={heroClassId}&mythologyTypeId={mythologyTypeId}&cultureId={cultureId}&archetypeId={archetypeId}",
      handler.RequestUri?.PathAndQuery);
  }

  [Fact]
  public async Task BrowseAsync_WithoutFilters_UsesCollectionEndpoint()
  {
    var handler = new RecordingHttpMessageHandler();
    using var httpClient = new HttpClient(handler)
    {
      BaseAddress = new Uri("https://hero-service.test/")
    };
    var sut = new HeroClient(httpClient);

    var result = await sut.BrowseAsync();

    Assert.Empty(result);
    Assert.Equal(HttpMethod.Get, handler.RequestMethod);
    Assert.Equal("/api/v1/heroes", handler.RequestUri?.AbsolutePath);
    Assert.Empty(handler.RequestUri?.Query ?? string.Empty);
  }

  [Fact]
  public async Task GetByNameAsync_EscapesNameInRequestPath()
  {
    var handler = new RecordingHttpMessageHandler(responseBody: "null");
    using var httpClient = new HttpClient(handler)
    {
      BaseAddress = new Uri("https://hero-service.test/")
    };
    var sut = new HeroClient(httpClient);

    var result = await sut.GetByNameAsync("Athena / North");

    Assert.Null(result);
    Assert.Equal("/api/v1/heroes/by-name/Athena%20%2F%20North", handler.RequestUri?.AbsolutePath);
  }

  private sealed class RecordingHttpMessageHandler(string responseBody = "[]") : HttpMessageHandler
  {
    public HttpMethod? RequestMethod { get; private set; }
    public Uri? RequestUri { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      RequestMethod = request.Method;
      RequestUri = request.RequestUri;

      return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new StringContent(responseBody, System.Text.Encoding.UTF8, "application/json")
      });
    }
  }
}
