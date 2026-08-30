using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Associe un heroId à un portrait (Sprite) côté client.
///
/// Pourquoi ce composant existe :
/// - HeroListItemDto / HeroSnapshotDto ne transportent aucune référence d'art
///   (pas d'URL de portrait côté backend pour l'instant).
/// - Le carrousel du Home et l'écran Hero Collection ont besoin d'un visuel
///   par héros dès maintenant.
///
/// MVP V1 : mapping statique assigné dans l'Inspector, même logique que
/// la liste de héros statique de HeroSelectionPanelPresenter.
/// À terme : remplacer par une résolution via Addressables/CDN une fois
/// qu'un champ portraitKey (ou équivalent) existera côté backend.
/// </summary>
public class HeroPortraitLibrary : MonoBehaviour
{
    [Serializable]
    private struct Entry
    {
        public string heroId;
        public Sprite portrait;
    }

    [Header("Portraits")]
    [SerializeField] private Entry[] entries = Array.Empty<Entry>();

    [Header("Fallback")]
    [SerializeField] private Sprite defaultPortrait;

    private Dictionary<string, Sprite> lookup;

    private void Awake()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        lookup = new Dictionary<string, Sprite>(entries.Length);

        foreach (Entry entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry.heroId) || entry.portrait == null)
            {
                continue;
            }

            lookup[entry.heroId] = entry.portrait;
        }
    }

    public Sprite GetPortrait(string heroId)
    {
        if (lookup == null)
        {
            BuildLookup();
        }

        if (!string.IsNullOrWhiteSpace(heroId) && lookup.TryGetValue(heroId, out Sprite portrait))
        {
            return portrait;
        }

        return defaultPortrait;
    }
}
