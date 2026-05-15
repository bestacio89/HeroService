using UnityEngine;

/// <summary>
/// Simple top-down camera follow system.
/// Suit automatiquement le joueur local via le tag "Player" si aucune cible n'est assignée.
/// </summary>
public class CameraFollowTopDown : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -8f);
    [SerializeField] private float followSpeed = 8f;

    private void Start()
    {
        if (target == null)
        {
            TryAssignPlayerTarget();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public bool TryAssignPlayerTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("[CameraFollowTopDown] Aucun GameObject tagué 'Player' trouvé.");
            return false;
        }

        target = player.transform;
        return true;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );
    }
}