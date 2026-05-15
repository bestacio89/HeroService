using UnityEngine;

namespace MobaPrototype.UI
{
    /// <summary>
    /// Définit les bornes monde utilisées pour projeter les positions 3D sur la minimap UI.
    /// </summary>
    public class MinimapBounds : MonoBehaviour
    {
        [Header("World Bounds")]
        [SerializeField] private Vector2 worldMin = new Vector2(-25f, -25f);
        [SerializeField] private Vector2 worldMax = new Vector2(25f, 25f);

        public Vector2 WorldMin => worldMin;
        public Vector2 WorldMax => worldMax;

        /// <summary>
        /// Convertit une position monde en coordonnées normalisées [0..1] pour la minimap.
        /// X = world X, Y = world Z.
        /// </summary>
        public Vector2 WorldToNormalized(Vector3 worldPosition)
        {
            float normalizedX = Mathf.InverseLerp(worldMin.x, worldMax.x, worldPosition.x);
            float normalizedY = Mathf.InverseLerp(worldMin.y, worldMax.y, worldPosition.z);

            return new Vector2(normalizedX, normalizedY);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 center = new Vector3(
                (worldMin.x + worldMax.x) * 0.5f,
                0f,
                (worldMin.y + worldMax.y) * 0.5f
            );

            Vector3 size = new Vector3(
                Mathf.Abs(worldMax.x - worldMin.x),
                0.1f,
                Mathf.Abs(worldMax.y - worldMin.y)
            );

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}