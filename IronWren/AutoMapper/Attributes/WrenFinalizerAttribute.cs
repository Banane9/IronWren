using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Required attribute that marks a function as a class's finalizer.
    /// <para/>
    /// Must have no arguments and a return type of void.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class WrenFinalizerAttribute : Attribute
    { }
}