namespace HeroService.Contracts.DTOs.GameVersions;

public sealed record GameVersionDto(
    Guid Id,
    string VersionName,
    bool IsActive
);