using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

public sealed record SkillEffectSnapshot
(
    bool HasDamage,
    bool HasHealing,
    bool HasShield,
    bool HasCrowdControl,
    bool HasMobility,
    bool HasBuff,
    bool HasDebuff,
    bool HasExecute,
    bool HasDamageOverTime,
    bool HasHealOverTime
);