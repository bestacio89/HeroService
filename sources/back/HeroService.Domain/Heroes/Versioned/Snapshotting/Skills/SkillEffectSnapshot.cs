using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

public sealed record SkillEffectSnapshot
(
    bool HasDamage,
    bool HasHealing,
    bool HasShielding,
    bool HasCrowdControl,
    bool HasMobility,
    bool HasBuff,
    bool HasDebuff,
    bool IsUltimate
);