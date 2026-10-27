using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Franz.Common.Mediator.Dispatchers;
using HeroService.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HeroService.Testing.Persistence.Integration;

public sealed class PostgreSqlPersistenceTests
{
  [Fact]
  [Trait("Category", "Integration")]
  public async Task ApplicationDbContext_CreatesSchemaAgainstRealPostgreSql()
  {
    await using IContainer postgres = new ContainerBuilder()
      .WithImage("postgres:16.10-alpine")
      .WithName($"hero-service-postgres-tests-{Guid.NewGuid():N}")
      .WithEnvironment("POSTGRES_DB", "hero_service_tests")
      .WithEnvironment("POSTGRES_USER", "postgres")
      .WithEnvironment("POSTGRES_PASSWORD", "postgres")
      .WithPortBinding(5432, true)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
      .Build();

    await postgres.StartAsync();

    var port = postgres.GetMappedPublicPort(5432);
    var connectionString =
      $"Host=localhost;Port={port};Database=hero_service_tests;Username=postgres;Password=postgres;Timeout=5;Command Timeout=10";

    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseNpgsql(connectionString)
      .Options;

    await using var dbContext = new ApplicationDbContext(
      options,
      new Mock<IDispatcher>().Object);

    await dbContext.Database.EnsureCreatedAsync();

    Assert.True(await dbContext.Database.CanConnectAsync());
    Assert.Equal(0, await dbContext.Heroes.CountAsync());
  }
}
