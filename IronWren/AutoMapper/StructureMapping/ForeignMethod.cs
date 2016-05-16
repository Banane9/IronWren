using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper.StructureMapping
{
    internal sealed class ForeignMethod : ForeignFunction
    {
        private readonly MethodInfo method;

        private readonly string source;

        public ForeignMethod(MethodInfo method)
        {
            this.method = method;

            source = $"foreign{(method.IsStatic ? " static " : " ")}{method.Name}({string.Join(", ", method.GetParameters().Select(p => p.Name))})";
        }

        internal override string GetSource()
        {
            return source;
        }
    }
}