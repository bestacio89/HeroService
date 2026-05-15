using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Exécute un countdown runtime générique (3, 2, 1, GO).
///
/// Responsabilités :
/// - émettre les ticks numérotés via <see cref="OnCountdownTick"/>,
/// - émettre le signal de complétion via <see cref="OnCountdownCompleted"/>,
/// - rester agnostique du gameplay métier et de toute logique UI.
///
/// Propriétaires légitimes :
/// - <see cref="MatchStartFlowController"/> démarre et arrête le countdown.
///
/// Observateurs légitimes :
/// - <see cref="CountdownUIPresenter"/> s'abonne aux événements pour afficher les valeurs.
///
/// Contraintes d'architecture :
/// - Ce script ne connaît pas le HUD, le GameManager ni l'état du match.
/// - Ne pas injecter de logique de flow ou de gameplay dans cette classe.
/// </summary>
public class CountdownController : MonoBehaviour
{
    [Header("Countdown")]
    [SerializeField] private int defaultStartValue = 3;
    [SerializeField] private float tickIntervalSeconds = 1f;

    private Coroutine _countdownRoutine;
    private bool _isRunning;

    /// <summary>Émis à chaque tick descendant (3, 2, 1). La valeur transmise est le chiffre courant.</summary>
    public event Action<int> OnCountdownTick;

    /// <summary>Émis une fois le countdown terminé, au moment du GO.</summary>
    public event Action OnCountdownCompleted;

    /// <summary>Vrai si un countdown est en cours d'exécution.</summary>
    public bool IsRunning => _isRunning;

    /// <summary>
    /// Démarre le countdown avec la valeur par défaut configurée dans l'Inspector.
    /// Retourne false si un countdown est déjà en cours.
    /// </summary>
    public bool TryStartCountdown()
    {
        return TryStartCountdown(defaultStartValue);
    }

    /// <summary>
    /// Démarre le countdown avec une valeur de départ explicite.
    /// Retourne false si un countdown est déjà en cours ou si la valeur est invalide.
    /// </summary>
    public bool TryStartCountdown(int startValue)
    {
        if (_isRunning)
        {
            Debug.LogWarning("[CountdownController] Countdown already running.");
            return false;
        }

        if (startValue <= 0)
        {
            Debug.LogWarning("[CountdownController] Invalid start value — must be > 0.");
            return false;
        }

        _countdownRoutine = StartCoroutine(RunCountdown(startValue));
        return true;
    }

    /// <summary>
    /// Interrompt le countdown en cours sans émettre <see cref="OnCountdownCompleted"/>.
    /// À appeler uniquement en cas d'abandon du match ou de reset de flow.
    /// </summary>
    public void StopCountdown()
    {
        if (_countdownRoutine != null)
        {
            StopCoroutine(_countdownRoutine);
            _countdownRoutine = null;
        }

        _isRunning = false;
    }

    private IEnumerator RunCountdown(int startValue)
    {
        _isRunning = true;

        for (int value = startValue; value > 0; value--)
        {
            Debug.Log($"[CountdownController] Tick — {value}");
            OnCountdownTick?.Invoke(value);
            yield return new WaitForSecondsRealtime(tickIntervalSeconds);
        }

        Debug.Log("[CountdownController] GO — countdown completed.");
        OnCountdownCompleted?.Invoke();

        _countdownRoutine = null;
        _isRunning = false;
    }
}