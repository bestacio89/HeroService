using System;

/// <summary>
/// Représente l'état canonique de la session de sélection pré-match pour le joueur local.
/// Cette donnée conserve le mode choisi, le héros actuellement sélectionné,
/// la lane préférée et l'état de confirmation.
/// </summary>
[Serializable]
public class HeroSelectionSessionData
{
    public GameModeType SelectedMode = GameModeType.Classic;
    public string SelectedHeroId = string.Empty;
    public LanePreferenceType PreferredLane = LanePreferenceType.None;
    public bool IsConfirmed = false;
}
