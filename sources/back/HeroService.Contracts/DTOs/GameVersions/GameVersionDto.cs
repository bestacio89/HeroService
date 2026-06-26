namespace HeroService.Contracts.DTOs.GameVersions;

public sealed record GameVersionDto(
    Guid Id,
    string VersionNumber,
    string VersionName,
    bool IsActive
);