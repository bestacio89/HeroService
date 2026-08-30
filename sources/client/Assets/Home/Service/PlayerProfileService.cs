using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Sert de source de vérité du profil joueur local pour tout le Home/Lobby.
/// Centralise le chargement (backend ou fallback MVP local) et notifie
/// les presenters abonnés (barre de statut, panneau Profile, etc.).
///
/// Ce service ne contient AUCUNE logique gameplay ni logique d'écran :
/// il expose uniquement l'état du profil et le moyen de le rafraîchir.
/// </summary>
public class PlayerProfileService : MonoBehaviour
{
    [Header("Backend")]
    [Tooltip("Base URL du Player/Profile service. Laisser vide pour rester en mode MVP local.")]
    [SerializeField] private string playerServiceBaseUrl = string.Empty;

    [Tooltip("Identifiant du joueur local. MVP V1 : valeur statique le temps de brancher l'auth.")]
    [SerializeField] private string localPlayerId = "player_local_dev";

    private PlayerApiClient apiClient;
    private PlayerProfileData currentProfile = new();

    public PlayerProfileData CurrentProfile => currentProfile;

    public event Action<PlayerProfileData> OnProfileUpdated;

    private void Awake()
    {
        currentProfile = BuildDefaultProfile();

        if (!string.IsNullOrWhiteSpace(playerServiceBaseUrl))
        {
            apiClient = new PlayerApiClient(playerServiceBaseUrl);
        }
    }

    private async void Start()
    {
        await LoadProfileAsync();
    }

    public async Task LoadProfileAsync()
    {
        if (apiClient == null)
        {
            Debug.Log("[PlayerProfileService] Aucune base URL configurée — profil MVP local utilisé.");
            OnProfileUpdated?.Invoke(currentProfile);
            return;
        }

        try
        {
            PlayerProfileDto dto = await apiClient.GetProfileAsync(localPlayerId);
            currentProfile = MapFromDto(dto);
            Debug.Log($"[PlayerProfileService] Profil chargé depuis le backend : {currentProfile.DisplayName}");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[PlayerProfileService] Échec du chargement backend, fallback MVP local. {ex.Message}");
            currentProfile = BuildDefaultProfile();
        }

        OnProfileUpdated?.Invoke(currentProfile);
    }

    /// <summary>
    /// Applique un gain de monnaie locale en attendant la persistance backend.
    /// MVP V1 uniquement — sera remplacé par un appel serveur autoritaire.
    /// </summary>
    public void ApplyLocalCurrencyDelta(int softDelta, int hardDelta)
    {
        currentProfile.SoftCurrency = Math.Max(0, currentProfile.SoftCurrency + softDelta);
        currentProfile.HardCurrency = Math.Max(0, currentProfile.HardCurrency + hardDelta);
        OnProfileUpdated?.Invoke(currentProfile);
    }

    private static PlayerProfileData MapFromDto(PlayerProfileDto dto)
    {
        return new PlayerProfileData
        {
            PlayerId = dto.id,
            DisplayName = dto.displayName,
            Level = dto.level,
            Xp = dto.xp,
            XpToNextLevel = dto.xpToNextLevel,
            SoftCurrency = dto.softCurrency,
            HardCurrency = dto.hardCurrency,
            EquippedHeroId = dto.equippedHeroId,
            HasBackendData = true
        };
    }

    private static PlayerProfileData BuildDefaultProfile()
    {
        // MVP V1 : profil statique, aligné sur la même logique que
        // la liste de héros statique dans HeroSelectionPanelPresenter.
        return new PlayerProfileData
        {
            PlayerId = "local",
            DisplayName = "Recruit",
            Level = 1,
            Xp = 0,
            XpToNextLevel = 1000,
            SoftCurrency = 500,
            HardCurrency = 50,
            EquippedHeroId = string.Empty,
            HasBackendData = false
        };
    }
}
