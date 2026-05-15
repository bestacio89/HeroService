using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Encapsule les règles MVP de disponibilité des héros selon le mode de jeu
/// et la collection de héros possédés par le joueur.
/// Ce service est prêt pour un futur branchement à une vraie source de données.
/// </summary>
public class HeroAvailabilityService
{
    public bool IsHeroAvailable(
        string heroId,
        GameModeType mode,
        IReadOnlyCollection<string> ownedHeroIds)
    {
        if (string.IsNullOrWhiteSpace(heroId))
        {
            return false;
        }

        switch (mode)
        {
            case GameModeType.Classic:
            case GameModeType.Ranked:
                return ownedHeroIds != null && ownedHeroIds.Contains(heroId);

            case GameModeType.Arcade:
                return true;

            default:
                return false;
        }
    }

    public List<string> GetAvailableHeroIds(
        IEnumerable<string> allHeroIds,
        GameModeType mode,
        IReadOnlyCollection<string> ownedHeroIds)
    {
        var results = new List<string>();
        var seen = new HashSet<string>();

        if (allHeroIds == null)
        {
            return results;
        }

        foreach (var heroId in allHeroIds)
        {
            if (string.IsNullOrWhiteSpace(heroId))
            {
                continue;
            }

            if (seen.Contains(heroId))
            {
                continue;
            }

            if (IsHeroAvailable(heroId, mode, ownedHeroIds))
            {
                seen.Add(heroId);
                results.Add(heroId);
            }
        }

        return results;
    }
}
