// Scripts/Player/HeroLoader.cs
using System;
using System.Collections;
using UnityEngine;
using MobaPrototype.Heroes;
using MobaPrototype.Heroes.Byakuya;
using MobaPrototype.Systems.Heroes;

namespace MobaPrototype.Player
{
    /// <summary>
    /// Charge et instancie visuellement le héros local choisi au runtime.
    /// </summary>
    public class HeroLoader : MonoBehaviour
    {
        [Header("Les 4 slots fixes — toujours présents sur le Player")]
        [SerializeField] private HeroSkillSlot slotBasic;
        [SerializeField] private HeroSkillSlot slotSkillOne;
        [SerializeField] private HeroSkillSlot slotSkillTwo;
        [SerializeField] private HeroSkillSlot slotUltimate;

        [Header("Reference")]
        [SerializeField] private HeroRuntime heroRuntime;

        private Component[] _activeHeroComponents;
        private bool _isLoaded = false;

        public event Action OnHeroLoaded;

        private void Awake()
        {
            if (heroRuntime == null) heroRuntime = GetComponent<HeroRuntime>();
        }

        private void Start()
        {
            StartCoroutine(ReloadHeroNextFrame());
        }

        public void ReloadHero()
        {
            StartCoroutine(ReloadHeroNextFrame());
        }

        public void ForceReload()
        {
            _isLoaded = false;
            ReloadHero();
        }

        private IEnumerator ReloadHeroNextFrame()
        {
            yield return null;

            if (heroRuntime == null || heroRuntime.HeroDefinition == null)
            {
                Debug.LogWarning("[HeroLoader] HeroRuntime ou HeroDefinition manquant.");
                yield break;
            }

            // ✅ Guard — ne charge qu'une seule fois
            if (_isLoaded)
            {
                ;
                yield break;
            }

            _isLoaded = true;

            DestroyActiveHeroComponents();
            ClearAllSlots();

            string heroId = heroRuntime.HeroDefinition.HeroId;
            ;

            switch (heroId)
            {
                case "byakuya":
                    yield return StartCoroutine(LoadByakuyaNextFrame());
                    break;

                default:
                    Debug.LogWarning($"[HeroLoader] Héros inconnu : {heroId}");
                    break;
            }
        }

        private IEnumerator LoadByakuyaNextFrame()
        {
            var basicAttack = gameObject.AddComponent<ByakuyaBasicAttack>();
            var skillOne = gameObject.AddComponent<ByakuyaSkillOne>();
            var skillTwo = gameObject.AddComponent<ByakuyaSkillTwo>();
            var ultimate = gameObject.AddComponent<ByakuyaUltimate>();
            var combatVfx = gameObject.AddComponent<ByakuyaCombatVFX>();

            yield return null;

            slotBasic.Bind(basicAttack);
            slotSkillOne.Bind(skillOne);
            slotSkillTwo.Bind(skillTwo);
            slotUltimate.Bind(ultimate);

            _activeHeroComponents = new Component[]
            {
                basicAttack, skillOne, skillTwo, ultimate, combatVfx
            };

            ;
            OnHeroLoaded?.Invoke();
        }

        private void DestroyActiveHeroComponents()
        {
            if (_activeHeroComponents == null) return;
            foreach (Component c in _activeHeroComponents)
                if (c != null) Destroy(c);
            _activeHeroComponents = null;
        }

        private void ClearAllSlots()
        {
            slotBasic?.Bind(null);
            slotSkillOne?.Bind(null);
            slotSkillTwo?.Bind(null);
            slotUltimate?.Bind(null);
        }
    }
}