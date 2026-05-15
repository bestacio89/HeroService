using System;

/// <summary>
/// Transporte les choix validés en pré-match vers le runtime de match.
/// En V16, il contient le mode, le héros confirmé et la lane préférée,
/// et sert de passerelle entre HeroSelectionSessionService, MatchBootstrapper et HeroSpawner.
/// </summary>
[Serializable]
public class MatchLaunchContext
{
    public GameModeType SelectedMode = GameModeType.Classic;
    public string SelectedHeroId = string.Empty;
    public LanePreferenceType PreferredLane = LanePreferenceType.None;
}
