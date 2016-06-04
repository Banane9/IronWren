using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper
{
    internal sealed class MethodDetails<TAttribute> where TAttribute : Attribute
    {
        public TAttribute Attribute { get; }
        public MethodInfo Info { get; }

        public MethodDetails(MethodInfo info, TAttribute attribute)
        {
            Info = info;
            Attribute = attribute;
        }
    }
}