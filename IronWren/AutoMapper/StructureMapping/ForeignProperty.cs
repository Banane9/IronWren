using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper.StructureMapping
{
    internal sealed class ForeignProperty : ForeignFunction
    {
        private readonly bool isIndexer;
        private readonly PropertyInfo property;

        private readonly string source;

        public ForeignProperty(PropertyInfo property, PropertyType type)
        {
            this.property = property;

            var indexParameters = property.GetIndexParameters().Select(p => p.Name).ToArray();
            if (indexParameters.Length > 0)
            {
                isIndexer = true;

                if (type == PropertyType.Get)
                    source = $"foreign{(property.GetMethod.IsStatic ? " static " : " ")}[{string.Join(", ", indexParameters)}]";
                else
                    source = $"foreign{(property.SetMethod.IsStatic ? " static " : " ")}[{string.Join(", ", indexParameters)}]=(value)";
            }
            else
            {
                if (type == PropertyType.Get)
                    source = $"foreign{(property.GetMethod.IsStatic ? " static " : " ")}{property.Name}";
                else
                    source = $"foreign{(property.GetMethod.IsStatic ? " static " : " ")}{property.Name}=(value)";
            }
        }

        internal override string GetSource()
        {
            return source;
        }
    }
}