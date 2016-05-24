using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper.StructureMapping
{
    internal static class Signature
    {
        public static string MakeConstructor(int arguments)
        {
            // May have to add construct before new
            return $"new({string.Join(",", Enumerable.Repeat("_", arguments))})";
        }

        public static string MakeIndexer(PropertyType type, string name, int arguments)
        {
            if (type == PropertyType.Get)
                return $"[{string.Join(",", Enumerable.Repeat("_", arguments))}]";

            return $"[{string.Join(",", Enumerable.Repeat("_", arguments))}]=(_)";
        }

        public static string MakeMethod(string name, int arguments)
        {
            return $"{name}({string.Join(",", Enumerable.Repeat("_", arguments))})";
        }

        public static string MakeProperty(PropertyType type, string name)
        {
            if (type == PropertyType.Get)
                return name;

            return $"{name}=(_)";
        }
    }
}