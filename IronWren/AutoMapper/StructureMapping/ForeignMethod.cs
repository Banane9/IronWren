using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace IronWren.AutoMapper.StructureMapping
{
    internal sealed class ForeignMethod : ForeignFunction
    {
        /// <summary>
        /// Gets the <see cref="MethodInfo"/> for the foreign method.
        /// </summary>
        private readonly MethodInfo Method;

        private readonly string source;

        public ForeignMethod(MethodInfo method, WrenMethodAttribute methodAttribute)
        {
            Method = method;

            source = $"foreign{(method.IsStatic ? " static " : " ")}{methodAttribute.Name}({string.Join(", ", methodAttribute.Arguments)})";
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

            if (!Method.IsStatic)
            {
                var id = Marshal.ReadInt32(vm.GetSlotForeign(0));
                instance = AutoMapper.AllocatedObjects[vm][id];
            }

            Method.Invoke(instance, new[] { vm });
        }
    }
}