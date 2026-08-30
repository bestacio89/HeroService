using TMPro;
using UnityEngine;

/// <summary>
/// Emplacement d'écran pour le Shop (Shop tab).
///
/// Volontairement laissé au stade "Coming soon" : le Shop nécessite des
/// décisions de contenu et d'économie qui n'existent pas encore
/// (catalogue d'items, prix en monnaie molle/dure, packs, rotation de skins).
/// Construire cette logique maintenant reviendrait à deviner un modèle
/// économique — l'onglet garde donc sa place dans la navigation, prêt
/// à recevoir un ShopCatalogService + ShopItemView le jour venu.
/// </summary>
public class ShopPanelPresenter : MonoBehaviour
{
    [Header("Placeholder")]
    [SerializeField] private TMP_Text comingSoonText;

    private void Awake()
    {
        if (comingSoonText != null)
        {
            comingSoonText.text = "Shop coming soon";
        }
    }
}
