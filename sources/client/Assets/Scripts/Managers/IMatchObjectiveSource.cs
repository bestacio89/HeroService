namespace MobaPrototype.Managers
{
    /// <summary>
    /// Contrat minimal pour toute source d'objectif capable
    /// de provoquer une fin de match.
    /// </summary>
    public interface IMatchObjectiveSource
    {
        MatchResult VictoryResult { get; }

        void TriggerObjectiveResolved();
    }
}
