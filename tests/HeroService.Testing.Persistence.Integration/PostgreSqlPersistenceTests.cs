using Franz.Common.Mediator.Dispatchers;
using HeroService.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Testcontainers.PostgreSql;

namespace HeroService.Testing.Persistence.Integration;

public sealed class PostgreSqlPersistenceTests
{
  [Fact]
  [Trait("Category", "Integration")]
  public async Task ApplicationDbContext_CreatesSchemaAgainstRealPostgreSql()
  {
    await using var postgres = new PostgreSqlBuilder("postgres:16.10-alpine")
      .WithDatabase("hero_service_tests")
      .WithUsername("postgres")
      .WithPassword("postgres")
      .Build();

    await postgres.StartAsync();

    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseNpgsql(postgres.GetConnectionString())
      .Options;

    await using var dbContext = new ApplicationDbContext(
      options,
      DispatchProxy.Create<IDispatcher, NoOpDispatcherProxy>());

    await dbContext.Database.EnsureCreatedAsync();

    Assert.True(await dbContext.Database.CanConnectAsync());
    Assert.Equal(0, await dbContext.Heroes.CountAsync());
  }

  public class NoOpDispatcherProxy : DispatchProxy
  {
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
      var returnType = targetMethod?.ReturnType;
      if (returnType is null || returnType == typeof(void))
        return null;

      if (returnType == typeof(Task))
        return Task.CompletedTask;

      if (returnType == typeof(ValueTask))
        return ValueTask.CompletedTask;

      if (returnType.IsValueType)
        return Activator.CreateInstance(returnType);

      return null;
    }
  }
}
