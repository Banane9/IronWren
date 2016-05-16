using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper.StructureMapping
{
    internal sealed class ForeignConstructor : ForeignFunction
    {
        private readonly ConstructorInfo constructor;

        private readonly string source;

        public ForeignConstructor(ConstructorInfo constructor)
        {
            this.constructor = constructor;

            source = $"construct new({string.Join(", ", constructor.GetParameters().Select(p => p.Name))}) {{ }}";
        }

        internal override string GetSource()
        {
            return source;
        }
    }
}