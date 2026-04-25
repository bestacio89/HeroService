namespace HeroService.Domain.Skins;

public class Skin : Entity<Guid>
{
  public Guid HeroId { get; private set; }

  public string Name { get; private set; } = string.Empty;

  public SkinRarity Rarity { get; private set; }

  public string? VisualTheme { get; private set; }

  public string? VfxBundleKey { get; private set; }

  public string? SfxBundleKey { get; private set; }

  private Skin() { }

  public Skin(Guid heroId, string name, SkinRarity rarity)
  {
    HeroId = heroId;
    Name = name;
    Rarity = rarity;

    MarkCreated("system");
  }
}