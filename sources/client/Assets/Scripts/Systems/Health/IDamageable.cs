// Scripts/Systems/Health/IDamageable.cs
using UnityEngine;
using MobaPrototype.Managers; // pour TeamId

namespace MobaPrototype.Systems.Health
{
    /// <summary>
    /// Contrat commun à tout ce qui peut recevoir des dégâts et être ciblé
    /// par le combat : les héros (HealthSystem) ET les structures (StructureHealth).
    ///
    /// Grâce à cette interface, le ciblage n'a plus besoin de demander
    /// "es-tu un héros ?" (en cherchant un HeroRuntime). Il demande seulement
    /// "es-tu attaquable, et de quelle équipe es-tu ?". Un Core peut donc être
    /// ciblé comme n'importe quel ennemi, sans être un héros.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>Équipe de la cible — sert à distinguer allié et ennemi.</summary>
        TeamId Team { get; }

        /// <summary>Vrai si la cible est déjà détruite/morte (le ciblage doit l'ignorer).</summary>
        bool IsDead { get; }

        /// <summary>Transform de la cible — pour calculer distance et position.</summary>
        Transform Transform { get; }

        /// <summary>Inflige des dégâts à la cible.</summary>
        void TakeDamage(float amount);
    }
}
