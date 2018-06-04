using System;

namespace Arragro.Core.Common.Helpers
{
    public static class StringHelpers
    {
        public static string ToCamelCase(this string value)
        {
            return Char.ToLowerInvariant(value[0]) + value.Substring(1);
        }
        public static string ToCamelCaseFromDotNotation(this string value)
        {
            var split = value.Split('.');
            for (var i = 0; i < split.Length; i++)
            {
                split[i] = Char.ToLowerInvariant(split[i][0]) + split[i].Substring(1);
            }
            return String.Join(".", split);
        }
    }
}
