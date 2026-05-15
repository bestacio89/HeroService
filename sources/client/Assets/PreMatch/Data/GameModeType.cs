/// <summary>
/// Définit les modes de jeu disponibles dans le flux pré-match MVP.
/// Cette enum est utilisée par les presenters UI, la session de sélection
/// et le MatchLaunchContext pour transporter le choix de mode jusqu'au runtime.
/// </summary>
public enum GameModeType
{
    Classic = 0,
    Ranked = 1,
    Arcade = 2
}
