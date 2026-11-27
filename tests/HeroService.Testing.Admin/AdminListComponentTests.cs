using Bunit;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.DTOs.Skills;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using MudBlazor.Services;
using OriginCulturesPage = HeroService.Admin.Components.Pages.Admin.OriginCultures.OriginCultures;
using SkillsIndex = HeroService.Admin.Components.Pages.Admin.Skills.Index;
using HeroClassesIndex = HeroService.Admin.Components.Pages.Admin.HeroClasses.Index;
using MythologyTypesIndex = HeroService.Admin.Components.Pages.Admin.MythologyTypes.Index;

namespace HeroService.Testing.Admin;

public sealed class AdminListComponentTests
{
  [Fact]
  public void Skills_RendersLoadedItemsAndSummary()
  {
    using var context = CreateContext();
    var skills = new Mock<ISkillClient>();
    skills
      .Setup(client => client.GetAllAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync((IReadOnlyCollection<SkillDto>)
      [
        new SkillDto { Id = Guid.NewGuid(), Name = "Fireball" },
        new SkillDto { Id = Guid.NewGuid(), Name = "Frost Nova" }
      ]);
    context.Services.AddSingleton(skills.Object);

    var heroes = new Mock<IHeroClient>();
    heroes
      .Setup(client => client.BrowseAsync(
        null,
        null,
        null,
        null,
        It.IsAny<CancellationToken>()))
      .ReturnsAsync((IReadOnlyCollection<HeroDto>)Array.Empty<HeroDto>());
    context.Services.AddSingleton(heroes.Object);

    var cut = context.RenderComponent<SkillsIndex>();

    Assert.Contains("Fireball", cut.Markup);
    Assert.Contains("Frost Nova", cut.Markup);
    Assert.Contains("2 results", cut.Markup);
    skills.Verify(client => client.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public void HeroClasses_SearchFiltersDisplayedRows()
  {
    using var context = CreateContext();
    var client = new Mock<IHeroClassClient>();
    client
      .Setup(api => api.GetAllAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync((IReadOnlyCollection<HeroClassDto>)
      [
        new HeroClassDto(Guid.NewGuid(), "Mage"),
        new HeroClassDto(Guid.NewGuid(), "Warrior")
      ]);
    context.Services.AddSingleton(client.Object);

    var cut = context.RenderComponent<HeroClassesIndex>();

    Assert.Contains("Mage", cut.Markup);
    Assert.Contains("Warrior", cut.Markup);

    cut.Find("input").Input("mage");

    cut.WaitForAssertion(() =>
    {
      Assert.Contains("Mage", cut.Markup);
      Assert.DoesNotContain("Warrior", cut.Markup);
      Assert.Contains("1 result", cut.Markup);
    });
  }

  [Fact]
  public void MythologyTypes_ProvidesRetryAfterApiFailure()
  {
    using var context = CreateContext();
    var client = new Mock<IMythologyTypeClient>();
    client
      .SetupSequence(api => api.GetAllAsync(It.IsAny<CancellationToken>()))
      .ThrowsAsync(new HttpRequestException("API unavailable"))
      .ReturnsAsync((IReadOnlyList<MythologyTypeDto>)Array.Empty<MythologyTypeDto>());
    context.Services.AddSingleton(client.Object);

    var cut = context.RenderComponent<MythologyTypesIndex>();

    Assert.Contains("Records could not be loaded", cut.Markup);
    Assert.Contains("Try again", cut.Markup);

    cut.FindAll("button").First(button => button.TextContent.Contains("Try again", StringComparison.Ordinal))
      .Click();

    cut.WaitForAssertion(() =>
    {
      Assert.Contains("No records yet", cut.Markup);
      Assert.DoesNotContain("Records could not be loaded", cut.Markup);
    });

    client.Verify(api => api.GetAllAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
  }

  [Fact]
  public void OriginCultures_DistinguishesLoadFailureAndRetriesSuccessfully()
  {
    using var context = CreateContext();
    var client = new Mock<IOriginCultureClient>();
    client
      .SetupSequence(api => api.GetAllAsync(It.IsAny<CancellationToken>()))
      .ThrowsAsync(new HttpRequestException("API unavailable"))
      .ReturnsAsync((IReadOnlyList<OriginCultureDto>)
      [
        new OriginCultureDto(Guid.NewGuid(), "Norse")
      ]);
    context.Services.AddSingleton(client.Object);

    var cut = context.RenderComponent<OriginCulturesPage>();

    Assert.Contains("Records could not be loaded", cut.Markup);
    Assert.Contains("The HeroService API could not be reached", cut.Markup);
    Assert.DoesNotContain("No records yet", cut.Markup);

    cut.FindAll("button")
      .First(button => button.TextContent.Contains("Try again", StringComparison.Ordinal))
      .Click();

    cut.WaitForAssertion(() =>
    {
      Assert.Contains("Norse", cut.Markup);
      Assert.DoesNotContain("Records could not be loaded", cut.Markup);
      Assert.DoesNotContain("No records yet", cut.Markup);
    });

    client.Verify(api => api.GetAllAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
  }

  [Fact]
  public void OriginCultures_CancelledRetirementDoesNotCallApi()
  {
    using var context = CreateContext();
    var culture = new OriginCultureDto(Guid.NewGuid(), "Norse");
    var client = new Mock<IOriginCultureClient>();
    client
      .Setup(api => api.GetAllAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync((IReadOnlyList<OriginCultureDto>)[culture]);
    context.Services.AddSingleton(client.Object);

    var dialogProvider = context.RenderComponent<MudDialogProvider>();
    var cut = context.RenderComponent<OriginCulturesPage>();

    cut.WaitForAssertion(() => Assert.Contains("Norse", cut.Markup));
    cut.WaitForElement("button[aria-label='Rename record']");
    cut.WaitForElement("button[aria-label='Retire record']").Click();

    dialogProvider.WaitForAssertion(() =>
      Assert.Contains("Retire origin culture?", dialogProvider.Markup));

    dialogProvider.FindAll("button")
      .First(button => button.TextContent.Contains("Cancel", StringComparison.Ordinal))
      .Click();

    client.Verify(
      api => api.RetireAsync(culture.Id, It.IsAny<CancellationToken>()),
      Times.Never);
  }

  [Fact]
  public void OriginCultures_ConfirmedRetirementCallsApiAndRefreshes()
  {
    using var context = CreateContext();
    var culture = new OriginCultureDto(Guid.NewGuid(), "Norse");
    var client = new Mock<IOriginCultureClient>();
    client
      .Setup(api => api.GetAllAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync((IReadOnlyList<OriginCultureDto>)[culture]);
    client
      .Setup(api => api.RetireAsync(culture.Id, It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);
    context.Services.AddSingleton(client.Object);

    var dialogProvider = context.RenderComponent<MudDialogProvider>();
    var cut = context.RenderComponent<OriginCulturesPage>();

    cut.WaitForAssertion(() => Assert.Contains("Norse", cut.Markup));
    cut.WaitForElement("button[aria-label='Rename record']");
    cut.WaitForElement("button[aria-label='Retire record']").Click();

    dialogProvider.WaitForAssertion(() =>
      Assert.Contains("Retire origin culture?", dialogProvider.Markup));

    dialogProvider.FindAll("button")
      .First(button => button.TextContent.Trim() == "Retire")
      .Click();

    cut.WaitForAssertion(() =>
      client.Verify(
        api => api.RetireAsync(culture.Id, It.IsAny<CancellationToken>()),
        Times.Once));

    client.Verify(
      api => api.GetAllAsync(It.IsAny<CancellationToken>()),
      Times.Exactly(2));
  }

  private static TestContext CreateContext()
  {
    var context = new TestContext();
    context.JSInterop.Mode = JSRuntimeMode.Loose;
    context.Services.AddMudServices();
    context.Services.AddLogging();
    return context;
  }
}
