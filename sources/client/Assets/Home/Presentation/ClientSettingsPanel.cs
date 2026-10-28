using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds the settings overlay at runtime so it can be used by the generated
/// Home scene without requiring manually maintained prefab references.
/// The panel exposes only settings that have real client-side effects.
/// </summary>
[DisallowMultipleComponent]
public sealed class ClientSettingsPanel : MonoBehaviour
{
    [SerializeField] private Button openButton;
    [SerializeField] private ClientSettingsService settingsService;

    private GameObject overlay;
    private TMP_Text volumeValue;
    private TMP_Text qualityValue;
    private TMP_Text frameRateValue;
    private TMP_Text resolutionValue;
    private TMP_Text fullscreenValue;
    private Slider masterVolumeSlider;

    private void Awake()
    {
        if (settingsService == null)
        {
            settingsService = FindObjectOfType<ClientSettingsService>();
        }

        BuildOverlay();
        RefreshValues();
    }

    private void Start()
    {
        // SettingsService.Awake may execute after this component's Awake.
        RefreshValues();
    }

    private void OnEnable()
    {
        if (settingsService != null)
        {
            settingsService.OnSettingsChanged += RefreshValues;
        }

        if (openButton != null)
        {
            openButton.onClick.RemoveListener(Open);
            openButton.onClick.AddListener(Open);
        }
    }

    private void OnDisable()
    {
        if (settingsService != null)
        {
            settingsService.OnSettingsChanged -= RefreshValues;
        }

        if (openButton != null)
        {
            openButton.onClick.RemoveListener(Open);
        }
    }

    private void BuildOverlay()
    {
        overlay = CreateRect("SettingsOverlay", transform);
        Stretch(overlay.GetComponent<RectTransform>());
        Image dimmer = overlay.AddComponent<Image>();
        dimmer.color = new Color(0.015f, 0.02f, 0.035f, 0.88f);

        GameObject safePanel = CreateRect("SafePanel", overlay.transform);
        RectTransform safeRect = safePanel.GetComponent<RectTransform>();
        safeRect.anchorMin = new Vector2(0.04f, 0.04f);
        safeRect.anchorMax = new Vector2(0.96f, 0.96f);
        safeRect.offsetMin = Vector2.zero;
        safeRect.offsetMax = Vector2.zero;
        safePanel.AddComponent<SafeAreaController>();

        GameObject panel = CreateRect("Panel", safePanel.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.03f, 0.02f);
        panelRect.anchorMax = new Vector2(0.97f, 0.98f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelBackground = panel.AddComponent<Image>();
        panelBackground.color = new Color(0.055f, 0.065f, 0.09f, 1f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(24, 24, 20, 20);
        layout.spacing = 14;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        TMP_Text heading = AddLabel(panel.transform, "SETTINGS", 30, 64, FontStyles.Bold);
        heading.gameObject.AddComponent<LayoutElement>().preferredHeight = 64f;

        GameObject body = CreateRect("SettingsBody", panel.transform);
        LayoutElement bodyLayout = body.AddComponent<LayoutElement>();
        bodyLayout.flexibleHeight = 1f;
        ScrollRect scrollRect = body.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        GameObject viewport = CreateRect("Viewport", body.transform);
        Stretch(viewport.GetComponent<RectTransform>());
        viewport.AddComponent<RectMask2D>();

        GameObject bodyContent = CreateRect("Content", viewport.transform);
        RectTransform contentRect = bodyContent.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = Vector2.one;
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;
        VerticalLayoutGroup contentLayout = bodyContent.AddComponent<VerticalLayoutGroup>();
        contentLayout.spacing = 14f;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = false;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;
        ContentSizeFitter contentFitter = bodyContent.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = contentRect;

        GameObject audioSection = CreateSection(bodyContent.transform, "AUDIO");
        CreateSliderRow(audioSection.transform, "Master volume", out volumeValue, out masterVolumeSlider);
        masterVolumeSlider.minValue = 0f;
        masterVolumeSlider.maxValue = 1f;
        masterVolumeSlider.wholeNumbers = false;
        masterVolumeSlider.onValueChanged.AddListener(value =>
        {
            if (settingsService != null)
            {
                settingsService.SetMasterVolume(value);
            }
        });

        GameObject displaySection = CreateSection(bodyContent.transform, "DISPLAY");
        CreateActionRow(displaySection.transform, "Graphics quality", out qualityValue,
            () => settingsService?.CycleQuality(-1),
            () => settingsService?.CycleQuality(1));
        CreateActionRow(displaySection.transform, "Frame rate limit", out frameRateValue,
            () => settingsService?.CycleFrameRate(-1),
            () => settingsService?.CycleFrameRate(1));

        if (!Application.isMobilePlatform)
        {
            CreateActionRow(displaySection.transform, "Resolution", out resolutionValue,
                () => settingsService?.CycleResolution(-1),
                () => settingsService?.CycleResolution(1));
            CreateToggleRow(displaySection.transform, "Fullscreen", out fullscreenValue,
                () => settingsService?.ToggleFullscreen());
        }

        GameObject footer = CreateRect("Footer", panel.transform);
        HorizontalLayoutGroup footerLayout = footer.AddComponent<HorizontalLayoutGroup>();
        footerLayout.spacing = 12;
        footerLayout.childForceExpandWidth = true;
        footerLayout.childForceExpandHeight = true;
        footerLayout.childControlWidth = true;
        footerLayout.childControlHeight = true;
        footer.AddComponent<LayoutElement>().preferredHeight = 56f;
        AddButton(footer.transform, "Reset defaults", () => settingsService?.ResetToDefaults(), 52);
        AddButton(footer.transform, "Close", Close, 52);

        overlay.SetActive(false);
    }

    private GameObject CreateSection(Transform parent, string title)
    {
        GameObject section = CreateRect(title + "Section", parent);
        VerticalLayoutGroup layout = section.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        AddLabel(section.transform, title, 18, 30, FontStyles.Bold);
        return section;
    }

    private void CreateSliderRow(Transform parent, string title, out TMP_Text valueText, out Slider slider)
    {
        GameObject row = CreateRect(title + "Row", parent);
        RectTransform rowRect = row.GetComponent<RectTransform>();
        rowRect.sizeDelta = new Vector2(0f, 64f);
        HorizontalLayoutGroup layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 12;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;

        AddLabel(row.transform, title, 18, 64, FontStyles.Normal).gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
        slider = CreateSlider(row.transform, "ValueSlider");
        LayoutElement sliderLayout = slider.gameObject.AddComponent<LayoutElement>();
        sliderLayout.preferredWidth = 260f;
        sliderLayout.flexibleWidth = 1f;
        valueText = AddLabel(row.transform, "100%", 18, 64, FontStyles.Bold);
        valueText.gameObject.AddComponent<LayoutElement>().preferredWidth = 72f;
    }

    private void CreateActionRow(
        Transform parent,
        string title,
        out TMP_Text valueText,
        UnityEngine.Events.UnityAction previous,
        UnityEngine.Events.UnityAction next)
    {
        CreateActionRow(parent, title, out valueText, previous, next, out _);
    }

    private void CreateActionRow(
        Transform parent,
        string title,
        out TMP_Text valueText,
        UnityEngine.Events.UnityAction previous,
        UnityEngine.Events.UnityAction next,
        out Button actionButton)
    {
        GameObject row = CreateRect(title + "Row", parent);
        row.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 64f);
        HorizontalLayoutGroup layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 12;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;

        AddLabel(row.transform, title, 18, 64, FontStyles.Normal).gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
        AddButton(row.transform, "‹", previous, 64);
        valueText = AddLabel(row.transform, string.Empty, 18, 64, FontStyles.Bold);
        valueText.alignment = TextAlignmentOptions.Center;
        valueText.gameObject.AddComponent<LayoutElement>().preferredWidth = 150f;
        actionButton = AddButton(row.transform, "›", next, 64);
    }

    private Slider CreateSlider(Transform parent, string name)
    {
        GameObject root = CreateRect(name, parent);
        Image background = root.AddComponent<Image>();
        background.color = new Color(1f, 1f, 1f, 0.12f);

        GameObject fill = CreateRect("Fill", root.transform);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.3f, 0.75f, 1f, 1f);

        Slider slider = root.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.targetGraphic = background;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.direction = Slider.Direction.LeftToRight;
        return slider;
    }

    private TMP_Text AddLabel(Transform parent, string text, int fontSize, float height, FontStyles style)
    {
        GameObject labelObject = CreateRect(text + "Label", parent);
        labelObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, height);
        TMP_Text label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.fontStyle = style;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.enableWordWrapping = false;
        return label;
    }

    private Button AddButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick, float height)
    {
        GameObject buttonObject = CreateRect(label + "Button", parent);
        buttonObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, height);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.12f, 0.28f, 0.42f, 1f);
        Button button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(onClick);

        TMP_Text text = AddLabel(buttonObject.transform, label, 18, height, FontStyles.Bold);
        RectTransform labelRect = text.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(8f, 0f);
        labelRect.offsetMax = new Vector2(-8f, 0f);
        return button;
    }

    private static GameObject CreateRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void RefreshValues()
    {
        if (settingsService == null)
        {
            return;
        }

        if (volumeValue != null)
        {
            volumeValue.text = Mathf.RoundToInt(settingsService.MasterVolume * 100f) + "%";
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.SetValueWithoutNotify(settingsService.MasterVolume);
        }

        if (qualityValue != null)
        {
            qualityValue.text = settingsService.QualityName;
        }

        if (frameRateValue != null)
        {
            frameRateValue.text = settingsService.TargetFrameRate + " FPS";
        }

        if (resolutionValue != null)
        {
            resolutionValue.text = settingsService.ResolutionName;
        }

        if (fullscreenValue != null)
        {
            fullscreenValue.text = settingsService.Fullscreen ? "On" : "Off";
        }
    }

    private void CreateToggleRow(
        Transform parent,
        string title,
        out TMP_Text valueText,
        UnityEngine.Events.UnityAction toggle)
    {
        GameObject row = CreateRect(title + "Row", parent);
        row.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 64f);
        HorizontalLayoutGroup layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 12;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;

        AddLabel(row.transform, title, 18, 64, FontStyles.Normal)
            .gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
        valueText = AddLabel(row.transform, string.Empty, 18, 64, FontStyles.Bold);
        valueText.alignment = TextAlignmentOptions.Center;
        valueText.gameObject.AddComponent<LayoutElement>().preferredWidth = 100f;
        AddButton(row.transform, "Toggle", toggle, 56);
    }

    private void Open()
    {
        if (overlay != null)
        {
            overlay.SetActive(true);
            RefreshValues();
        }
    }

    private void Close()
    {
        if (overlay != null)
        {
            overlay.SetActive(false);
        }
    }
}
