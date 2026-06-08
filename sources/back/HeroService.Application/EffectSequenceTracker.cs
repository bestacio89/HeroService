using HeroService.Domain.Heroes.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Application.Combat;

public sealed class EffectSequenceTracker
{
  private readonly Queue<(EffectType Type, DateTime Timestamp)> _recentEffects = new();
  private readonly TimeSpan _maxWindow;

  public EffectSequenceTracker(TimeSpan maxWindow)
  {
    _maxWindow = maxWindow;
  }

  public void Register(EffectType effectType)
  {
    _recentEffects.Enqueue((effectType, DateTime.UtcNow));
    Prune();
  }

  // Gungnir: Damage → CrowdControl → Damage within window
  public bool MatchesPattern(EffectType[] pattern, TimeSpan window)
  {
    var recent = GetWithin(window);
    if (recent.Count < pattern.Length) return false;

    // Sliding window match over recent effects
    for (int i = 0; i <= recent.Count - pattern.Length; i++)
    {
      bool match = true;
      for (int j = 0; j < pattern.Length; j++)
      {
        if (recent[i + j] != pattern[j]) { match = false; break; }
      }
      if (match) return true;
    }
    return false;
  }

  // Book of Thoth: distinct EffectTypes used in window
  public int CountDistinctEffectsInWindow(TimeSpan window)
      => GetWithin(window).Distinct().Count();

  // Mjöllnir: N consecutive Damage without interruption
  public bool HasConsecutive(EffectType type, int count)
  {
    int streak = 0;
    foreach (var (t, _) in _recentEffects.Reverse())
    {
      if (t == type) streak++;
      else break;
    }
    return streak >= count;
  }

  // Fenrir Fang: consecutive hits on same target — needs target context
  public bool HasConsecutiveOnTarget(EffectType type, Guid targetId, int count)
      => throw new NotImplementedException("Requires target-aware registration — see note below");

  private List<EffectType> GetWithin(TimeSpan window)
  {
    var cutoff = DateTime.UtcNow - window;
    return _recentEffects
        .Where(e => e.Timestamp >= cutoff)
        .Select(e => e.Type)
        .ToList();
  }

  private void Prune()
  {
    var cutoff = DateTime.UtcNow - _maxWindow;
    while (_recentEffects.Count > 0 && _recentEffects.Peek().Timestamp < cutoff)
      _recentEffects.Dequeue();
  }
}