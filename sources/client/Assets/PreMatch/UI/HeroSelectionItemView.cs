using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Représente une entrée visuelle unitaire de la liste de héros dans l'écran de sélection.
/// Cette vue reste passive : elle affiche le nom, l'état sélectionné
/// et relaie le clic utilisateur vers le presenter.
/// </summary>
public class HeroSelectionItemView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text heroNameText;
    [SerializeField] private GameObject selectedIndicator;
    [SerializeField] private Button button;

    private string heroId;
    private Action<string> onClick;

    public string HeroId => heroId;

    public void Setup(string heroId, string displayName, bool isSelected, Action<string> onClick)
    {
        this.heroId = heroId;
        this.onClick = onClick;

        if (heroNameText != null)
        {
            heroNameText.text = displayName;
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
        onClick?.Invoke(heroId);
    }
}
