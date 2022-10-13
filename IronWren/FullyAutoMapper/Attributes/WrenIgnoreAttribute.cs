using System;

namespace IronWren.FullyAutoMapper
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal class WrenIgnoreAttribute : Attribute
    {
    }
}
