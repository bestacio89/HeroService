using UnityEngine;

namespace MobaPrototype.Core
{
    [ExecuteAlways]
    /// <summary>
    /// Maintient le transform local verrouillé sur des valeurs de référence.
    /// </summary>
    public class LockTransform : MonoBehaviour
    {
        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (transform.localPosition != Vector3.zero)
                {
                    transform.localPosition = Vector3.zero;
                    Debug.LogWarning($"[LockTransform] {gameObject.name} localPosition remis à (0,0,0).");
                }
                if (transform.localRotation != Quaternion.identity)
                {
                    transform.localRotation = Quaternion.identity;
                    Debug.LogWarning($"[LockTransform] {gameObject.name} localRotation remise à zéro.");
                }
            }
#endif
        }
    }
}