using System;

/// <summary>
/// Représente l'état canonique du profil joueur local pour le Home/Lobby.
/// Cette donnée est la source de vérité affichée par la barre de statut,
/// le panneau Profile et tout élément UI ayant besoin du niveau/monnaie du joueur.
///
/// Règle de priorité (même convention que HeroSelectionSessionData) :
/// - HasBackendData = false → profil MVP local (valeurs par défaut), aucun backend connecté.
/// - HasBackendData = true  → profil chargé depuis le Player/Profile service via PlayerApiClient.
/// </summary>
[Serializable]
public class PlayerProfileData
{
    public string PlayerId = string.Empty;
    public string DisplayName = "Recruit";
    public int Level = 1;
    public int Xp = 0;
    public int XpToNextLevel = 1000;
    public int SoftCurrency = 0;
    public int HardCurrency = 0;
    public string EquippedHeroId = string.Empty;

    /// <summary>
    /// Indique si ces données proviennent réellement du backend.
    /// Quand false, le Home fonctionne en mode legacy avec un profil MVP statique.
    /// </summary>
    public bool HasBackendData = false;

    public float XpProgress01 =>
        XpToNextLevel <= 0 ? 0f : Math.Clamp((float)Xp / XpToNextLevel, 0f, 1f);
}
