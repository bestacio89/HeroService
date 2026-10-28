using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Owns local client preferences only. These values affect presentation and
/// device rendering; they are not gameplay rules or server-authoritative state.
/// </summary>
[DisallowMultipleComponent]
public sealed class ClientSettingsService : MonoBehaviour
{
    private const string MasterVolumeKey = "client.settings.masterVolume";
    private const string QualityLevelKey = "client.settings.qualityLevel";
    private const string TargetFrameRateKey = "client.settings.targetFrameRate";
    private const string FullscreenKey = "client.settings.fullscreen";
    private const string ResolutionWidthKey = "client.settings.resolutionWidth";
    private const string ResolutionHeightKey = "client.settings.resolutionHeight";
    private const string InterfaceScaleKey = "client.settings.interfaceScale";

    private static readonly int[] SupportedFrameRates = { 30, 60, 90, 120 };
    private static readonly float[] SupportedInterfaceScales = { 0.9f, 1f, 1.1f, 1.2f };

    public event Action OnSettingsChanged;

    public float MasterVolume { get; private set; }
    public int QualityLevel { get; private set; }
    public int TargetFrameRate { get; private set; }
    public bool Fullscreen { get; private set; }
    public float InterfaceScale { get; private set; } = 1f;
    public string InterfaceScaleName => Mathf.RoundToInt(InterfaceScale * 100f) + "%";

    private int defaultScreenWidth;
    private int defaultScreenHeight;

    public string ResolutionName
    {
        get
        {
            int width = Application.isMobilePlatform
                ? Screen.width
                : PlayerPrefs.GetInt(
                    ResolutionWidthKey,
                    defaultScreenWidth > 0 ? defaultScreenWidth : Screen.width);
            int height = Application.isMobilePlatform
                ? Screen.height
                : PlayerPrefs.GetInt(
                    ResolutionHeightKey,
                    defaultScreenHeight > 0 ? defaultScreenHeight : Screen.height);

            return width + " × " + height;
        }
    }

    public string QualityName
    {
        get
        {
            string[] names = QualitySettings.names;
            return names.Length > 0 && QualityLevel >= 0 && QualityLevel < names.Length
                ? names[QualityLevel]
                : "Default";
        }
    }

    public int[] AvailableFrameRates => (int[])SupportedFrameRates.Clone();

    private void Awake()
    {
        defaultScreenWidth = Screen.currentResolution.width > 0
            ? Screen.currentResolution.width
            : Screen.width;
        defaultScreenHeight = Screen.currentResolution.height > 0
            ? Screen.currentResolution.height
            : Screen.height;
        LoadAndApply();
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value);
        AudioListener.volume = MasterVolume;
        PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
        SaveAndNotify();
    }

    public void CycleQuality(int direction)
    {
        int count = QualitySettings.names.Length;
        if (count == 0)
        {
            return;
        }

        QualityLevel = (QualityLevel + Math.Sign(direction) + count) % count;
        QualitySettings.SetQualityLevel(QualityLevel, true);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFrameRate;
        PlayerPrefs.SetInt(QualityLevelKey, QualityLevel);
        SaveAndNotify();
    }

    public void CycleFrameRate(int direction)
    {
        int currentIndex = Array.IndexOf(SupportedFrameRates, TargetFrameRate);
        if (currentIndex < 0)
        {
            currentIndex = 1;
        }

        currentIndex = (currentIndex + Math.Sign(direction) + SupportedFrameRates.Length)
            % SupportedFrameRates.Length;
        TargetFrameRate = SupportedFrameRates[currentIndex];
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFrameRate;
        PlayerPrefs.SetInt(TargetFrameRateKey, TargetFrameRate);
        SaveAndNotify();
    }

    public void CycleResolution(int direction)
    {
        if (Application.isMobilePlatform)
        {
            return;
        }

        Resolution[] resolutions = GetAvailableResolutions();
        if (resolutions.Length == 0)
        {
            return;
        }

        int currentIndex = FindCurrentResolutionIndex(resolutions);
        int nextIndex = (currentIndex + Math.Sign(direction) + resolutions.Length) % resolutions.Length;
        Resolution selected = resolutions[nextIndex];

        PlayerPrefs.SetInt(ResolutionWidthKey, selected.width);
        PlayerPrefs.SetInt(ResolutionHeightKey, selected.height);
        Screen.SetResolution(
            selected.width,
            selected.height,
            Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        SaveAndNotify();
    }

    public void CycleInterfaceScale(int direction)
    {
        int currentIndex = Array.FindIndex(
            SupportedInterfaceScales,
            value => Mathf.Approximately(value, InterfaceScale));
        if (currentIndex < 0)
        {
            currentIndex = 1;
        }

        currentIndex = (currentIndex + Math.Sign(direction) + SupportedInterfaceScales.Length)
            % SupportedInterfaceScales.Length;
        InterfaceScale = SupportedInterfaceScales[currentIndex];
        PlayerPrefs.SetFloat(InterfaceScaleKey, InterfaceScale);
        ApplyInterfaceScale();
        SaveAndNotify();
    }

    public void ToggleFullscreen()
    {
        if (Application.isMobilePlatform)
        {
            return;
        }

        Fullscreen = !Fullscreen;
        Screen.fullScreen = Fullscreen;
        PlayerPrefs.SetInt(FullscreenKey, Fullscreen ? 1 : 0);
        SaveAndNotify();
    }

    public void ResetToDefaults()
    {
        MasterVolume = 1f;
        QualityLevel = GetDefaultQualityLevel();
        TargetFrameRate = 60;
        Fullscreen = true;
        InterfaceScale = 1f;

        ApplyAll();
        if (!Application.isMobilePlatform && defaultScreenWidth > 0 && defaultScreenHeight > 0)
        {
            PlayerPrefs.SetInt(ResolutionWidthKey, defaultScreenWidth);
            PlayerPrefs.SetInt(ResolutionHeightKey, defaultScreenHeight);
            Screen.SetResolution(
                defaultScreenWidth,
                defaultScreenHeight,
                FullScreenMode.FullScreenWindow);
        }

        PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
        PlayerPrefs.SetInt(QualityLevelKey, QualityLevel);
        PlayerPrefs.SetInt(TargetFrameRateKey, TargetFrameRate);
        PlayerPrefs.SetInt(FullscreenKey, Fullscreen ? 1 : 0);
        PlayerPrefs.SetFloat(InterfaceScaleKey, InterfaceScale);
        SaveAndNotify();
    }

    private void LoadAndApply()
    {
        int qualityCount = QualitySettings.names.Length;
        int defaultQuality = GetDefaultQualityLevel();

        MasterVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
        QualityLevel = qualityCount == 0
            ? 0
            : Mathf.Clamp(PlayerPrefs.GetInt(QualityLevelKey, defaultQuality), 0, qualityCount - 1);

        int savedFrameRate = PlayerPrefs.GetInt(TargetFrameRateKey, 60);
        TargetFrameRate = Array.IndexOf(SupportedFrameRates, savedFrameRate) >= 0
            ? savedFrameRate
            : 60;

        Fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) != 0;
        InterfaceScale = Mathf.Clamp(PlayerPrefs.GetFloat(InterfaceScaleKey, 1f), 0.9f, 1.2f);
        ApplyAll();

        if (!Application.isMobilePlatform)
        {
            int savedWidth = PlayerPrefs.GetInt(ResolutionWidthKey, defaultScreenWidth);
            int savedHeight = PlayerPrefs.GetInt(ResolutionHeightKey, defaultScreenHeight);
            if (savedWidth > 0 && savedHeight > 0)
            {
                Screen.SetResolution(
                    savedWidth,
                    savedHeight,
                    Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
            }
        }
    }

    private void ApplyAll()
    {
        AudioListener.volume = MasterVolume;
        if (QualitySettings.names.Length > 0)
        {
            QualitySettings.SetQualityLevel(QualityLevel, true);
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFrameRate;
        ApplyInterfaceScale();
        if (!Application.isMobilePlatform)
        {
            Screen.fullScreen = Fullscreen;
        }
    }

    private void ApplyInterfaceScale()
    {
        ResponsiveCanvasScaler[] scalers = FindObjectsOfType<ResponsiveCanvasScaler>(true);
        foreach (ResponsiveCanvasScaler scaler in scalers)
        {
            if (scaler != null)
            {
                scaler.SetScaleMultiplier(InterfaceScale);
            }
        }
    }

    private static int GetDefaultQualityLevel()
    {
        int qualityCount = QualitySettings.names.Length;
        if (qualityCount == 0)
        {
            return 0;
        }

        return Application.isMobilePlatform
            ? Mathf.Min(1, qualityCount - 1)
            : qualityCount - 1;
    }

    private static Resolution[] GetAvailableResolutions()
    {
        Resolution[] supported = Screen.resolutions;
        if (supported == null || supported.Length == 0)
        {
            return Array.Empty<Resolution>();
        }

        List<Resolution> unique = new(supported.Length);
        foreach (Resolution candidate in supported)
        {
            // The client is configured for landscape MOBA ergonomics. Do not
            // offer portrait desktop modes that would contradict the menu layout.
            if (candidate.width < candidate.height)
            {
                continue;
            }

            bool alreadyAdded = unique.Exists(
                existing => existing.width == candidate.width && existing.height == candidate.height);
            if (!alreadyAdded)
            {
                unique.Add(candidate);
            }
        }

        unique.Sort((left, right) =>
        {
            int widthComparison = left.width.CompareTo(right.width);
            return widthComparison != 0
                ? widthComparison
                : left.height.CompareTo(right.height);
        });

        return unique.ToArray();
    }

    private static int FindCurrentResolutionIndex(Resolution[] resolutions)
    {
        int index = Array.FindIndex(
            resolutions,
            resolution => resolution.width == Screen.width && resolution.height == Screen.height);

        return index >= 0 ? index : resolutions.Length - 1;
    }

    private void SaveAndNotify()
    {
        PlayerPrefs.Save();
        OnSettingsChanged?.Invoke();
    }
}
