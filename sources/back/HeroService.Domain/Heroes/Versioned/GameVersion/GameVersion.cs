public class GameVersion : Entity<Guid>
{
  public string VersionNumber { get; private set; }
  public string VersionName { get; private set; }
  public bool IsActive { get; private set; }

  private GameVersion() { }

  public void Define(string versionNumber, string versionName, string createdBy)
  {
    if (!string.IsNullOrWhiteSpace(VersionNumber))
      throw new InvalidOperationException("GameVersion is already defined.");

    if (string.IsNullOrWhiteSpace(versionNumber))
      throw new ArgumentException("VersionNumber cannot be empty.", nameof(versionNumber));

    if (string.IsNullOrWhiteSpace(versionName))
      throw new ArgumentException("VersionName cannot be empty.", nameof(versionName));

    VersionNumber = versionNumber;
    VersionName = versionName;

    IsActive = false;

    MarkCreated(createdBy);
  }

  public void Activate(string updatedBy)
  {
    if (IsActive)
      return;

    IsActive = true;
    MarkUpdated(updatedBy);
  }

  public void Deactivate(string updatedBy)
  {
    if (!IsActive)
      return;

    IsActive = false;
    MarkUpdated(updatedBy);
  }
}