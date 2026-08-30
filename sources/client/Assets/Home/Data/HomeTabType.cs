/// <summary>
/// Définit les onglets disponibles dans la navigation principale du Home/Lobby.
/// Utilisé par HomeNavigationController pour savoir quel panneau afficher
/// et par NavTabButtonView pour identifier le bouton cliqué.
/// </summary>
public enum HomeTabType
{
    Home = 0,
    Heroes = 1,
    Shop = 2,
    Profile = 3
}
