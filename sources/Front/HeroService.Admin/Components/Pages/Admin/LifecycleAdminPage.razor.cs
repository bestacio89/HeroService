using Microsoft.AspNetCore.Components;

namespace HeroService.Admin.Components.Shared;

public sealed partial class LifecycleAdminPage<TDto> : ComponentBase
{
  [Parameter] public string Title { get; set; } = string.Empty;
  [Parameter] public string? Description { get; set; }
  [Parameter] public IReadOnlyList<TDto> Items { get; set; } = [];
  [Parameter] public bool IsLoading { get; set; }
  [Parameter] public string? ErrorMessage { get; set; }
  [Parameter] public RenderFragment? HeaderContent { get; set; }
  [Parameter] public RenderFragment<TDto>? RowTemplate { get; set; }
  [Parameter] public Func<Task>? OnCreate { get; set; }
  [Parameter] public Func<TDto, Task>? OnRename { get; set; }
  [Parameter] public Func<TDto, Task>? OnRetire { get; set; }
  [Parameter] public Func<Task>? OnRefresh { get; set; }

  private bool _isActionInProgress;

  private bool CanRunActions => !IsLoading && !_isActionInProgress;

  private async Task Refresh()
  {
    if (OnRefresh is null || !CanRunActions)
      return;

    await ExecuteActionAsync(OnRefresh);
  }

  private async Task OpenCreate()
  {
    if (OnCreate is null || !CanRunActions)
      return;

    await ExecuteActionAsync(OnCreate);
  }

  private async Task OpenRename(TDto item)
  {
    var callback = OnRename;
    if (callback is null || !CanRunActions)
      return;

    await ExecuteActionAsync(() => callback(item));
  }

  private async Task Retire(TDto item)
  {
    var callback = OnRetire;
    if (callback is null || !CanRunActions)
      return;

    await ExecuteActionAsync(() => callback(item));
  }

  private async Task ExecuteActionAsync(Func<Task> action)
  {
    _isActionInProgress = true;

    try
    {
      await action();
    }
    finally
    {
      _isActionInProgress = false;
    }
  }
}
