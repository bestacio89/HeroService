using System;
using MobaPrototype.Managers;
using UnityEngine;

/// <summary>
/// Gère les phases de démarrage du match côté runtime.
///
/// Responsabilités :
/// - exposer un état de start flow (Idle / PreGame / Countdown / InGame),
/// - entrer en PreGame après la transition runtime,
/// - lancer le countdown et piloter sa visibilité UI,
/// - verrouiller / déverrouiller les inputs gameplay,
/// - déclencher le vrai StartMatch à la fin du countdown.
///
/// Dépendances directes :
/// - <see cref="GameManager"/> : source de vérité du match.
/// - <see cref="CountdownController"/> : exécution technique du countdown.
/// - <see cref="CountdownUIPresenter"/> : affichage passif du countdown dans le HUD.
/// - <see cref="PlayerInputGate"/> : autorisation d'input gameplay local.
///
/// Contraintes d'architecture :
/// - Ce script orchestre le flow ; il ne contient pas de logique de gameplay métier.
/// - <see cref="CountdownUIPresenter"/> est piloté ici et nulle part ailleurs.
/// </summary>
public class MatchStartFlowController : MonoBehaviour
{
    public enum StartFlowState
    {
        Idle,
        PreGame,
        Countdown,
        InGame
    }

    [Header("Runtime References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CountdownController countdownController;

    [Header("UI")]
    [SerializeField] private CountdownUIPresenter countdownUIPresenter;

    private StartFlowState currentState = StartFlowState.Idle;
    private bool isSubscribedToCountdown;
    private PlayerInputGate localPlayerInputGate;

    public event Action OnPreGameEntered;
    public event Action OnCountdownStarted;
    public event Action<int> OnCountdownTick;
    public event Action OnInGameEntered;

    public StartFlowState CurrentState => currentState;
    public bool IsIdle => currentState == StartFlowState.Idle;
    public bool IsPreGame => currentState == StartFlowState.PreGame;
    public bool IsCountdown => currentState == StartFlowState.Countdown;
    public bool IsInGame => currentState == StartFlowState.InGame;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = GameManager.Instance;

        // L'abonnement aux events du CountdownController se fait une seule fois, ici.
        SubscribeToCountdown();
        InitializeCountdownUI();
    }

    private void OnDestroy()
    {
        UnsubscribeFromCountdown();
    }

    public bool TryEnterPreGame()
    {
        if (currentState == StartFlowState.PreGame)
            return true;

        if (currentState == StartFlowState.Countdown || currentState == StartFlowState.InGame)
        {
            Debug.LogWarning("[MatchStartFlow] Cannot enter PreGame from current state.");
            return false;
        }

        if (!TryResolveLocalPlayerInputGate())
            Debug.LogWarning("[MatchStartFlow] Local PlayerInputGate not found.");

        SetGameplayInputEnabled(false);

        currentState = StartFlowState.PreGame;
        Debug.Log("[MatchStartFlow] Entered PreGame.");
        OnPreGameEntered?.Invoke();
        return true;
    }

    public bool TryStartCountdown()
    {
        if (currentState != StartFlowState.PreGame)
        {
            Debug.LogWarning("[MatchStartFlow] Cannot start countdown: not in PreGame.");
            return false;
        }

        if (countdownController == null)
        {
            Debug.LogWarning("[MatchStartFlow] CountdownController is missing.");
            return false;
        }

        // Pas de SubscribeToCountdown() ici — l'abonnement est garanti depuis Awake().

        SetGameplayInputEnabled(false);

        currentState = StartFlowState.Countdown;
        Debug.Log("[MatchStartFlow] Countdown started.");
        OnCountdownStarted?.Invoke();

        countdownUIPresenter?.Show();

        bool started = countdownController.TryStartCountdown();

        if (!started)
        {
            currentState = StartFlowState.PreGame;
            countdownUIPresenter?.Hide();
            Debug.LogWarning("[MatchStartFlow] Countdown failed to start.");
            return false;
        }

        return true;
    }

    public bool TryEnterInGame()
    {
        if (currentState == StartFlowState.InGame)
            return true;

        if (gameManager == null)
            gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogWarning("[MatchStartFlow] GameManager missing.");
            return false;
        }

        if (!gameManager.IsMatchReady())
        {
            Debug.LogWarning("[MatchStartFlow] Match is not ready.");
            return false;
        }

        gameManager.StartMatch();

        if (!TryResolveLocalPlayerInputGate())
            Debug.LogWarning("[MatchStartFlow] Local PlayerInputGate not found.");

        SetGameplayInputEnabled(true);

        // Sécurité : force le masquage du presenter si HideAfterDelay ne s'est pas encore exécuté.
        countdownUIPresenter?.Hide();

        currentState = StartFlowState.InGame;
        Debug.Log("[MatchStartFlow] Entered InGame.");
        OnInGameEntered?.Invoke();
        return true;
    }

    public void ResetFlow()
    {
        if (countdownController != null && countdownController.IsRunning)
            countdownController.StopCountdown();

        countdownUIPresenter?.Hide();

        SetGameplayInputEnabled(false);
        currentState = StartFlowState.Idle;
    }

    // --- Initialisation UI

    /// <summary>
    /// Initialise le CountdownUIPresenter si présent.
    /// Appelé dans Awake — le presenter est caché par défaut après initialisation.
    /// </summary>
    private void InitializeCountdownUI()
    {
        if (countdownUIPresenter == null)
        {
            Debug.LogWarning("[MatchStartFlow] CountdownUIPresenter not assigned — countdown UI will be skipped.");
            return;
        }

        if (countdownController == null)
        {
            Debug.LogWarning("[MatchStartFlow] Cannot initialize CountdownUIPresenter: CountdownController is missing.");
            return;
        }

        countdownUIPresenter.Initialize(countdownController);
    }

    // --- Abonnements countdown

    private void SubscribeToCountdown()
    {
        if (countdownController == null || isSubscribedToCountdown) return;

        countdownController.OnCountdownTick += HandleCountdownTick;
        countdownController.OnCountdownCompleted += HandleCountdownCompleted;
        isSubscribedToCountdown = true;
    }

    private void UnsubscribeFromCountdown()
    {
        if (countdownController == null || !isSubscribedToCountdown) return;

        countdownController.OnCountdownTick -= HandleCountdownTick;
        countdownController.OnCountdownCompleted -= HandleCountdownCompleted;
        isSubscribedToCountdown = false;
    }

    private void HandleCountdownTick(int value)
    {
        OnCountdownTick?.Invoke(value);
    }

    private void HandleCountdownCompleted()
    {
        TryEnterInGame();
    }

    // --- Input gate

    private bool TryResolveLocalPlayerInputGate()
    {
        if (localPlayerInputGate != null) return true;

        if (gameManager == null)
            gameManager = GameManager.Instance;

        if (gameManager == null || gameManager.LocalHero == null) return false;

        localPlayerInputGate = gameManager.LocalHero.GetComponent<PlayerInputGate>();
        return localPlayerInputGate != null;
    }

    private void SetGameplayInputEnabled(bool enabled)
    {
        if (localPlayerInputGate != null)
            localPlayerInputGate.SetGameplayInputEnabled(enabled);
    }
}
