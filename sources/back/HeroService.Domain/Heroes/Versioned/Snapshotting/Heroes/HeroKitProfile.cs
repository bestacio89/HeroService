using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Domain.Heroes.Versioned.Snapshotting;


public sealed record HeroKitProfile(
    int DamageSkillCount,
    int CrowdControlSkillCount,
    int MobilitySkillCount,
    int SustainSkillCount,      // Heal + HealOverTime + Shield
    int UtilitySkillCount,      // Utility + Vision + ZoneControl
    bool HasSummon,
    bool HasTransformation,
    bool HasExecute,
    // Derived behavioral tags — computed, not stored:
    bool IsBurstOriented,       // High damage, low DoT
    bool IsSustainOriented,     // High heal/shield ratio
    bool IsControlOriented,     // High CC count
    bool IsMobilityOriented     // High mobility count
);