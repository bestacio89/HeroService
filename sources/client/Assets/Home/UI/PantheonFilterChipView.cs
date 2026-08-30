using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Chip de filtre par panthéon (ex: "All", "Greek", "Norse", "Egyptian"...).
/// Les valeurs sont générées dynamiquement par HeroCollectionPresenter à partir
/// des mythologyName présents dans le catalogue — aucune liste de panthéons
/// n'est codée en dur ici pour éviter la duplication avec la taxonomie backend.
/// </summary>
public class PantheonFilterChipView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private GameObject selectedIndicator;
    [SerializeField] private Button button;

    private string filterValue;
    private Action<string> onClick;

    public void Setup(string label, string filterValue, bool isSelected, Action<string> onClick)
    {
        this.filterValue = filterValue;
        this.onClick = onClick;

        if (labelText != null)
        {
            labelText.text = label;
        }

        SetSelected(isSelected);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(isSelected);
        }
    }

    private void Awake()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
            button.onClick.AddListener(HandleClick);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        onClick?.Invoke(filterValue);
    }
}
