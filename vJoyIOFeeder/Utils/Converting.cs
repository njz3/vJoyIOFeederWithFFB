using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BackForceFeeder.Utils
{
    public static class Converting
    {
        public static bool HexStrToInt(string hex, out int value)
        {
            return int.TryParse(hex, NumberStyles.AllowHexSpecifier|NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }

        // https://stackoverflow.com/questions/30299671/matching-strings-with-wildcard/30300521
        public static String WildCardToRegex(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }
    }
}
