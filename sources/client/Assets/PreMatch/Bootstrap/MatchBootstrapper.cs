using MobaPrototype.Managers;
using System;
using UnityEngine;

/// <summary>
/// Prépare puis applique la transition entre le pré-match et le runtime du match.
/// 
/// Responsabilités :
/// - valider que la session confirmée est exploitable,
/// - construire un MatchLaunchContext,
/// - demander au HeroSpawner d'appliquer ce contexte au runtime local,
/// - exposer un signal léger de succès / échec.
///
/// Ne contient pas de logique gameplay :
/// - ne démarre pas le match,
/// - ne pilote pas le GameManager,
/// - ne gère ni HUD, ni caméra, ni fermeture UI.
/// </summary>
public class MatchBootstrapper : MonoBehaviour
{
    [Header("Services")]
    [SerializeField] private HeroSelectionSessionService sessionService;

    [Header("Runtime")]
    [SerializeField] private HeroSpawner heroSpawner;

    private MatchLaunchContext preparedContext;

    public event Action<MatchLaunchContext> OnRuntimeContextApplied;
    public event Action OnRuntimeContextApplyFailed;

    public MatchLaunchContext PreparedContext => preparedContext;

    public bool HasPreparedContext =>
        preparedContext != null &&
        !string.IsNullOrWhiteSpace(preparedContext.SelectedHeroId);

    public bool TryPrepareMatchLaunch()
    {
        preparedContext = null;

        if (sessionService == null)
        {
            Debug.LogWarning("[MatchBootstrapper] SessionService is missing.");
            return false;
        }

        if (sessionService.CurrentSession == null)
        {
            Debug.LogWarning("[MatchBootstrapper] CurrentSession is null.");
            return false;
        }

        if (!sessionService.CurrentSession.IsConfirmed)
        {
            Debug.LogWarning("[MatchBootstrapper] Cannot prepare launch: selection not confirmed.");
            return false;
        }

        MatchLaunchContext context = sessionService.BuildMatchLaunchContext();

        if (context == null)
        {
            Debug.LogWarning("[MatchBootstrapper] BuildMatchLaunchContext returned null.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(context.SelectedHeroId))
        {
            Debug.LogWarning("[MatchBootstrapper] Launch context invalid: SelectedHeroId is empty.");
            return false;
        }

        preparedContext = context;

        Debug.Log(
            $"[MatchBootstrapper] Launch context prepared | " +
            $"Mode={preparedContext.SelectedMode} | " +
            $"Hero={preparedContext.SelectedHeroId} | " +
            $"Lane={preparedContext.PreferredLane}"
        );

        return true;
    }

    public bool TryApplyPreparedContextToRuntime()
    {
        if (!HasPreparedContext)
        {
            Debug.LogWarning("[MatchBootstrapper] Cannot apply runtime context: no prepared context.");
            return false;
        }

        if (heroSpawner == null)
        {
            Debug.LogWarning("[MatchBootstrapper] HeroSpawner is missing.");
            return false;
        }

        bool applied = heroSpawner.TryApplyLaunchContext(preparedContext);

        if (!applied)
        {
            Debug.LogWarning("[MatchBootstrapper] Prepared context could not be applied to runtime.");
            return false;
        }

        Debug.Log("[MatchBootstrapper] Prepared context successfully applied to runtime.");
        return true;
    }

    public bool TryPrepareAndApply()
    {
        if (!TryPrepareMatchLaunch())
        {
            OnRuntimeContextApplyFailed?.Invoke();
            return false;
        }

        if (!TryApplyPreparedContextToRuntime())
        {
            OnRuntimeContextApplyFailed?.Invoke();
            return false;
        }

        OnRuntimeContextApplied?.Invoke(preparedContext);
        return true;
    }

    public void ClearPreparedContext()
    {
        preparedContext = null;
    }
}