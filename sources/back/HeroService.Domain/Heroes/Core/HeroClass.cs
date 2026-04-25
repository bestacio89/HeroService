namespace HeroService.Domain.Heroes.Core;

public class HeroClass : Entity<Guid>
{
  public string Name { get; private set; } = string.Empty;

  private HeroClass() { }

  public HeroClass(string name, string createdBy)
  {
    Name = name;
    MarkCreated(createdBy);
  }
}