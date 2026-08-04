using System.Diagnostics;
using Microsoft.AspNetCore.Components;

namespace HeroService.Admin.Components.Pages;

public sealed partial class Error : ComponentBase
{
  [CascadingParameter]
  private HttpContext? HttpContext { get; set; }


  private string? RequestId { get; set; }


  private bool ShowRequestId =>
      !string.IsNullOrWhiteSpace(RequestId);


  protected override void OnInitialized()
  {
    RequestId =
        Activity.Current?.Id ??
        HttpContext?.TraceIdentifier;
  }
}