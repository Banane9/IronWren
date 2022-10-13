using System;
using System.Collections.Generic;
using System.Text;

namespace IronWren.FullyAutoMapper
{
    internal static class StringCase
    {
        public static string ToCamelCase(string name)
        {
            return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
        }

        public static string ToPascalCase(string name)
        {
            return name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
        }
    }
}
