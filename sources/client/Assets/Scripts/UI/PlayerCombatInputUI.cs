using UnityEngine;
using MobaPrototype.Player;

namespace UI
{
    /// <summary>
    /// Relie les boutons UI de combat aux actions du joueur.
    /// </summary>
    public sealed class PlayerCombatInputUI : MonoBehaviour
    {
        [Header("Les 4 slots fixes — assignés dans l'Inspector ou via SetSlots()")]
        [SerializeField] private HeroSkillSlot slotBasic;
        [SerializeField] private HeroSkillSlot slotSkillOne;
        [SerializeField] private HeroSkillSlot slotSkillTwo;
        [SerializeField] private HeroSkillSlot slotUltimate;

        public void SetSlots(
            HeroSkillSlot basic,
            HeroSkillSlot skillOne,
            HeroSkillSlot skillTwo,
            HeroSkillSlot ultimate)
        {
            slotBasic = basic;
            slotSkillOne = skillOne;
            slotSkillTwo = skillTwo;
            slotUltimate = ultimate;
        }

        public void OnAttackPressed()
        {
            if (slotBasic == null)
            {
                Debug.LogWarning("[PlayerCombatInputUI] slotBasic non assigné.", this);
                return;
            }

            slotBasic.TryCast();
        }

        public void OnSkillOnePressed()
        {
            if (slotSkillOne == null)
            {
                Debug.LogWarning("[PlayerCombatInputUI] slotSkillOne non assigné.", this);
                return;
            }

            slotSkillOne.TryCast();
        }

        public void OnSkillTwoPressed()
        {
            if (slotSkillTwo == null)
            {
                Debug.LogWarning("[PlayerCombatInputUI] slotSkillTwo non assigné.", this);
                return;
            }

            slotSkillTwo.TryCast();
        }

        public void OnUltimatePressed()
        {
            if (slotUltimate == null)
            {
                Debug.LogWarning("[PlayerCombatInputUI] slotUltimate non assigné.", this);
                return;
            }

            slotUltimate.TryCast();
        }
    }
}