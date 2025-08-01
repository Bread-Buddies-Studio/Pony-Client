using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Extensions;

public static class MatchExtensions
{
    public static string? GetInternalString(this Match match)
    {
        // Conditions //
        if (!match.Value.Contains('='))
            return null;
        // Success //
        return match.Value[(match.Value.IndexOf('=') + 1)..^1];
        
    }

    public static string GetCode(this Match match)
    {
        if (match.Value.Contains('='))
            return match.Value[1..(match.Value.IndexOf('=') - 1)];
        if (match.Value.Contains('\\'))
            return match.Value[2..^1];
        return match.Value[1..^1];
    }
}
