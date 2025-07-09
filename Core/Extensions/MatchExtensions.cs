using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Extensions;

public static class MatchExtensions
{
    public static string? GetInternalString(this Match match) => string.Concat(match.Value.SkipWhile(letter => letter is not '=').Skip(1).SkipLast(1));
}
