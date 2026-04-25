using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Domain.Heroes.Core;

public sealed class HeroLore : Entity<Guid>
{
  public Guid HeroId { get; private set; }

  public string Title { get; private set; }
  public string Description { get; private set; }   // mythological identity
  public string BackgroundStory { get; private set; }

  private HeroLore() { }

  public HeroLore(Guid heroId, string title, string description, string background)
  {
    HeroId = heroId;
    Title = title;
    Description = description;
    BackgroundStory = background;

    MarkCreated("system");
  }
}