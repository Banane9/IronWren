using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

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

        internal override WrenForeignMethod Bind()
        {
            return invoke;
        }

        internal override string GetSource()
        {
            return source;
        }

        private void invoke(WrenVM vm)
        {
            object instance = null;

            if (!method.IsStatic)
            {
                var id = Marshal.ReadInt32(vm.GetSlotForeign(0));
                instance = AutoMapper.AllocatedObjects[vm][id];
            }

            method.Invoke(instance, new[] { vm });
        }
    }
}