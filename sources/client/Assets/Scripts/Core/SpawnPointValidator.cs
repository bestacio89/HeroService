using UnityEngine;

namespace MobaPrototype.Core
{
    /// <summary>
    /// Valide et affiche la position world-space de ce SpawnPoint en éditeur.
    /// Attache sur chaque TeamX_Spawn_XX.
    /// </summary>
    [ExecuteAlways]
    /// <summary>
    /// Vérifie la cohérence des points de spawn et signale les erreurs de configuration.
    /// </summary>
    public class SpawnPointValidator : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool showGizmo = true;
        [SerializeField] private Color gizmoColor = Color.cyan;
        [SerializeField] private float gizmoRadius = 0.3f;

        private void OnDrawGizmos()
        {
            if (!showGizmo) return;
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, gizmoRadius);
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 0.5f,
                $"{gameObject.name}\n{transform.position}"
            );
        }

        private void Update()
        {
            if (Application.isPlaying) return;

            // Alerte si Y est aberrant
            if (Mathf.Abs(transform.position.y) > 10f)
            {
                Debug.LogError(
                    $"[SpawnPointValidator] ⚠️ {gameObject.name} a une position Y aberrante : " +
                    $"{transform.position.y}. Vérifier la Transform !"
                );
            }
        }
#endif
    }
}