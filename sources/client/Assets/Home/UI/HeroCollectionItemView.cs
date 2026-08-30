using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Représente une entrée visuelle de la grille Hero Collection.
/// Contrairement à HeroSelectionItemView (flux pré-match, avec verrouillage
/// après confirmation), cette vue est en lecture seule : elle sert à parcourir
/// le roster, pas à préparer un match. Le clic est réservé pour un futur
/// panneau de détail héros (lore, skills, skins).
/// </summary>
public class HeroCollectionItemView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text heroNameText;
    [SerializeField] private TMP_Text heroClassText;
    [SerializeField] private TMP_Text pantheonTagText;
    [SerializeField] private Button button;

    private string heroId;
    private Action<string> onClick;

    public string HeroId => heroId;

    public void Setup(HeroListItemDto hero, Sprite portrait, Action<string> onClick)
    {
        heroId = hero.id;
        this.onClick = onClick;

        if (heroNameText != null)
        {
            heroNameText.text = hero.name;
        }

        if (heroClassText != null && hero.heroClass != null)
        {
            heroClassText.text = hero.heroClass.name;
        }

        if (pantheonTagText != null && hero.affiliation != null)
        {
            pantheonTagText.text = hero.affiliation.mythologyName;
        }

        if (portraitImage != null && portrait != null)
        {
            portraitImage.sprite = portrait;
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
