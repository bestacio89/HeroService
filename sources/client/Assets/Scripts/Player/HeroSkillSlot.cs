using UnityEngine;
using MobaPrototype.Heroes;

namespace MobaPrototype.Player
{
    /// <summary>
    /// Relie une action UI à une compétence de héros.
    /// </summary>
    public class HeroSkillSlot : MonoBehaviour
    {
        private IHeroSkill _skill;

        public void Bind(IHeroSkill skill)
        {
            _skill = skill;
        }

        public void TryCast()
        {
            if (_skill == null)
            {
                Debug.LogWarning($"[HeroSkillSlot] Aucune compétence liée sur {gameObject.name}", this);
                return;
            }

            _skill.TryCast();
        }

        public float CooldownRemaining => _skill?.CooldownRemaining ?? 0f;
        public float CooldownMax => _skill?.CooldownMax ?? 1f;
        public bool IsReady => _skill?.IsReady ?? false;
        public bool IsBound => _skill != null;
    }
}