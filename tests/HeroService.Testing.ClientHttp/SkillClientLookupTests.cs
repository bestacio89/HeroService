using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using HeroService.Client.Http.Clients;

namespace HeroService.Testing.ClientHttp;

public sealed class SkillClientLookupTests
{
  [Fact]
  public async Task GetLookupsAsync_WithIds_SendsSingleBatchRequest()
  {
    var firstId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    var secondId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    var handler = new RecordingHttpMessageHandler();
    using var httpClient = new HttpClient(handler)
    {
      BaseAddress = new Uri("https://hero-service.test/")
    };
    var sut = new SkillClient(httpClient);

    var result = await sut.GetLookupsAsync([firstId, secondId, firstId]);

    result.Should().BeEmpty();
    handler.RequestCount.Should().Be(1);
    handler.RequestUri!.AbsolutePath.Should().Be("/api/v1/skills/lookup");
    handler.RequestUri.Query.Should().Contain($"ids={firstId}");
    handler.RequestUri.Query.Should().Contain($"ids={secondId}");
    handler.RequestUri.Query.Split("ids=", StringSplitOptions.RemoveEmptyEntries).Should().HaveCount(2);
  }

  [Fact]
  public async Task GetLookupsAsync_WithNoValidIds_DoesNotSendRequest()
  {
    var handler = new RecordingHttpMessageHandler();
    using var httpClient = new HttpClient(handler)
    {
      BaseAddress = new Uri("https://hero-service.test/")
    };
    var sut = new SkillClient(httpClient);

    var result = await sut.GetLookupsAsync([Guid.Empty]);

    result.Should().BeEmpty();
    handler.RequestCount.Should().Be(0);
  }

  private sealed class RecordingHttpMessageHandler : HttpMessageHandler
  {
    public int RequestCount { get; private set; }
    public Uri? RequestUri { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
      RequestCount++;
      RequestUri = request.RequestUri;

      return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new StringContent("[]", Encoding.UTF8, "application/json")
      });
    }
  }
}
