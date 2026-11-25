using Bunit;
using HeroService.Admin.Components.Pages.Admin.Heroes;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.DTOs.Skills;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Services;

namespace HeroService.Testing.Admin;

public sealed class HeroCreationTests
{
  [Fact]
  public void CreatePage_RejectsMissingDomainReferencesBeforeCallingApi()
  {
    using var context = CreateContext();
    var heroClient = new Mock<IHeroClient>();
    context.Services.AddSingleton(heroClient.Object);

    var cut = context.RenderComponent<Create>();

    cut.FindAll("button")
      .Single(button => button.TextContent.Contains("Create hero", StringComparison.Ordinal))
      .Click();

    cut.WaitForAssertion(() =>
    {
      Assert.Contains("A hero name is required.", cut.Markup);
      Assert.Contains("Select a hero class.", cut.Markup);
      Assert.Contains("Select a mythology.", cut.Markup);
      Assert.Contains("Select an origin culture.", cut.Markup);
      Assert.Contains("Select an origin archetype.", cut.Markup);
      Assert.Contains("Assign a valid skill to all five skill-kit slots.", cut.Markup);
    });

    heroClient.Verify(
      client => client.CreateAsync(
        It.IsAny<HeroService.Contracts.DTOs.Requests.HeroCreateRequestDto>(),
        It.IsAny<CancellationToken>()),
      Times.Never);
  }

  private static TestContext CreateContext()
  {
    var context = new TestContext();
    context.JSInterop.Mode = JSRuntimeMode.Loose;
    context.Services.AddMudServices();
    context.Services.AddLogging();

    var heroClassClient = new Mock<IHeroClassClient>();
    heroClassClient
      .Setup(client => client.GetAllAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync((IReadOnlyCollection<HeroClassDto>)Array.Empty<HeroClassDto>());

    var mythologyClient = new Mock<IMythologyTypeClient>();
    mythologyClient
      .Setup(client => client.GetAllAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync((IReadOnlyList<MythologyTypeDto>)Array.Empty<MythologyTypeDto>());

    var cultureClient = new Mock<IOriginCultureClient>();
    cultureClient
      .Setup(client => client.GetAllAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync((IReadOnlyList<OriginCultureDto>)Array.Empty<OriginCultureDto>());

    var archetypeClient = new Mock<IOriginArchetypeClient>();
    archetypeClient
      .Setup(client => client.GetAllAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync((IReadOnlyList<OriginArchetypeDto>)Array.Empty<OriginArchetypeDto>());

    var skillClient = new Mock<ISkillClient>();
    skillClient
      .Setup(client => client.GetAllAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync((IReadOnlyCollection<SkillDto>)Array.Empty<SkillDto>());

    context.Services.AddSingleton(heroClassClient.Object);
    context.Services.AddSingleton(mythologyClient.Object);
    context.Services.AddSingleton(cultureClient.Object);
    context.Services.AddSingleton(archetypeClient.Object);
    context.Services.AddSingleton(skillClient.Object);

    return context;
  }
}
