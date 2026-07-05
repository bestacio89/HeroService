namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

public sealed class SkillSnapshot
{
  public Guid SkillId { get; }
  public Guid GameVersionId { get; }
  public string Name { get; }

  public SkillExecutionSnapshot Execution { get; }
  public SkillEffectSnapshot Effects { get; }

  public IReadOnlyList<EffectExecutionSnapshot> EffectExecutions { get; }

  public SkillSnapshot(
      Guid skillId,
      string name,
      Guid gameVersionId,
      SkillExecutionSnapshot execution,
      SkillEffectSnapshot effects,
      IReadOnlyList<EffectExecutionSnapshot> effectExecutions)
  {
    SkillId = skillId;
    Name = name;
    GameVersionId = gameVersionId;

    Execution = execution;
    Effects = effects;
    EffectExecutions = effectExecutions;
  }
}