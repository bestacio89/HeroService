using UnityEngine;

/// <summary>
/// Expose l'état d'autorisation des inputs gameplay pour le joueur local.
/// 
/// Responsabilités :
/// - centraliser un bool simple d'autorisation,
/// - permettre aux scripts joueur de savoir si l'input gameplay est autorisé,
/// - rester agnostique de la logique de match.
/// </summary>
public class PlayerInputGate : MonoBehaviour
{
    [Header("Runtime State")]
    [SerializeField] private bool gameplayInputEnabled = true;

    public bool GameplayInputEnabled => gameplayInputEnabled;

    public void SetGameplayInputEnabled(bool enabled)
    {
        if (gameplayInputEnabled == enabled)
        {
            return;
        }

        gameplayInputEnabled = enabled;
        Debug.Log($"[PlayerInputGate] Gameplay input enabled = {enabled}");
    }

    public bool IsGameplayInputAllowed()
    {
        return gameplayInputEnabled;
    }
}