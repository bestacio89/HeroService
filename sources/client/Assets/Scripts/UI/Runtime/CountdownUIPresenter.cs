using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Presenter passif du countdown runtime (3, 2, 1, GO).
///
/// Responsabilités :
/// - s'abonner aux événements de <see cref="CountdownController"/>,
/// - afficher les valeurs reçues dans le HUD,
/// - se masquer automatiquement après le GO.
///
/// Propriétaire légitime :
/// - <see cref="MatchStartFlowController"/> appelle <see cref="Initialize"/>, <see cref="Show"/> et <see cref="Hide"/>.
///
/// Contraintes d'architecture :
/// - Aucune logique de flow ni de gameplay dans cette classe.
/// - Ce script ne prend aucune décision : il reçoit des données et les affiche.
/// - Utiliser TMP_Text à la place de Text si le projet passe sur TextMeshPro.
/// </summary>
public class CountdownUIPresenter : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;
    private Animator animator; // optionnel — déclenche "Pulse" à chaque tick

    private CountdownController _countdownController;

    /// <summary>
    /// Initialise le presenter et s'abonne aux événements du countdown.
    /// Doit être appelé par <see cref="MatchStartFlowController"/> avant <see cref="Show"/>.
    /// </summary>
    public void Initialize(CountdownController countdownController)
    {
        _countdownController = countdownController;
        _countdownController.OnCountdownTick += HandleCountdownTick;
        _countdownController.OnCountdownCompleted += HandleCountdownCompleted;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_countdownController == null) return;
        _countdownController.OnCountdownTick -= HandleCountdownTick;
        _countdownController.OnCountdownCompleted -= HandleCountdownCompleted;
    }

    /// <summary>Rend le panneau visible et réinitialise le texte.</summary>
    public void Show()
    {
        gameObject.SetActive(true);
        countdownText.text = string.Empty;
    }

    /// <summary>Masque immédiatement le panneau.</summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // --- Handlers : présentation uniquement

    private void HandleCountdownTick(int value)
    {
        countdownText.text = value.ToString();
        animator?.SetTrigger("Pulse");
    }

    private void HandleCountdownCompleted()
    {
        countdownText.text = "GO!";
        if (gameObject.activeInHierarchy)
            StartCoroutine(HideAfterDelay(1.2f));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Hide();
    }
}