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
        public static double Normalize12b(uint value12bits)
        {
            // Scale 12bits input to 0.0 ... 1.0
            return (double)(value12bits) * (1.0 / (double)0xFFF);
        }

        public static double Normalize16b(uint value16bits)
        {
            // Scale 16bits input to 0.0 ... 1.0
            return (double)(value16bits) * (1.0 / (double)0xFFFF);
        }
    }
}
