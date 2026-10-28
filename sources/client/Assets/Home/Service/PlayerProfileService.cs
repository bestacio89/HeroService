using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Owns the profile data consumed by Home/Lobby presenters.
/// Local demo data is used only when no Player service URL is configured;
/// a configured backend failure is exposed as an unavailable profile.
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
    private PlayerProfileData currentProfile;
    private bool isLoading;
    private bool hasLoaded;
    private bool isUsingLocalProfile;
    private string lastLoadError = string.Empty;

    public PlayerProfileData CurrentProfile => currentProfile;
    public bool IsLoading => isLoading;
    public bool HasLoaded => hasLoaded;
    public bool IsUsingLocalProfile => isUsingLocalProfile;
    public string LastLoadError => lastLoadError;

    public event Action OnProfileLoadStarted;
    public event Action<PlayerProfileData> OnProfileUpdated;
    public event Action<string> OnProfileLoadFailed;

    private void Awake()
    {
        if (!string.IsNullOrWhiteSpace(playerServiceBaseUrl))
        {
            apiClient = new PlayerApiClient(playerServiceBaseUrl);
        }

        if (apiClient == null)
        {
            currentProfile = BuildDefaultProfile();
            isUsingLocalProfile = true;
        }
    }

    private void Start()
    {
        _ = LoadProfileAsync();
    }

    public async Task LoadProfileAsync()
    {
        if (isLoading)
        {
            return;
        }

        isLoading = true;
        lastLoadError = string.Empty;
        OnProfileLoadStarted?.Invoke();

        if (apiClient == null)
        {
            currentProfile = BuildDefaultProfile();
            isUsingLocalProfile = true;
            hasLoaded = true;
            isLoading = false;
            Debug.Log("[PlayerProfileService] Player service URL is not configured; using the explicit local demo profile.");
            OnProfileUpdated?.Invoke(currentProfile);
            return;
        }

        isUsingLocalProfile = false;

        try
        {
            PlayerProfileDto dto = await apiClient.GetProfileAsync(localPlayerId);
            if (dto == null)
            {
                throw new InvalidOperationException("Player service returned an empty profile response.");
            }

            currentProfile = MapFromDto(dto);
            hasLoaded = true;
            Debug.Log($"[PlayerProfileService] Profile loaded from backend: {currentProfile.DisplayName}");
        }
        catch (Exception ex)
        {
            currentProfile = null;
            hasLoaded = false;
            lastLoadError = ex.Message;
            Debug.LogError($"[PlayerProfileService] Could not load the configured Player service profile: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }

        if (currentProfile != null)
        {
            OnProfileUpdated?.Invoke(currentProfile);
        }
        else
        {
            OnProfileUpdated?.Invoke(null);
            OnProfileLoadFailed?.Invoke(lastLoadError);
        }
    }

    /// <summary>
    /// Applique un gain de monnaie locale en attendant la persistance backend.
    /// MVP V1 uniquement — sera remplacé par un appel serveur autoritaire.
    /// </summary>
    public void ApplyLocalCurrencyDelta(int softDelta, int hardDelta)
    {
        if (currentProfile == null || currentProfile.HasBackendData || !isUsingLocalProfile)
        {
            Debug.LogWarning("[PlayerProfileService] Local currency deltas are only available for the local demo profile.");
            return;
        }

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
        // Local-only demo data. Never use this to hide a configured backend failure.
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
