[System.Serializable]
public class SkillExecutionSnapshotDto
{
	public float cooldown;      // → HeroSkillSlot.CooldownMax
	public float manaCost;      // → HeroSkillSlot.ManaCost
	public float damage;
	public float healing;
	public float shieldValue;
	public float castTime;
	public float channelDuration;
	public float range;
	public float crowdControlDuration;
}