using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        internal override string GetSource()
        {
            return source;
        }

        internal override void Invoke(WrenVM vm)
        {
            object instance = null;

            if (!Method.IsStatic)
                instance = vm.GetSlotForeign(0);

            Method.Invoke(instance, new[] { vm });
        }
    }
}