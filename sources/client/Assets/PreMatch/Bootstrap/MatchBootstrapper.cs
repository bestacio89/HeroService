using MobaPrototype.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transitional local-prototype bootstrap between pre-match selection and runtime.
/// It validates the confirmed local selection and applies a local HeroSpawner context.
/// This is not an authoritative match initialization flow: once MatchService exists,
/// this boundary must request and apply its match-scoped definition/snapshot instead
/// of assembling match state from HeroService data or client-side rules.
/// 
/// Current responsibilities:
/// - validate the confirmed local selection,
/// - construct a presentation/runtime launch context,
/// - apply the local prototype context to HeroSpawner,
/// - expose a lightweight success/failure signal.
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

    public IReadOnlyList<HeroSelectionMapping> AvailableHeroMappings =>
        heroSpawner != null
            ? heroSpawner.AvailableHeroMappings
            : Array.Empty<HeroSelectionMapping>();

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