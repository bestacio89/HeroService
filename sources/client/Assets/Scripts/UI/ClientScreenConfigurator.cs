using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Applies shared mobile display behavior to every loaded client scene.
/// PreMatchScreenLayout owns selection-panel composition; this component
/// handles canvas scaling and safe-area offsets for edge-anchored HUD elements.
/// </summary>
public static class ClientScreenConfigurator
{
    private static readonly string[] EdgePaddedRoots =
    {
        "Joystick",
        "CombatUI",
        "MatchTimerText",
        "MatchStateText",
        "MinimapPanel"
    };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneCallbacks()
    {
        if (Application.isMobilePlatform)
        {
            // MOBA controls and menu composition are landscape-first. Permit
            // both landscape rotations while excluding portrait orientations.
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }

        SceneManager.sceneLoaded -= ConfigureLoadedScene;
        SceneManager.sceneLoaded += ConfigureLoadedScene;
    }

    private static void ConfigureLoadedScene(Scene scene, LoadSceneMode mode)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Canvas[] canvases = root.GetComponentsInChildren<Canvas>(true);
            foreach (Canvas canvas in canvases)
            {
                ConfigureCanvas(canvas);
            }
        }

        foreach (string rootName in EdgePaddedRoots)
        {
            ConfigureNamedRoot<SafeAreaEdgePadding>(scene, rootName);
        }
    }

    private static void ConfigureCanvas(Canvas canvas)
    {
        if (canvas == null || canvas.renderMode == RenderMode.WorldSpace)
        {
            return;
        }

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        if (scaler.referenceResolution.x <= 0f || scaler.referenceResolution.y <= 0f)
        {
            scaler.referenceResolution = new Vector2(1080f, 1920f);
        }

        if (canvas.GetComponent<ResponsiveCanvasScaler>() == null)
        {
            canvas.gameObject.AddComponent<ResponsiveCanvasScaler>();
        }

        if (canvas.GetComponent<PreMatchScreenLayout>() == null)
        {
            canvas.gameObject.AddComponent<PreMatchScreenLayout>();
        }
    }

    private static void ConfigureNamedRoot<T>(Scene scene, string objectName)
        where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform candidate in transforms)
            {
                if (candidate.name != objectName || candidate.GetComponent<RectTransform>() == null)
                {
                    continue;
                }

                if (candidate.GetComponent<T>() == null)
                {
                    candidate.gameObject.AddComponent<T>();
                }
            }
        }
    }
}
