using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Source de vérité unique pour la liste des héros côté Home/Lobby.
/// Évite de dupliquer l'appel GET /api/v1/heroes entre le carrousel du Home
/// et l'écran Hero Collection : les deux s'abonnent à ce service.
///
/// Behavior:
/// - Loads the catalogue once at startup and caches it in memory.
/// - Uses local demo data only when no HeroService URL is configured.
/// - Preserves an empty/error state when a configured backend fails, rather
///   than silently presenting demo heroes as authoritative backend data.
/// </summary>
public class HeroCatalogService : MonoBehaviour
{
    [Header("Backend")]
    [Tooltip("Base URL du HeroService. Laisser vide pour rester en mode MVP local.")]
    [SerializeField] private string heroServiceBaseUrl = string.Empty;

    private HeroApiClient apiClient;
    private HeroListItemDto[] cachedHeroes = Array.Empty<HeroListItemDto>();
    private bool hasLoaded;
    private bool isLoading;
    private bool isUsingFallbackCatalog;
    private string lastLoadError = string.Empty;

    public IReadOnlyList<HeroListItemDto> CachedHeroes => cachedHeroes;
    public bool HasLoaded => hasLoaded;
    public bool IsLoading => isLoading;
    public bool IsUsingFallbackCatalog => isUsingFallbackCatalog;
    public string LastLoadError => lastLoadError;

    public event Action OnCatalogLoadStarted;
    public event Action<HeroListItemDto[]> OnCatalogLoaded;
    public event Action<string> OnCatalogLoadFailed;

    private void Awake()
    {
        if (!string.IsNullOrWhiteSpace(heroServiceBaseUrl))
        {
            apiClient = new HeroApiClient(heroServiceBaseUrl);
        }
    }

    private void Start()
    {
        _ = LoadCatalogAsync();
    }

    public async Task LoadCatalogAsync()
    {
        if (isLoading)
        {
            return;
        }

        isLoading = true;
        lastLoadError = string.Empty;
        OnCatalogLoadStarted?.Invoke();

        if (apiClient == null)
        {
            cachedHeroes = BuildFallbackCatalog();
            hasLoaded = true;
            isUsingFallbackCatalog = true;
            isLoading = false;
            Debug.Log("[HeroCatalogService] HeroService URL is not configured; using the explicit local demo catalogue.");
            OnCatalogLoaded?.Invoke(cachedHeroes);
            return;
        }

        isUsingFallbackCatalog = false;

        try
        {
            HeroListItemDto[] loadedHeroes = await apiClient.GetHeroesAsync();
            cachedHeroes = ValidateCatalog(loadedHeroes);
            hasLoaded = true;

            if (cachedHeroes.Length == 0)
            {
                Debug.LogWarning("[HeroCatalogService] HeroService returned an empty catalogue.");
            }
        }
        catch (Exception ex)
        {
            cachedHeroes = Array.Empty<HeroListItemDto>();
            hasLoaded = false;
            lastLoadError = ex.Message;
            Debug.LogError($"[HeroCatalogService] Could not load the configured HeroService catalogue: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }

        if (hasLoaded)
        {
            OnCatalogLoaded?.Invoke(cachedHeroes);
        }
        else
        {
            OnCatalogLoadFailed?.Invoke(lastLoadError);
        }
    }

    private static HeroListItemDto[] ValidateCatalog(HeroListItemDto[] heroes)
    {
        if (heroes == null)
        {
            throw new InvalidOperationException("HeroService returned a null catalogue.");
        }

        for (int i = 0; i < heroes.Length; i++)
        {
            HeroListItemDto hero = heroes[i];
            if (hero == null ||
                string.IsNullOrWhiteSpace(hero.id) ||
                string.IsNullOrWhiteSpace(hero.name))
            {
                throw new InvalidOperationException(
                    $"HeroService returned an invalid hero catalogue item at index {i}.");
            }
        }

        return heroes;
    }

    private static HeroListItemDto[] BuildFallbackCatalog()
    {
        // Explicit local-only demo data for front-end development. This is
        // never used to hide a failure from a configured HeroService.
        return new[]
        {
            new HeroListItemDto
            {
                id = "hero_warrior",
                name = "Warrior",
                heroClass = new HeroClassDto { id = "class_fighter", name = "Fighter" },
                affiliation = new HeroAffiliationDto
                {
                    archetypeName = "Champion",
                    mythologyName = "Greek",
                    cultureName = "Hellenic"
                }
            },
            new HeroListItemDto
            {
                id = "hero_mage",
                name = "Mage",
                heroClass = new HeroClassDto { id = "class_mage", name = "Mage" },
                affiliation = new HeroAffiliationDto
                {
                    archetypeName = "Sorcerer",
                    mythologyName = "Norse",
                    cultureName = "Scandinavian"
                }
            },
            new HeroListItemDto
            {
                id = "hero_archer",
                name = "Archer",
                heroClass = new HeroClassDto { id = "class_marksman", name = "Marksman" },
                affiliation = new HeroAffiliationDto
                {
                    archetypeName = "Hunter",
                    mythologyName = "Egyptian",
                    cultureName = "Kemetic"
                }
            },
            new HeroListItemDto
            {
                id = "hero_tank",
                name = "Tank",
                heroClass = new HeroClassDto { id = "class_tank", name = "Tank" },
                affiliation = new HeroAffiliationDto
                {
                    archetypeName = "Guardian",
                    mythologyName = "Mesopotamian",
                    cultureName = "Sumerian"
                }
            },
            new HeroListItemDto
            {
                id = "hero_assassin",
                name = "Assassin",
                heroClass = new HeroClassDto { id = "class_assassin", name = "Assassin" },
                affiliation = new HeroAffiliationDto
                {
                    archetypeName = "Shadow",
                    mythologyName = "Japanese",
                    cultureName = "Shinto"
                }
            }
        };
    }
}
