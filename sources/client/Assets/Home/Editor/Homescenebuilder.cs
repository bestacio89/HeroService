using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Outil d'édition qui construit automatiquement la hiérarchie complète de la
/// scène Home (Services, Canvas, navigation, 4 panneaux, barre de navigation)
/// ainsi que les deux prefabs nécessaires à l'écran Hero Collection
/// (HeroCollectionItem, PantheonFilterChip).
///
/// Utilisation :
/// 1. Créer/ouvrir une scène VIDE à Assets/Home/Scenes/Home.unity.
/// 2. Menu MOBA > Home > Build Home Scene And Prefabs.
/// 3. Sauvegarder la scène (Ctrl+S).
///
/// Idempotent : recherche chaque objet par nom avant de le créer, donc
/// relancer l'outil ne duplique rien. En revanche, les valeurs par défaut
/// (texte placeholder, couleurs) sont réappliquées à chaque exécution —
/// si du texte/des couleurs sont personnalisés à la main dans l'Inspector,
/// un nouveau run de l'outil les réinitialise sur ces objets précis.
///
/// Aucune base URL n'est renseignée sur PlayerProfileService / HeroCatalogService :
/// les deux restent en mode MVP local tant que le Player service et le
/// endpoint HeroController ne sont pas exposés côté backend.
/// </summary>
public static class HomeSceneBuilder
{
    private const float NavBarHeightRatio = 0.12f;

    [MenuItem("MOBA/Home/Build Home Scene And Prefabs")]
    public static void BuildHomeScene()
    {
        // 1. Services (hors Canvas — objets logiques persistants)
        GameObject servicesRoot = FindOrCreateRoot("Services");

        GameObject playerServiceGo = FindOrCreateChild(servicesRoot.transform, "PlayerProfileService");
        PlayerProfileService playerProfileService = GetOrAddComponent<PlayerProfileService>(playerServiceGo);

        GameObject heroCatalogGo = FindOrCreateChild(servicesRoot.transform, "HeroCatalogService");
        HeroCatalogService heroCatalogService = GetOrAddComponent<HeroCatalogService>(heroCatalogGo);

        GameObject portraitLibGo = FindOrCreateChild(servicesRoot.transform, "HeroPortraitLibrary");
        HeroPortraitLibrary portraitLibrary = GetOrAddComponent<HeroPortraitLibrary>(portraitLibGo);

        // 2. Canvas + EventSystem
        GameObject canvasGo = FindOrCreateRoot("Canvas");
        Canvas canvas = canvasGo.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920); // portrait mobile
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();
        }

        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<StandaloneInputModule>();
        }

        // 3. MainMenuSceneLoader + overlay de chargement
        GameObject loaderGo = FindOrCreateUIChild(canvasGo.transform, "MainMenuSceneLoader");
        SetAnchors(loaderGo.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
        MainMenuSceneLoader sceneLoader = GetOrAddComponent<MainMenuSceneLoader>(loaderGo);

        GameObject overlayGo = FindOrCreateUIChild(loaderGo.transform, "LoadingOverlay");
        SetAnchors(overlayGo.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
        Image overlayBg = GetOrAddComponent<Image>(overlayGo);
        overlayBg.color = new Color(0f, 0f, 0f, 0.85f);
        overlayGo.SetActive(false);

        Slider progressSlider = AddSliderChild(overlayGo.transform, "ProgressBar", new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.52f));

        SetPrivateField(sceneLoader, "loadingOverlayRoot", overlayGo);
        SetPrivateField(sceneLoader, "loadingProgressBar", progressSlider);

        // 4. Racine de navigation
        GameObject navRootGo = FindOrCreateUIChild(canvasGo.transform, "HomeNavigationController_Root");
        SetAnchors(navRootGo.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
        HomeNavigationController navController = GetOrAddComponent<HomeNavigationController>(navRootGo);

        // 5. Panneaux
        GameObject panelHome = BuildHomePanel(navRootGo.transform, playerProfileService, heroCatalogService, portraitLibrary, sceneLoader);
        GameObject panelHeroes = BuildHeroesPanel(navRootGo.transform, heroCatalogService, portraitLibrary);
        GameObject panelShop = BuildShopPanel(navRootGo.transform);
        GameObject panelProfile = BuildProfilePanel(navRootGo.transform, playerProfileService);

        // 6. Barre de navigation inférieure
        GameObject navBar = BuildBottomNavBar(
            canvasGo.transform,
            out NavTabButtonView btnHome,
            out NavTabButtonView btnHeroes,
            out NavTabButtonView btnShop,
            out NavTabButtonView btnProfile);

        // 7. Câblage du HomeNavigationController
        ConfigureNavigationTabs(navController, new[]
        {
            (HomeTabType.Home, panelHome, btnHome),
            (HomeTabType.Heroes, panelHeroes, btnHeroes),
            (HomeTabType.Shop, panelShop, btnShop),
            (HomeTabType.Profile, panelProfile, btnProfile),
        });

        panelHome.SetActive(true);
        panelHeroes.SetActive(false);
        panelShop.SetActive(false);
        panelProfile.SetActive(false);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorUtility.DisplayDialog(
            "Home Scene Builder",
            "Home scene hierarchy built. Save the scene now (Ctrl+S).",
            "OK");
    }

    // =================================================================
    // Panels
    // =================================================================

    private static GameObject BuildHomePanel(
        Transform navRoot,
        PlayerProfileService playerProfileService,
        HeroCatalogService heroCatalogService,
        HeroPortraitLibrary portraitLibrary,
        MainMenuSceneLoader sceneLoader)
    {
        GameObject panel = FindOrCreateUIChild(navRoot, "Panel_Home");
        AnchorPanelAboveNavBar(panel.GetComponent<RectTransform>());

        HomeLandingPresenter presenter = GetOrAddComponent<HomeLandingPresenter>(panel);

        // --- Status bar ---
        GameObject statusBar = FindOrCreateUIChild(panel.transform, "StatusBar");
        SetAnchors(statusBar.GetComponent<RectTransform>(), new Vector2(0f, 0.9f), Vector2.one);
        HorizontalLayoutGroup statusLayout = GetOrAddComponent<HorizontalLayoutGroup>(statusBar);
        statusLayout.childForceExpandWidth = false;
        statusLayout.childForceExpandHeight = true;
        statusLayout.spacing = 16;
        statusLayout.padding = new RectOffset(16, 16, 8, 8);
        statusLayout.childAlignment = TextAnchor.MiddleLeft;

        TextMeshProUGUI nameText = AddTMPTextChild(statusBar.transform, "PlayerNameText", "Recruit", 26, TextAlignmentOptions.MidlineLeft, Vector2.zero, Vector2.one);
        TextMeshProUGUI levelText = AddTMPTextChild(statusBar.transform, "PlayerLevelText", "Lv. 1", 22, TextAlignmentOptions.MidlineLeft, Vector2.zero, Vector2.one);
        Slider xpBar = AddSliderChild(statusBar.transform, "XpBar", Vector2.zero, Vector2.one);
        LayoutElement xpLayoutElement = GetOrAddComponent<LayoutElement>(xpBar.gameObject);
        xpLayoutElement.preferredWidth = 160;
        xpLayoutElement.preferredHeight = 16;
        TextMeshProUGUI softText = AddTMPTextChild(statusBar.transform, "SoftCurrencyText", "0", 20, TextAlignmentOptions.MidlineLeft, Vector2.zero, Vector2.one);
        TextMeshProUGUI hardText = AddTMPTextChild(statusBar.transform, "HardCurrencyText", "0", 20, TextAlignmentOptions.MidlineLeft, Vector2.zero, Vector2.one);

        // --- Hero showcase ---
        GameObject showcase = FindOrCreateUIChild(panel.transform, "HeroShowcase");
        SetAnchors(showcase.GetComponent<RectTransform>(), new Vector2(0.1f, 0.28f), new Vector2(0.9f, 0.88f));
        HeroShowcaseCarousel carousel = GetOrAddComponent<HeroShowcaseCarousel>(showcase);

        GameObject portraitGo = FindOrCreateUIChild(showcase.transform, "PortraitImage");
        SetAnchors(portraitGo.GetComponent<RectTransform>(), new Vector2(0.15f, 0.15f), new Vector2(0.85f, 0.9f));
        Image portraitImage = GetOrAddComponent<Image>(portraitGo);
        portraitImage.color = new Color(0.15f, 0.15f, 0.18f, 1f);

        TextMeshProUGUI heroNameText = AddTMPTextChild(showcase.transform, "HeroNameText", "Hero", 30, TextAlignmentOptions.Center, new Vector2(0f, 0f), new Vector2(1f, 0.1f));
        TextMeshProUGUI pantheonTagText = AddTMPTextChild(showcase.transform, "PantheonTagText", "Pantheon", 18, TextAlignmentOptions.Center, new Vector2(0f, 0.1f), new Vector2(1f, 0.16f));
        Button prevButton = AddButtonChild(showcase.transform, "PreviousButton", "<", new Vector2(0f, 0.4f), new Vector2(0.12f, 0.6f));
        Button nextButton = AddButtonChild(showcase.transform, "NextButton", ">", new Vector2(0.88f, 0.4f), new Vector2(1f, 0.6f));

        // --- Battle CTA ---
        Button battleButton = AddButtonChild(panel.transform, "BattleButton", "BATTLE", new Vector2(0.25f, 0.02f), new Vector2(0.75f, 0.16f));

        SetPrivateField(presenter, "playerProfileService", playerProfileService);
        SetPrivateField(presenter, "sceneLoader", sceneLoader);
        SetPrivateField(presenter, "playerNameText", nameText);
        SetPrivateField(presenter, "playerLevelText", levelText);
        SetPrivateField(presenter, "xpBar", xpBar);
        SetPrivateField(presenter, "softCurrencyText", softText);
        SetPrivateField(presenter, "hardCurrencyText", hardText);
        SetPrivateField(presenter, "battleButton", battleButton);

        SetPrivateField(carousel, "heroCatalogService", heroCatalogService);
        SetPrivateField(carousel, "portraitLibrary", portraitLibrary);
        SetPrivateField(carousel, "portraitImage", portraitImage);
        SetPrivateField(carousel, "heroNameText", heroNameText);
        SetPrivateField(carousel, "pantheonTagText", pantheonTagText);
        SetPrivateField(carousel, "previousButton", prevButton);
        SetPrivateField(carousel, "nextButton", nextButton);

        return panel;
    }

    private static GameObject BuildHeroesPanel(
        Transform navRoot,
        HeroCatalogService heroCatalogService,
        HeroPortraitLibrary portraitLibrary)
    {
        GameObject panel = FindOrCreateUIChild(navRoot, "Panel_Heroes");
        AnchorPanelAboveNavBar(panel.GetComponent<RectTransform>());

        HeroCollectionPresenter presenter = GetOrAddComponent<HeroCollectionPresenter>(panel);

        GameObject filterRow = FindOrCreateUIChild(panel.transform, "FilterChipContainer");
        SetAnchors(filterRow.GetComponent<RectTransform>(), new Vector2(0f, 0.9f), Vector2.one);
        HorizontalLayoutGroup filterLayout = GetOrAddComponent<HorizontalLayoutGroup>(filterRow);
        filterLayout.childForceExpandWidth = false;
        filterLayout.childForceExpandHeight = true;
        filterLayout.spacing = 8;
        filterLayout.padding = new RectOffset(16, 16, 8, 8);
        filterLayout.childAlignment = TextAnchor.MiddleLeft;

        Transform gridContent = CreateVerticalScrollView(panel.transform, "HeroGridScroll", new Vector2(0f, 0f), new Vector2(1f, 0.88f), out _);
        GridLayoutGroup grid = GetOrAddComponent<GridLayoutGroup>(gridContent.gameObject);
        grid.cellSize = new Vector2(220, 280);
        grid.spacing = new Vector2(12, 12);
        grid.padding = new RectOffset(12, 12, 12, 12);
        ContentSizeFitter fitter = GetOrAddComponent<ContentSizeFitter>(gridContent.gameObject);
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        HeroCollectionItemView itemPrefab = GetOrCreateHeroCollectionItemPrefab();
        PantheonFilterChipView chipPrefab = GetOrCreatePantheonFilterChipPrefab();

        SetPrivateField(presenter, "heroCatalogService", heroCatalogService);
        SetPrivateField(presenter, "portraitLibrary", portraitLibrary);
        SetPrivateField(presenter, "heroGridContainer", gridContent);
        SetPrivateField(presenter, "heroItemPrefab", itemPrefab);
        SetPrivateField(presenter, "filterChipContainer", filterRow.transform);
        SetPrivateField(presenter, "filterChipPrefab", chipPrefab);

        return panel;
    }

    private static GameObject BuildShopPanel(Transform navRoot)
    {
        GameObject panel = FindOrCreateUIChild(navRoot, "Panel_Shop");
        AnchorPanelAboveNavBar(panel.GetComponent<RectTransform>());

        ShopPanelPresenter presenter = GetOrAddComponent<ShopPanelPresenter>(panel);

        TextMeshProUGUI comingSoon = AddTMPTextChild(panel.transform, "ComingSoonText", "Shop coming soon", 26, TextAlignmentOptions.Center, new Vector2(0.1f, 0.45f), new Vector2(0.9f, 0.55f));

        SetPrivateField(presenter, "comingSoonText", comingSoon);

        return panel;
    }

    private static GameObject BuildProfilePanel(Transform navRoot, PlayerProfileService playerProfileService)
    {
        GameObject panel = FindOrCreateUIChild(navRoot, "Panel_Profile");
        AnchorPanelAboveNavBar(panel.GetComponent<RectTransform>());

        ProfilePanelPresenter presenter = GetOrAddComponent<ProfilePanelPresenter>(panel);

        TextMeshProUGUI nameText = AddTMPTextChild(panel.transform, "PlayerNameText", "Recruit", 32, TextAlignmentOptions.Center, new Vector2(0.1f, 0.85f), new Vector2(0.9f, 0.95f));
        TextMeshProUGUI idText = AddTMPTextChild(panel.transform, "PlayerIdText", "Offline profile", 16, TextAlignmentOptions.Center, new Vector2(0.1f, 0.8f), new Vector2(0.9f, 0.85f));
        TextMeshProUGUI levelText = AddTMPTextChild(panel.transform, "PlayerLevelText", "Level 1", 24, TextAlignmentOptions.Center, new Vector2(0.1f, 0.7f), new Vector2(0.9f, 0.78f));
        Slider xpBar = AddSliderChild(panel.transform, "XpBar", new Vector2(0.15f, 0.63f), new Vector2(0.85f, 0.68f));
        TextMeshProUGUI xpLabel = AddTMPTextChild(panel.transform, "XpLabelText", "0 / 1000 XP", 16, TextAlignmentOptions.Center, new Vector2(0.1f, 0.56f), new Vector2(0.9f, 0.62f));
        TextMeshProUGUI softText = AddTMPTextChild(panel.transform, "SoftCurrencyText", "0", 20, TextAlignmentOptions.Center, new Vector2(0.15f, 0.45f), new Vector2(0.45f, 0.53f));
        TextMeshProUGUI hardText = AddTMPTextChild(panel.transform, "HardCurrencyText", "0", 20, TextAlignmentOptions.Center, new Vector2(0.55f, 0.45f), new Vector2(0.85f, 0.53f));

        SetPrivateField(presenter, "playerProfileService", playerProfileService);
        SetPrivateField(presenter, "playerNameText", nameText);
        SetPrivateField(presenter, "playerIdText", idText);
        SetPrivateField(presenter, "playerLevelText", levelText);
        SetPrivateField(presenter, "xpBar", xpBar);
        SetPrivateField(presenter, "xpLabelText", xpLabel);
        SetPrivateField(presenter, "softCurrencyText", softText);
        SetPrivateField(presenter, "hardCurrencyText", hardText);

        return panel;
    }

    // =================================================================
    // Bottom navigation
    // =================================================================

    private static GameObject BuildBottomNavBar(
        Transform canvasTransform,
        out NavTabButtonView btnHome,
        out NavTabButtonView btnHeroes,
        out NavTabButtonView btnShop,
        out NavTabButtonView btnProfile)
    {
        GameObject navBar = FindOrCreateUIChild(canvasTransform, "BottomNavBar");
        SetAnchors(navBar.GetComponent<RectTransform>(), Vector2.zero, new Vector2(1f, NavBarHeightRatio));

        Image navBarBg = GetOrAddComponent<Image>(navBar);
        navBarBg.color = new Color(0.05f, 0.05f, 0.07f, 1f);

        HorizontalLayoutGroup layout = GetOrAddComponent<HorizontalLayoutGroup>(navBar);
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        layout.childAlignment = TextAnchor.MiddleCenter;

        btnHome = CreateNavButton(navBar.transform, "NavButton_Home", "Home", HomeTabType.Home);
        btnHeroes = CreateNavButton(navBar.transform, "NavButton_Heroes", "Heroes", HomeTabType.Heroes);
        btnShop = CreateNavButton(navBar.transform, "NavButton_Shop", "Shop", HomeTabType.Shop);
        btnProfile = CreateNavButton(navBar.transform, "NavButton_Profile", "Profile", HomeTabType.Profile);

        return navBar;
    }

    private static NavTabButtonView CreateNavButton(Transform parent, string name, string label, HomeTabType tab)
    {
        GameObject go = FindOrCreateUIChild(parent, name);

        Image bg = GetOrAddComponent<Image>(go);
        bg.color = new Color(0f, 0f, 0f, 0f);

        Button button = GetOrAddComponent<Button>(go);

        GameObject iconGo = FindOrCreateUIChild(go.transform, "Icon");
        SetAnchors(iconGo.GetComponent<RectTransform>(), new Vector2(0.3f, 0.45f), new Vector2(0.7f, 0.9f));
        Image icon = GetOrAddComponent<Image>(iconGo);
        icon.color = new Color(1f, 1f, 1f, 0.6f);

        TextMeshProUGUI labelText = AddTMPTextChild(go.transform, "Label", label, 16, TextAlignmentOptions.Center, new Vector2(0f, 0.05f), new Vector2(1f, 0.35f));

        GameObject indicatorGo = FindOrCreateUIChild(go.transform, "SelectedIndicator");
        SetAnchors(indicatorGo.GetComponent<RectTransform>(), new Vector2(0.3f, 0f), new Vector2(0.7f, 0.05f));
        Image indicatorImage = GetOrAddComponent<Image>(indicatorGo);
        indicatorImage.color = new Color(0.3f, 0.75f, 1f, 1f);
        indicatorGo.SetActive(false);

        NavTabButtonView view = GetOrAddComponent<NavTabButtonView>(go);
        SetEnumField(view, "tab", (int)tab);
        SetPrivateField(view, "iconImage", icon);
        SetPrivateField(view, "labelText", labelText);
        SetPrivateField(view, "selectedIndicator", indicatorGo);
        SetPrivateField(view, "button", button);

        return view;
    }

    // =================================================================
    // Prefabs
    // =================================================================

    private static HeroCollectionItemView GetOrCreateHeroCollectionItemPrefab()
    {
        const string path = "Assets/Home/Prefabs/HeroCollectionItem.prefab";
        HeroCollectionItemView existing = AssetDatabase.LoadAssetAtPath<HeroCollectionItemView>(path);
        if (existing != null)
        {
            return existing;
        }

        EnsureFolder("Assets/Home/Prefabs");

        GameObject root = new GameObject("HeroCollectionItem", typeof(RectTransform));
        root.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 280);

        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.13f, 1f);
        Button button = root.AddComponent<Button>();

        GameObject portraitGo = CreateUIChildRaw(root.transform, "Portrait");
        SetAnchors(portraitGo.GetComponent<RectTransform>(), new Vector2(0.05f, 0.35f), new Vector2(0.95f, 0.95f));
        Image portraitImage = portraitGo.AddComponent<Image>();
        portraitImage.color = Color.white;

        TextMeshProUGUI nameText = CreateTMPRaw(root.transform, "HeroName", "Hero Name", 20, TextAlignmentOptions.Center, new Vector2(0f, 0.22f), new Vector2(1f, 0.34f));
        TextMeshProUGUI classText = CreateTMPRaw(root.transform, "HeroClass", "Class", 15, TextAlignmentOptions.Center, new Vector2(0f, 0.12f), new Vector2(1f, 0.22f));
        classText.color = new Color(1f, 1f, 1f, 0.75f);
        TextMeshProUGUI pantheonText = CreateTMPRaw(root.transform, "PantheonTag", "Pantheon", 13, TextAlignmentOptions.Center, new Vector2(0f, 0.02f), new Vector2(1f, 0.12f));
        pantheonText.color = new Color(0.6f, 0.8f, 1f, 1f);

        HeroCollectionItemView view = root.AddComponent<HeroCollectionItemView>();
        SetPrivateField(view, "portraitImage", portraitImage);
        SetPrivateField(view, "heroNameText", nameText);
        SetPrivateField(view, "heroClassText", classText);
        SetPrivateField(view, "pantheonTagText", pantheonText);
        SetPrivateField(view, "button", button);

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);

        return savedPrefab.GetComponent<HeroCollectionItemView>();
    }

    private static PantheonFilterChipView GetOrCreatePantheonFilterChipPrefab()
    {
        const string path = "Assets/Home/Prefabs/PantheonFilterChip.prefab";
        PantheonFilterChipView existing = AssetDatabase.LoadAssetAtPath<PantheonFilterChipView>(path);
        if (existing != null)
        {
            return existing;
        }

        EnsureFolder("Assets/Home/Prefabs");

        GameObject root = new GameObject("PantheonFilterChip", typeof(RectTransform));
        root.GetComponent<RectTransform>().sizeDelta = new Vector2(110, 40);

        Image bg = root.AddComponent<Image>();
        bg.color = new Color(1f, 1f, 1f, 0.08f);
        Button button = root.AddComponent<Button>();

        LayoutElement layoutElement = root.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 110;
        layoutElement.preferredHeight = 32;

        TextMeshProUGUI labelText = CreateTMPRaw(root.transform, "Label", "All", 16, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);

        GameObject indicatorGo = CreateUIChildRaw(root.transform, "SelectedIndicator");
        SetAnchors(indicatorGo.GetComponent<RectTransform>(), new Vector2(0.1f, 0f), new Vector2(0.9f, 0.08f));
        Image indicatorImage = indicatorGo.AddComponent<Image>();
        indicatorImage.color = new Color(0.3f, 0.75f, 1f, 1f);
        indicatorGo.SetActive(false);

        PantheonFilterChipView view = root.AddComponent<PantheonFilterChipView>();
        SetPrivateField(view, "labelText", labelText);
        SetPrivateField(view, "selectedIndicator", indicatorGo);
        SetPrivateField(view, "button", button);

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);

        return savedPrefab.GetComponent<PantheonFilterChipView>();
    }

    // =================================================================
    // Low-level UI helpers
    // =================================================================

    private static TextMeshProUGUI AddTMPTextChild(Transform parent, string name, string defaultText, int fontSize, TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = FindOrCreateUIChild(parent, name);
        SetAnchors(go.GetComponent<RectTransform>(), anchorMin, anchorMax);

        TextMeshProUGUI text = GetOrAddComponent<TextMeshProUGUI>(go);
        text.text = defaultText;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        return text;
    }

    private static Button AddButtonChild(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = FindOrCreateUIChild(parent, name);
        SetAnchors(go.GetComponent<RectTransform>(), anchorMin, anchorMax);

        Image bg = GetOrAddComponent<Image>(go);
        bg.color = new Color(0.2f, 0.5f, 0.9f, 1f);

        Button button = GetOrAddComponent<Button>(go);

        AddTMPTextChild(go.transform, "Label", label, 22, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);

        return button;
    }

    private static Slider AddSliderChild(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = FindOrCreateUIChild(parent, name);
        SetAnchors(go.GetComponent<RectTransform>(), anchorMin, anchorMax);

        Slider slider = go.GetComponent<Slider>();
        bool isNewSlider = slider == null;
        if (isNewSlider)
        {
            slider = go.AddComponent<Slider>();
        }

        GameObject bgGo = FindOrCreateUIChild(go.transform, "Background");
        SetAnchors(bgGo.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
        Image bgImage = GetOrAddComponent<Image>(bgGo);
        bgImage.color = new Color(1f, 1f, 1f, 0.15f);

        GameObject fillAreaGo = FindOrCreateUIChild(go.transform, "Fill Area");
        SetAnchors(fillAreaGo.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);

        GameObject fillGo = FindOrCreateUIChild(fillAreaGo.transform, "Fill");
        SetAnchors(fillGo.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
        Image fillImage = GetOrAddComponent<Image>(fillGo);
        fillImage.color = new Color(0.3f, 0.75f, 1f, 1f);

        if (isNewSlider)
        {
            slider.fillRect = fillGo.GetComponent<RectTransform>();
            slider.targetGraphic = fillImage;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0f;
            slider.interactable = false;
        }

        return slider;
    }

    private static Transform CreateVerticalScrollView(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, out ScrollRect scrollRect)
    {
        GameObject scrollGo = FindOrCreateUIChild(parent, name);
        SetAnchors(scrollGo.GetComponent<RectTransform>(), anchorMin, anchorMax);

        scrollRect = GetOrAddComponent<ScrollRect>(scrollGo);
        Image scrollBg = GetOrAddComponent<Image>(scrollGo);
        scrollBg.color = new Color(0f, 0f, 0f, 0.05f);

        GameObject viewportGo = FindOrCreateUIChild(scrollGo.transform, "Viewport");
        SetAnchors(viewportGo.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
        Image viewportImage = GetOrAddComponent<Image>(viewportGo);
        viewportImage.color = Color.white;
        Mask mask = GetOrAddComponent<Mask>(viewportGo);
        mask.showMaskGraphic = false;

        GameObject contentGo = FindOrCreateUIChild(viewportGo.transform, "Content");
        RectTransform contentRt = contentGo.GetComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.anchoredPosition = Vector2.zero;

        scrollRect.viewport = viewportGo.GetComponent<RectTransform>();
        scrollRect.content = contentRt;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        return contentRt;
    }

    // Raw (non-idempotent) child creators, used only inside freshly-created
    // prefab roots where "find existing" makes no sense.
    private static GameObject CreateUIChildRaw(Transform parent, string name)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static TextMeshProUGUI CreateTMPRaw(Transform parent, string name, string defaultText, int fontSize, TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = CreateUIChildRaw(parent, name);
        SetAnchors(go.GetComponent<RectTransform>(), anchorMin, anchorMax);

        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.text = defaultText;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        return text;
    }

    // =================================================================
    // Generic helpers
    // =================================================================

    private static GameObject FindOrCreateRoot(string name)
    {
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name == name)
            {
                return root;
            }
        }

        return new GameObject(name);
    }

    private static GameObject FindOrCreateChild(Transform parent, string name)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            return existing.gameObject;
        }

        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go;
    }

    private static GameObject FindOrCreateUIChild(Transform parent, string name)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            return existing.gameObject;
        }

        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }

        return comp;
    }

    private static void SetAnchors(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax)
    {
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void AnchorPanelAboveNavBar(RectTransform rt)
    {
        SetAnchors(rt, new Vector2(0f, NavBarHeightRatio), Vector2.one);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        int lastSlash = path.LastIndexOf('/');
        string parent = path.Substring(0, lastSlash);
        string leaf = path.Substring(lastSlash + 1);

        if (!AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        AssetDatabase.CreateFolder(parent, leaf);
    }

    private static void SetPrivateField(Object target, string fieldName, Object value)
    {
        var so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(fieldName);

        if (prop == null)
        {
            Debug.LogError($"[HomeSceneBuilder] Field '{fieldName}' not found on {target.GetType().Name}");
            return;
        }

        prop.objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetEnumField(Object target, string fieldName, int enumIndex)
    {
        var so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(fieldName);

        if (prop == null)
        {
            Debug.LogError($"[HomeSceneBuilder] Field '{fieldName}' not found on {target.GetType().Name}");
            return;
        }

        prop.enumValueIndex = enumIndex;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void ConfigureNavigationTabs(HomeNavigationController controller, (HomeTabType tab, GameObject panel, NavTabButtonView button)[] entries)
    {
        var so = new SerializedObject(controller);
        SerializedProperty tabsProp = so.FindProperty("tabs");
        tabsProp.arraySize = entries.Length;

        for (int i = 0; i < entries.Length; i++)
        {
            SerializedProperty element = tabsProp.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("tab").enumValueIndex = (int)entries[i].tab;
            element.FindPropertyRelative("panel").objectReferenceValue = entries[i].panel;
            element.FindPropertyRelative("navButton").objectReferenceValue = entries[i].button;
        }

        so.ApplyModifiedPropertiesWithoutUndo();
    }
}