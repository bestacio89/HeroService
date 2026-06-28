using HeroService.Contracts.DTOs.Skills;

public static class SkillUiSeed
{
  public static IReadOnlyCollection<SkillDto> Data { get; } =
      new List<SkillDto>
      {
            new()
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "Mjolnir Strike",
                SkillType = "Damage",
                Effects = new List<SkillEffectDto>
                {
                    new()
                    {
                        EffectType = "Damage",
                        Magnitude = 150,
                        Duration = 0,
                        Radius = 2
                    }
                }
            },

            new()
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Thunder Leap",
                SkillType = "Mobility",
                Effects = new List<SkillEffectDto>()
            }
      };
}