namespace HeroService.Domain.Heroes.Versioned.GameVersion;

public class GameVersionState
{
  public Guid ActiveGameVersionId { get; private set; }

  public void SetActive(Guid gameVersionId)
  {
    ActiveGameVersionId = gameVersionId;
  }
}