using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Services;

namespace Core.Extensions;

public static partial class StringExtensions
{
    public static MatchCollection ParseBBCode(this string input) => BBCodeRegex.Matches(input);
    public static string RemoveBBCode(this string input) => BBCodeRegex.Replace(input, string.Empty);

    private static readonly Regex BBCodeRegex =
        new(
            pattern: GetPattern(),
            options: RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static string GetPattern()
    {
        string[] codes = [.. BBCodeService.BBCodeActions.Keys];
        StringBuilder patternBuilder = new(@"(\n|\[((");
        patternBuilder.AppendJoin('|', codes);
        patternBuilder.Append(@$")(=(\w*(\.\d)? ?)+)?|\\(");
        patternBuilder.AppendJoin('|', codes);
        patternBuilder.Append(@"))\])");

        return patternBuilder.ToString(); // (\n|\[(({code})(=(\w*(\.\d)? ?)+)?|\\({code}))\])
    }
}