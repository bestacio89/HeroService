using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Snapshots;

public sealed record SkillEffectSnapshotDto(
    bool HasDamage,
    bool HasHealing,
    bool HasShield,
    bool HasCrowdControl,
    bool HasMobility,
    bool HasBuff,
    bool HasDebuff,
    bool HasExecute,
    bool HasDamageOverTime
);