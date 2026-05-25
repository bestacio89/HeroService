using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Snapshots;

public sealed record SkillExecutionSnapshotDto(
   float Cooldown,
    float ManaCost,
    float Damage,
    float Healing,
    float ShieldValue,
    float CastTime,
    float ChannelDuration,
    float Range,
    float CrowdControlDuration
);