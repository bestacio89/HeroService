using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Déclenche la transition depuis le Home/Lobby vers la scène de match
/// (Prototype_Map, qui contient déjà ModeSelectionPresenter et
/// HeroSelectionPanelPresenter).
///
/// Responsabilités :
/// - charger la scène cible de façon asynchrone,
/// - afficher une barre de progression pendant le chargement.
///
/// Important :
/// - ne contient AUCUNE logique de sélection de mode/héros,
/// - la scène cible garde l'entière responsabilité du flux pré-match existant.
/// </summary>
public class MainMenuSceneLoader : MonoBehaviour
{
    [Header("Target Scene")]
    [Tooltip("Doit correspondre au nom de scène ajouté dans Build Settings.")]
    [SerializeField] private string matchSceneName = "Prototype_Map";

    [Header("Loading Overlay")]
    [SerializeField] private GameObject loadingOverlayRoot;
    [SerializeField] private Slider loadingProgressBar;

    public event Action OnLoadStarted;
    public event Action OnLoadFailed;

    private bool isLoading;

    private void Awake()
    {
        if (loadingOverlayRoot != null)
        {
            loadingOverlayRoot.SetActive(false);
        }
    }

    public void StartMatch()
    {
        if (isLoading)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(matchSceneName))
        {
            Debug.LogError("[MainMenuSceneLoader] matchSceneName is empty.");
            OnLoadFailed?.Invoke();
            return;
        }

        StartCoroutine(LoadMatchSceneRoutine());
    }

    private System.Collections.IEnumerator LoadMatchSceneRoutine()
    {
        isLoading = true;
        OnLoadStarted?.Invoke();

        if (loadingOverlayRoot != null)
        {
            loadingOverlayRoot.SetActive(true);
        }

        if (loadingProgressBar != null)
        {
            loadingProgressBar.value = 0f;
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(matchSceneName, LoadSceneMode.Single);

        if (op == null)
        {
            Debug.LogError($"[MainMenuSceneLoader] Scene '{matchSceneName}' is not registered in Build Settings.");
            isLoading = false;
            OnLoadFailed?.Invoke();
            yield break;
        }

        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = Mathf.Clamp01(op.progress);
            }

            yield return null;
        }

        // Note : isLoading reste true car la scène change — cet objet
        // va être détruit avec le reste de la scène Home (LoadSceneMode.Single).
    }
}
