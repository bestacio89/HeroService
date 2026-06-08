namespace HeroService.Contracts.DTOs.Snapshots;

/// <summary>
/// DTO mirror of HeroKitProfile.
/// Exposes the hero's behavioral fingerprint to external consumers
/// (matchmaking, item affinity, frontend).
/// </summary>
public sealed record HeroKitProfileDto(
    int DamageSkillCount,
    int CrowdControlSkillCount,
    int MobilitySkillCount,
    int SustainSkillCount,
    int UtilitySkillCount,

    bool HasSummon,
    bool HasTransformation,
    bool HasExecute,

    bool IsBurstOriented,
    bool IsSustainOriented,
    bool IsControlOriented,
    bool IsMobilityOriented
);