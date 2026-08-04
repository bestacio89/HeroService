using Microsoft.AspNetCore.Components;

namespace HeroService.Admin.Components.Shared;

public sealed partial class LifecycleAdminPage<TDto>
    : ComponentBase
{
  [Parameter]
  public string Title { get; set; } = string.Empty;


  [Parameter]
  public IReadOnlyList<TDto> Items { get; set; } = [];


  [Parameter]
  public bool IsLoading { get; set; }


  [Parameter]
  public RenderFragment? HeaderContent { get; set; }


  [Parameter]
  public RenderFragment<TDto>? RowTemplate { get; set; }


  [Parameter]
  public Func<Task>? OnCreate { get; set; }


  [Parameter]
  public Func<TDto, Task>? OnRename { get; set; }


  [Parameter]
  public Func<TDto, Task>? OnRetire { get; set; }


  [Parameter]
  public Func<Task>? OnRefresh { get; set; }


  private async Task OpenCreate()
  {
    if (OnCreate is not null)
    {
      await OnCreate();
    }
  }


  private async Task OpenRename(TDto item)
  {
    if (OnRename is not null)
    {
      await OnRename(item);
    }
  }


  private async Task Retire(TDto item)
  {
    if (OnRetire is not null)
    {
      await OnRetire(item);
    }
  }
}