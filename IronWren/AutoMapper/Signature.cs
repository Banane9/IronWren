using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Contains methods to generate Wren signatures for class elements.
    /// </summary>
    public static class Signature
    {
        /// <summary>
        /// Generates an indexer signature from the given details.
        /// </summary>
        /// <param name="type">Whether it's a getter or setter indexer.</param>
        /// <param name="arguments">The number of arguments that the indexer has.</param>
        /// <returns>The signature of the indexer.</returns>
        public static string MakeIndexer(PropertyType type, int arguments)
        {
            if (type == PropertyType.Get)
                return $"[{string.Join(",", Enumerable.Repeat("_", arguments))}]";

            return $"[{string.Join(",", Enumerable.Repeat("_", arguments))}]=(_)";
        }

        /// <summary>
        /// Generates a method signature from the given details.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <param name="arguments">Th number of arguments that the method has.</param>
        /// <returns>The signature of the method.</returns>
        public static string MakeMethod(string name, int arguments)
        {
            return $"{name}({string.Join(",", Enumerable.Repeat("_", arguments))})";
        }

        /// <summary>
        /// Generates a property signature from the given details.
        /// </summary>
        /// <param name="type">Whether it's a getter or setter property.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The signature of the property.</returns>
        public static string MakeProperty(PropertyType type, string name)
        {
            if (type == PropertyType.Get)
                return name;

            return $"{name}=(_)";
        }
    }
}