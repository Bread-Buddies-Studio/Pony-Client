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
        var startMatch = matchCollection
            .LastOrDefault(match =>
            (match.Value == $"[{code}]" || match.Value.StartsWith($"[{code}="))
                && match.Index < index);

        var previousEndMatch = matchCollection
            .LastOrDefault(match =>
            (match.Value == $"[\\{code}]" || match.Value.StartsWith($"[{code}="))
                && match.Index < index);
        
        var endMatch = matchCollection
            .FirstOrDefault(match => 
            (match.Value == $"[\\{code}]" || match.Value.StartsWith($"[{code}="))
                && match.Index > index);
        
        var nextStartMatch = matchCollection
            .FirstOrDefault(match => 
            (match.Value == $"[{code}]" || match.Value.StartsWith($"[{code}="))
                && match.Index > index);
        // Conditions //
        if (startMatch is null || endMatch is null)
            return false; // If no start or end matches found, return false //
        if (previousEndMatch is not null && startMatch.Index < previousEndMatch.Index)
            return false; // If the start match is before the previous end match, return false //
        if (nextStartMatch is not null && nextStartMatch.Index < endMatch.Index)
            return false; // If the next start match is before the end match, return false //
        // Success //
        return true;
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
            if (match is null)
                return false;
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
