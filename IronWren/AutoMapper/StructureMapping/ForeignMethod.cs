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

        public ForeignMethod(MethodInfo method, WrenMethodAttribute methodAttribute)
        {
            Method = method;

            source = $"foreign{(method.IsStatic ? " static " : " ")}{methodAttribute.Name}({string.Join(", ", methodAttribute.Arguments)})";
        }

        internal override string GetSource()
        {
            return source;
        }
    }
}