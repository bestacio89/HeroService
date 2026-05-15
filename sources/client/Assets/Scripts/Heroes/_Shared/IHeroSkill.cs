// Scripts/Heroes/_Shared/IHeroSkill.cs

namespace MobaPrototype.Heroes
{
    /// <summary>
    /// Contrat commun pour toutes les compétences de héros.
    /// Chaque héros implémente cette interface pour ses 4 compétences.
    /// </summary>
    public interface IHeroSkill
    {
        void TryCast();
        float CooldownRemaining { get; }
        float CooldownMax { get; }
        bool IsReady { get; }
    }
}