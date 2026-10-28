using System;
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

    private static readonly int[] SupportedFrameRates = { 30, 60, 90, 120 };

    public event Action OnSettingsChanged;

    public float MasterVolume { get; private set; }
    public int QualityLevel { get; private set; }
    public int TargetFrameRate { get; private set; }
    public bool Fullscreen { get; private set; }

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
        QualityLevel = Mathf.Max(0, QualitySettings.names.Length - 1);
        TargetFrameRate = 60;
        Fullscreen = true;

        ApplyAll();
        PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
        PlayerPrefs.SetInt(QualityLevelKey, QualityLevel);
        PlayerPrefs.SetInt(TargetFrameRateKey, TargetFrameRate);
        PlayerPrefs.SetInt(FullscreenKey, Fullscreen ? 1 : 0);
        SaveAndNotify();
    }

    private void LoadAndApply()
    {
        int qualityCount = QualitySettings.names.Length;
        int defaultQuality = Mathf.Max(0, qualityCount - 1);

        MasterVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
        QualityLevel = qualityCount == 0
            ? 0
            : Mathf.Clamp(PlayerPrefs.GetInt(QualityLevelKey, defaultQuality), 0, qualityCount - 1);

        int savedFrameRate = PlayerPrefs.GetInt(TargetFrameRateKey, 60);
        TargetFrameRate = Array.IndexOf(SupportedFrameRates, savedFrameRate) >= 0
            ? savedFrameRate
            : 60;

        Fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) != 0;
        ApplyAll();
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
        if (!Application.isMobilePlatform)
        {
            Screen.fullScreen = Fullscreen;
        }
    }

    private void SaveAndNotify()
    {
        PlayerPrefs.Save();
        OnSettingsChanged?.Invoke();
    }
}
