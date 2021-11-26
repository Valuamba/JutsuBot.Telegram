using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Htlv.Parser.Extensions
{
    public static class StringExtensions
    {
        public static bool IsMatchPatter(this string str, string pattern) =>
           str?.Contains(
               pattern,
               StringComparison.Ordinal) ?? false;

        public static string TrimStringPattern(this string str, string pattern) =>
            str?.Replace(pattern, string.Empty);
    }
}
