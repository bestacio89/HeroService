using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Représente un bouton unitaire de la barre de navigation inférieure
/// (Home / Heroes / Shop / Profile), à la manière de Mobile Legends.
/// Cette vue reste passive : elle affiche l'icône/le label, l'état
/// sélectionné, et relaie le clic vers HomeNavigationController.
/// </summary>
public class NavTabButtonView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HomeTabType tab;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private GameObject selectedIndicator;
    [SerializeField] private Button button;

    [Header("Icon Tint")]
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor = new Color(1f, 1f, 1f, 0.6f);

    private Action<HomeTabType> onClick;

    public HomeTabType Tab => tab;

    public void Setup(Action<HomeTabType> onClick)
    {
        this.onClick = onClick;
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(isSelected);
        }

        if (iconImage != null)
        {
            iconImage.color = isSelected ? selectedColor : unselectedColor;
        }

        if (labelText != null)
        {
            labelText.color = isSelected ? selectedColor : unselectedColor;
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
        onClick?.Invoke(tab);
    }
}
