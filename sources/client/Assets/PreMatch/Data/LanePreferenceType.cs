/// <summary>
/// Définit les préférences de lane disponibles dans la session pré-match.
/// En V16, cette donnée est déjà transportée par la session et par le MatchLaunchContext,
/// même si l'UI de choix de lane n'est pas encore branchée côté gameplay.
/// </summary>
public enum LanePreferenceType
{
    None = 0,
    Roam = 1,
    ExpLane = 2,
    Jungle = 3,
    MidLane = 4,
    GoldLane = 5
}
