using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Extensions;

public static partial class StringExtensions
{
    public static MatchCollection ParseBBCode(this string input) => BBCodeRegex().Matches(input);
    public static string RemoveBBCode(this string input) => BBCodeRegex().Replace(input, string.Empty);

    [GeneratedRegex(
        @"
(\[b\]
|\[\\b\]
|\[i\]
|\[\\i\]
|\[u\]
|\[\\u\]
|\[size=\d+\]
|\[\\size\]
|\n
)
", RegexOptions.IgnorePatternWhitespace)]
    private static partial Regex BBCodeRegex();
}

// [size=30]
// |[size=\d]