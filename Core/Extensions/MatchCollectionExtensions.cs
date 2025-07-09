using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Extensions;

public static class MatchCollectionExtensions
{
    public static bool IsIndexWithin(this MatchCollection matchCollection, int index, string code)
    {
        // Find Start and End Matches //
        var startMatches = matchCollection.Where(match => match.Value.StartsWith('[' + code));
        var endMatches = matchCollection.Where(match => match.Value.StartsWith("[\\" + code));
        // Conditions //
        if (!startMatches.Any() || !endMatches.Any())
            return false; // If no start or end matches found, return false //
        // Sub-Methods //
        Match? findClosestStartMatch(int givenIndex)
        {
            foreach ((int index, Match match) in startMatches.Index())
            {
                // Conditions //
                if (match.Index < givenIndex)
                    continue;
                // Found Closest //
                return startMatches.ElementAt((index - 1).Clamp0(startMatches.Count() - 1));
            }
            return startMatches.FirstOrDefault();
        }
        Match? findClosestEndMatch(int givenIndex)
        {
            foreach ((int index, Match match) in endMatches.Index().Reverse())
            {
                // Conditions //
                if (match.Index > givenIndex)
                    continue;
                // Found Closest //
                return endMatches.ElementAt((index - 1).Clamp0(endMatches.Count() - 1));
            }
            return endMatches.FirstOrDefault();
        }
        // Data //
        Match? startMatch = findClosestStartMatch(index);
        Match? endMatch = findClosestEndMatch(index);
        // Conditions //
        if (startMatch is null)
            return false; // If both matches are null, return false //
        if (startMatch is not null && endMatch is null)
            return true;
        if (startMatch is not null && endMatch is null)
            return false;
        return startMatch?.Index <= index && index <= endMatch?.Index + endMatch?.Length - 1;
    }

    public static Match? FindClosestPreviousMatch(this MatchCollection matchCollection, int givenIndex)
    {
        return matchCollection
            .LastOrDefault(match => match.Index < givenIndex);
    }

    public static Match? FindClosestNextMatch(this MatchCollection matchCollection, int givenIndex)
    {
        return matchCollection
            .FirstOrDefault(match => match.Index > givenIndex);
    }

    public static Match? FindClosestPreviousStartingMatch(this MatchCollection matchCollection, int givenIndex, string code)
    {
        return matchCollection
            .LastOrDefault(match => match.Index < givenIndex && match.Value.StartsWith('[' + code));
    }
    public static Match? FindNextEndingMatch(this MatchCollection matchCollection, int givenIndex, string code)
    {
        return matchCollection
            .FirstOrDefault(match => match.Index > givenIndex && match.Value.StartsWith("[\\" + code));
    }
    public static bool IsIndexInMatch(this MatchCollection matchCollection, int index)
    {
        // Iterate Matches //
        foreach (Match match in matchCollection)
        {
            // Conditions //
            if (match.Index > index)
                return false; // If Start Index is past Index //
            if (match.Index + match.Length - 1 < index)
                continue; // If End of Match is before Index //
            // Success //
            return true;
        }
        // Fail //
        return false;
    }
}
