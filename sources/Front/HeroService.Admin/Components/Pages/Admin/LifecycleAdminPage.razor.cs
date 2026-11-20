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
  [Parameter] public EventCallback OnCreate { get; set; }
  [Parameter] public EventCallback<TDto> OnRename { get; set; }
  [Parameter] public EventCallback<TDto> OnRetire { get; set; }
  [Parameter] public EventCallback OnRefresh { get; set; }

  private bool _isActionInProgress;

  private bool CanRunActions => !IsLoading && !_isActionInProgress;

  private async Task Refresh()
  {
    if (!OnRefresh.HasDelegate || !CanRunActions)
      return;

    await ExecuteActionAsync(() => OnRefresh.InvokeAsync());
  }

  private async Task OpenCreate()
  {
    if (!OnCreate.HasDelegate || !CanRunActions)
      return;

    await ExecuteActionAsync(() => OnCreate.InvokeAsync());
  }

  private async Task OpenRename(TDto item)
  {
    if (!OnRename.HasDelegate || !CanRunActions)
      return;

    await ExecuteActionAsync(() => OnRename.InvokeAsync(item));
  }

  private async Task Retire(TDto item)
  {
    if (!OnRetire.HasDelegate || !CanRunActions)
      return;

    await ExecuteActionAsync(() => OnRetire.InvokeAsync(item));
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
