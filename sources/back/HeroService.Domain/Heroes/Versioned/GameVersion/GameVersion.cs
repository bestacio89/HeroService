namespace HeroService.Domain.Heroes.Versioned.GameVersion;

public class GameVersion : Entity<Guid>
{
  public string VersionName { get; private set; }
  public bool IsActive { get; private set; }  
  private GameVersion() { }

  public GameVersion(string versionName, string createdBy)
  {
    VersionName = versionName;
    MarkCreated(createdBy);
  }
}