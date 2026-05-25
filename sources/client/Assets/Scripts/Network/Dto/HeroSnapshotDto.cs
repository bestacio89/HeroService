[System.Serializable]
public class HeroSnapshotDto
{
  public string heroId;
  public string gameVersionId;
  public HeroStatSnapshotDto stats;
  public HeroSkillKitSnapshotDto skills;
}