using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper.StructureMapping
{
    internal sealed class ForeignProperty : ForeignFunction
    {
        private readonly MethodInfo method;

        private readonly string source;

        public ForeignProperty(MethodInfo method, WrenPropertyAttribute propertyAttribute)
        {
            this.method = method;

            if (propertyAttribute.Type == PropertyType.Get)
                source = $"foreign{(method.IsStatic ? " static " : " ")}{propertyAttribute.Name}";
            else
                source = $"foreign{(method.IsStatic ? " static " : " ")}{propertyAttribute.Name}=({propertyAttribute.Argument})";
        }

        internal override string GetSource()
        {
            return source;
        }

        internal override void Invoke(WrenVM vm)
        {
            object instance = null;

            if (!method.IsStatic)
                instance = vm.GetSlotForeign(0);

            method.Invoke(instance, new[] { vm });
        }
    }
}