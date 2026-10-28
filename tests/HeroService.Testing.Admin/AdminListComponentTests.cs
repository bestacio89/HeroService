using Bunit;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.DTOs.Skills;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Services;
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

    Assert.Contains("Mythology types could not be loaded", cut.Markup);
    Assert.Contains("Try again", cut.Markup);

    cut.FindAll("button").First(button => button.TextContent.Contains("Try again", StringComparison.Ordinal))
      .Click();

    cut.WaitForAssertion(() =>
    {
      Assert.Contains("No mythology types yet", cut.Markup);
      Assert.DoesNotContain("Mythology types could not be loaded", cut.Markup);
    });

    client.Verify(api => api.GetAllAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
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
