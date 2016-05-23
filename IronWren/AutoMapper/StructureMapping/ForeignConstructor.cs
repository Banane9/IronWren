using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper.StructureMapping
{
    internal sealed class ForeignConstructor : ForeignFunction
    {
        private readonly string source;

        /// <summary>
        /// Gets the <see cref="ConstructorInfo"/> for the foreign constructor.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        public ForeignConstructor(ConstructorInfo constructor, string[] arguments)
        {
            Constructor = constructor;

            source = $"construct new({string.Join(", ", arguments)}) {{ }}";
        }

        internal override string GetSource()
        {
            return source;
        }
    }
}