using System;

namespace HeroService.Domain.Heroes.Versioned.GameVersion;

public class GameVersion : Entity<Guid>
{
  public string VersionName { get; private set; }
  public bool IsActive { get; private set; }

  private GameVersion() { }

  public GameVersion(string versionName, string createdBy)
  {
    if (string.IsNullOrWhiteSpace(versionName))
      throw new ArgumentException("VersionName cannot be empty.", nameof(versionName));

    VersionName = versionName;
    IsActive = false;

    MarkCreated(createdBy);
  }

  // =========================================
  // DOMAIN BEHAVIOR: ACTIVATE VERSION
  // =========================================
  public void Activate(string updatedBy)
  {
    if (IsActive)
      return;

    IsActive = true;

    MarkUpdated(updatedBy);
  }

  // =========================================
  // DOMAIN BEHAVIOR: DEACTIVATE VERSION
  // =========================================
  public void Deactivate(string updatedBy)
  {
    if (!IsActive)
      return;

    IsActive = false;

    MarkUpdated(updatedBy);
  }
}