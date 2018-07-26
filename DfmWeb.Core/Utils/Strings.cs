using System;

namespace DfmWeb.Core.Utils
{
    public static class Strings
    {
        /// <summary>
        /// Compares two strings ignoring the case
        /// </summary>
        /// <param name="a">The first string to compare, or null</param>
        /// <param name="b">The second string to compare, or null</param>
        /// <returns>true if the value of the a parameter is equal to the value of the b parameter; otherwise, false.</returns>
        public static bool EqualsNoCase(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }
    }
}
