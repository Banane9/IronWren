using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper.StructureMapping
{
    internal sealed class ForeignField : ForeignFunction
    {
        private readonly FieldInfo field;

        private readonly string source;

        public ForeignField(FieldInfo field, PropertyType type)
        {
            this.field = field;

            if (type == PropertyType.Get)
                source = $"foreign{(field.IsStatic ? " static " : " ")}{field.Name}";
            else
                source = $"foreign{(field.IsStatic ? " static " : " ")}{field.Name}=(value)";
        }

        internal override string GetSource()
        {
            return source;
        }
    }
}