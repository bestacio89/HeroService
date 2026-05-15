using UnityEngine;
using UnityEngine.UI;

namespace MobaPrototype.UI
{
    /// <summary>
    /// Représente un marqueur individuel sur la minimap.
    /// </summary>
    public class MinimapMarkerView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Graphic graphic;
        [SerializeField] private CanvasGroup canvasGroup;

        public RectTransform RectTransform => rectTransform;

        private void Reset()
        {
            rectTransform = GetComponent<RectTransform>();
            graphic = GetComponent<Graphic>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
        }

        public void SetAnchoredPosition(Vector2 anchoredPosition)
        {
            if (rectTransform == null)
                return;

            rectTransform.anchoredPosition = anchoredPosition;
        }

        public void SetVisible(bool visible)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
                return;
            }

            gameObject.SetActive(visible);
        }

        public void SetSize(float size)
        {
            if (rectTransform == null)
                return;

            rectTransform.sizeDelta = new Vector2(size, size);
        }

        public void SetColor(Color color)
        {
            if (graphic != null)
                graphic.color = color;
        }

        public void SetRotation(float zRotation)
        {
            if (rectTransform == null)
                return;

            rectTransform.localEulerAngles = new Vector3(0f, 0f, zRotation);
        }
    }
}