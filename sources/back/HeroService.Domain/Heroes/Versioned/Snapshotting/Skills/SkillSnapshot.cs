namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

public sealed class SkillSnapshot
{
  public Guid SkillId { get; }
  public Guid GameVersionId { get; }

  public SkillExecutionSnapshot Execution { get; }
  public SkillEffectSnapshot Effects { get; }

  public SkillSnapshot(
      Guid skillId,
      Guid gameVersionId,
      SkillExecutionSnapshot execution,
      SkillEffectSnapshot effects)
  {
    SkillId = skillId;
    GameVersionId = gameVersionId;
    Execution = execution;
    Effects = effects;
  }
}