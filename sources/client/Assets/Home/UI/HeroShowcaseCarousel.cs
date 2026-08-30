using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Carrousel de héros affiché sur le Home, à la manière du splash rotatif
/// de Mobile Legends. Fait défiler automatiquement les héros du catalogue,
/// avec navigation manuelle (flèches) et affichage du tag mythologique
/// (mythologyName / cultureName) plutôt qu'un simple nom de classe générique
/// — c'est le fil rouge "mythologie d'abord" du jeu.
/// </summary>
public class HeroShowcaseCarousel : MonoBehaviour
{
    [Header("Services")]
    [SerializeField] private HeroCatalogService heroCatalogService;
    [SerializeField] private HeroPortraitLibrary portraitLibrary;

    [Header("Display")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text heroNameText;
    [SerializeField] private TMP_Text pantheonTagText;

    [Header("Navigation")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    [Header("Auto Rotation")]
    [SerializeField] private bool autoRotate = true;
    [SerializeField] private float autoRotateIntervalSeconds = 5f;

    private HeroListItemDto[] heroes = System.Array.Empty<HeroListItemDto>();
    private int currentIndex;
    private float autoRotateTimer;

    private void Awake()
    {
        if (previousButton != null)
        {
            previousButton.onClick.AddListener(ShowPrevious);
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(ShowNext);
        }
    }

    private void OnEnable()
    {
        if (heroCatalogService == null)
        {
            Debug.LogWarning("[HeroShowcaseCarousel] HeroCatalogService is missing.");
            return;
        }

        heroCatalogService.OnCatalogLoaded += HandleCatalogLoaded;

        if (heroCatalogService.HasLoaded)
        {
            HandleCatalogLoaded((HeroListItemDto[])System.Linq.Enumerable.ToArray(heroCatalogService.CachedHeroes));
        }

        autoRotateTimer = autoRotateIntervalSeconds;
    }

    private void OnDisable()
    {
        if (heroCatalogService != null)
        {
            heroCatalogService.OnCatalogLoaded -= HandleCatalogLoaded;
        }
    }

    private void OnDestroy()
    {
        if (previousButton != null)
        {
            previousButton.onClick.RemoveListener(ShowPrevious);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(ShowNext);
        }
    }

    private void Update()
    {
        if (!autoRotate || heroes.Length <= 1)
        {
            return;
        }

        autoRotateTimer -= Time.unscaledDeltaTime;

        if (autoRotateTimer <= 0f)
        {
            ShowNext();
            autoRotateTimer = autoRotateIntervalSeconds;
        }
    }

    private void HandleCatalogLoaded(HeroListItemDto[] loadedHeroes)
    {
        heroes = loadedHeroes ?? System.Array.Empty<HeroListItemDto>();
        currentIndex = 0;
        RefreshDisplay();
    }

    private void ShowPrevious()
    {
        if (heroes.Length == 0)
        {
            return;
        }

        currentIndex = (currentIndex - 1 + heroes.Length) % heroes.Length;
        RefreshDisplay();
        autoRotateTimer = autoRotateIntervalSeconds;
    }

    private void ShowNext()
    {
        if (heroes.Length == 0)
        {
            return;
        }

        currentIndex = (currentIndex + 1) % heroes.Length;
        RefreshDisplay();
        autoRotateTimer = autoRotateIntervalSeconds;
    }

    private void RefreshDisplay()
    {
        bool hasHero = heroes.Length > 0;

        if (previousButton != null)
        {
            previousButton.interactable = heroes.Length > 1;
        }

        if (nextButton != null)
        {
            nextButton.interactable = heroes.Length > 1;
        }

        if (!hasHero)
        {
            if (heroNameText != null) heroNameText.text = string.Empty;
            if (pantheonTagText != null) pantheonTagText.text = string.Empty;
            if (portraitImage != null) portraitImage.sprite = null;
            return;
        }

        HeroListItemDto current = heroes[currentIndex];

        if (heroNameText != null)
        {
            heroNameText.text = current.name;
        }

        if (pantheonTagText != null)
        {
            pantheonTagText.text = BuildPantheonTag(current);
        }

        if (portraitImage != null && portraitLibrary != null)
        {
            Sprite portrait = portraitLibrary.GetPortrait(current.id);

            if (portrait != null)
            {
                portraitImage.sprite = portrait;
            }
        }
    }

    private static string BuildPantheonTag(HeroListItemDto hero)
    {
        if (hero.affiliation == null)
        {
            return string.Empty;
        }

        return string.IsNullOrWhiteSpace(hero.affiliation.cultureName)
            ? hero.affiliation.mythologyName
            : $"{hero.affiliation.mythologyName} · {hero.affiliation.cultureName}";
    }
}
