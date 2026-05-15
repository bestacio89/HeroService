// Scripts/Player/HeroSelector.cs
// Permet de sélectionner un héros via une liste dans l'Inspector.
// Au changement de héros : met à jour HeroRuntime, recharge les slots via HeroLoader.

using UnityEngine;
using MobaPrototype.Systems.Heroes;

namespace MobaPrototype.Player
{
    /// <summary>
    /// Gère la sélection locale du héros à utiliser au runtime.
    /// </summary>
    public class HeroSelector : MonoBehaviour
    {
        [Header("Héros disponibles")]
        [SerializeField] private HeroDefinition[] availableHeroes;

        [Header("Sélection")]
        [SerializeField] private int selectedHeroIndex = 0;

        [Header("References")]
        [SerializeField] private HeroRuntime heroRuntime;
        [SerializeField] private HeroLoader heroLoader;

        private void Awake()
        {
            if (heroRuntime == null) heroRuntime = GetComponent<HeroRuntime>();
            if (heroLoader == null) heroLoader = GetComponent<HeroLoader>();
        }

        private void Start()
        {
            ApplySelectedHero();
        }

        /// <summary>
        /// Sélectionne un héros par index et applique immédiatement.
        /// Peut être appelé depuis un bouton UI ou un menu de sélection.
        /// </summary>
        public void SelectHero(int index)
        {
            if (availableHeroes == null || availableHeroes.Length == 0)
            {
                Debug.LogWarning("[HeroSelector] Aucun héros dans la liste.");
                return;
            }

            selectedHeroIndex = Mathf.Clamp(index, 0, availableHeroes.Length - 1);
            ApplySelectedHero();
        }

        /// <summary>
        /// Sélectionne un héros par son heroId (ex: "byakuya").
        /// </summary>
        public void SelectHeroById(string heroId)
        {
            if (availableHeroes == null) return;

            for (int i = 0; i < availableHeroes.Length; i++)
            {
                if (availableHeroes[i] != null &&
                    availableHeroes[i].HeroId == heroId)
                {
                    SelectHero(i);
                    return;
                }
            }

            Debug.LogWarning($"[HeroSelector] Héros introuvable : {heroId}");
        }

        private void ApplySelectedHero()
        {
            if (availableHeroes == null || availableHeroes.Length == 0) return;
            if (selectedHeroIndex < 0 || selectedHeroIndex >= availableHeroes.Length) return;

            HeroDefinition selected = availableHeroes[selectedHeroIndex];
            if (selected == null) return;

            // Met à jour HeroRuntime — déclenche OnHeroDefinitionChanged
            // ce qui recalcule les stats via HeroStatsProvider automatiquement
            heroRuntime.SetHeroDefinition(selected);

            // Recharge les slots de compétences pour le nouveau héros
            heroLoader.ReloadHero();

            ;
        }
    }
}