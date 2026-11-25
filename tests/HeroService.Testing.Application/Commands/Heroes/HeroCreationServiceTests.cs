using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Errors;
using HeroService.Application.Commands.Heroes.Services;
using HeroService.Contracts.DTOs.Requests;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using HeroService.Domain.Heroes.Core;
using Moq;

namespace HeroService.Testing.Application.Commands.Heroes;

public sealed class HeroCreationServiceTests
{
  [Fact]
  public async Task CreateAsync_ResolvesCultureUsingOriginCultureId()
  {
    var mythologyId = Guid.NewGuid();
    var heroClassId = Guid.NewGuid();
    var archetypeId = Guid.NewGuid();
    var cultureId = Guid.NewGuid();

    var mythology = new MythologyType("Norse", "test");
    var heroClass = new Mock<HeroClass>(heroClassId).Object;
    var archetype = new OriginArchetype("Aesir", "test");

    var mythologyRepository = new Mock<IEntityRepository<MythologyType, Guid>>();
    mythologyRepository
      .Setup(repository => repository.GetByIdAsync(mythologyId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(mythology);

    var heroClassRepository = new Mock<IEntityRepository<HeroClass, Guid>>();
    heroClassRepository
      .Setup(repository => repository.GetByIdAsync(heroClassId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(heroClass);

    var archetypeRepository = new Mock<IEntityRepository<OriginArchetype, Guid>>();
    archetypeRepository
      .Setup(repository => repository.GetByIdAsync(archetypeId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(archetype);

    var cultureRepository = new Mock<IEntityRepository<OriginCulture, Guid>>();
    cultureRepository
      .Setup(repository => repository.GetByIdAsync(cultureId, It.IsAny<CancellationToken>()))
      .ReturnsAsync((OriginCulture)null!);

    var uniquenessValidator = new Mock<IHeroUniquenessValidator>();
    uniquenessValidator
      .Setup(validator => validator.EnsureUniqueHeroNameAsync("Test Hero", It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    var service = new HeroCreationService(
      new Mock<IEntityFactory<Guid, Hero>>().Object,
      new Mock<IEntityFactory<Guid, HeroBaseStats>>().Object,
      new Mock<IEntityFactory<Guid, HeroLore>>().Object,
      new Mock<IEntityRepository<Hero, Guid>>().Object,
      new Mock<IEntityRepository<HeroBaseStats, Guid>>().Object,
      new Mock<IEntityRepository<HeroLore, Guid>>().Object,
      mythologyRepository.Object,
      heroClassRepository.Object,
      archetypeRepository.Object,
      cultureRepository.Object,
      new Mock<ISkillRepository>().Object,
      uniquenessValidator.Object);

    var request = new HeroCreateRequestDto
    {
      Name = "Test Hero",
      HeroClassId = heroClassId,
      MythologyTypeId = mythologyId,
      OriginArchetypeId = archetypeId,
      OriginCultureId = cultureId
    };

    await Assert.ThrowsAsync<BusinessException>(
      () => service.CreateAsync(request, CancellationToken.None));

    cultureRepository.Verify(
      repository => repository.GetByIdAsync(cultureId, It.IsAny<CancellationToken>()),
      Times.Once);

    cultureRepository.Verify(
      repository => repository.GetByIdAsync(archetypeId, It.IsAny<CancellationToken>()),
      Times.Never);
  }
}
