using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MobaPrototype.Core
{
    /// <summary>
    /// Script temporaire — remet tous les enfants SpawnPoints à Y=0.
    /// À supprimer après usage.
    /// </summary>
    public class SpawnPointResetter : MonoBehaviour
    {
#if UNITY_EDITOR
        [ContextMenu("Reset All SpawnPoints Y")]
        private void ResetAllSpawnPointsY()
        {
            foreach (Transform child in transform)
            {
                Vector3 pos = child.localPosition;
                pos.y = 0f;
                child.localPosition = pos;
                ;
            }
            EditorUtility.SetDirty(gameObject);
        }
#endif
    }
}