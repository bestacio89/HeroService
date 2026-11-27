using Bunit;
using HeroService.Admin.Components.Shared;
using HeroService.Contracts.DTOs.Heroes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace HeroService.Testing.Admin;

public sealed class LifecycleAdminPageTests
{
  [Fact]
  public void Search_FiltersRowsWithoutMutatingTheSourceCollection()
  {
    using var context = CreateContext();
    var items = new[]
    {
      new OriginCultureDto(Guid.NewGuid(), "Norse"),
      new OriginCultureDto(Guid.NewGuid(), "Greek")
    };

    var cut = context.RenderComponent<LifecycleAdminPage<OriginCultureDto>>(parameters => parameters
      .Add(component => component.Title, "Origin Cultures")
      .Add(component => component.Items, items)
      .Add(component => component.SearchPlaceholder, "Search origin cultures")
      .Add(component => component.SearchTextSelector, item => item.Name ?? string.Empty)
      .Add<RenderFragment<OriginCultureDto>>(component => component.RowTemplate,
        item => builder => builder.AddContent(0, item.Name)));

    Assert.Contains("Norse", cut.Markup);
    Assert.Contains("Greek", cut.Markup);

    cut.Find("input").Input("nors");

    cut.WaitForAssertion(() =>
    {
      Assert.Contains("Norse", cut.Markup);
      Assert.DoesNotContain("Greek", cut.Markup);
      Assert.Contains("1 of 2 records", cut.Markup);
    });

    Assert.Equal(2, items.Length);
  }

  [Fact]
  public void Search_ShowsDedicatedNoMatchState()
  {
    using var context = CreateContext();
    var items = new[]
    {
      new OriginCultureDto(Guid.NewGuid(), "Norse")
    };

    var cut = context.RenderComponent<LifecycleAdminPage<OriginCultureDto>>(parameters => parameters
      .Add(component => component.Title, "Origin Cultures")
      .Add(component => component.Items, items)
      .Add(component => component.SearchPlaceholder, "Search origin cultures")
      .Add(component => component.SearchTextSelector, item => item.Name ?? string.Empty)
      .Add<RenderFragment<OriginCultureDto>>(component => component.RowTemplate,
        item => builder => builder.AddContent(0, item.Name)));

    cut.Find("input").Input("Atlantis");

    cut.WaitForAssertion(() =>
    {
      Assert.Contains("No matching records", cut.Markup);
      Assert.Contains("Try a different search term", cut.Markup);
      Assert.DoesNotContain("Norse", cut.Markup);
    });
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
