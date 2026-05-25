[System.Serializable]
public class HeroSkillKitSnapshotDto
{
  public SkillSnapshotDto passive;
  public SkillSnapshotDto primary;   // → SlotSkillOne
  public SkillSnapshotDto secondary; // → SlotSkillTwo
  public SkillSnapshotDto tertiary;  // → (futur slot)
  public SkillSnapshotDto ultimate;  // → SlotUltimate
}


