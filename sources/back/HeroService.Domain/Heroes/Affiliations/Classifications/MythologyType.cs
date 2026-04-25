namespace HeroService.Domain.Heroes.Affiliations.Classifications;

public class MythologyType : Entity<Guid>
{
  public string Name { get; private set; } = string.Empty;

  private MythologyType() { }

  public MythologyType(string name, string createdBy)
  {
    Name = name;
    MarkCreated(createdBy);
  }
}