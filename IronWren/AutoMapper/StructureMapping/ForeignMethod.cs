using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper.StructureMapping
{
    internal sealed class ForeignMethod : ForeignFunction
    {
        private readonly string source;

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> for the foreign method.
        /// </summary>
        public MethodInfo Method { get; }

        public ForeignMethod(MethodInfo method)
        {
            Method = method;

            source = $"foreign{(method.IsStatic ? " static " : " ")}{method.Name}({string.Join(", ", method.GetParameters().Select(p => p.Name))})";
        }

        internal override string GetSource()
        {
            return source;
        }
    }
}