using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper.StructureMapping
{
    internal sealed class ForeignIndexer : ForeignFunction
    {
        private readonly MethodInfo method;

        private readonly string source;

        public ForeignIndexer(MethodInfo method, WrenIndexerAttribute indexerAttribute)
        {
            this.method = method;

            if (indexerAttribute.Type == PropertyType.Get)
                source = $"foreign{(method.IsStatic ? " static " : " ")}[{string.Join(", ", indexerAttribute.Arguments)}]";
            else
                source = $"foreign{(method.IsStatic ? " static " : " ")}[{string.Join(", ", indexerAttribute.Arguments)}]=({indexerAttribute.Argument})";
        }

        internal override string GetSource()
        {
            return source;
        }
    }
}