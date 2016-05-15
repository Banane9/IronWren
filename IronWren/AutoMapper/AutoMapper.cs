using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Contains extension methods for the <see cref="WrenVM"/> class to automatically map C# classes to Wren.
    /// </summary>
    public static class AutoMapper
    {
        /// <summary>
        /// Automatically maps the given type to make it accessible from Wren. Optionally places it into a module
        /// other than the <see cref="WrenVM.InterpetModule"/>.
        /// </summary>
        /// <typeparam name="TTarget">The type to map.</typeparam>
        /// <param name="vm">The <see cref="WrenVM"/> to make the type available to.</param>
        /// <param name="module">The name of the module to place the type into.</param>
        public static void AutoMap<TTarget>(this WrenVM vm, string module = WrenVM.InterpetModule)
        {
        }

        /// <summary>
        /// Automatically maps the given types to make them accessible from Wren. Optionally places them into a module
        /// other than the <see cref="WrenVM.InterpetModule"/>.
        /// </summary>
        /// <param name="vm">The <see cref="WrenVM"/> to make the types available to.</param>
        /// <param name="module">The name of the module to place the types into.</param>
        /// <param name="targets">The types to map.</param>
        public static void AutoMap(this WrenVM vm, string module = WrenVM.InterpetModule, params Type[] targets)
        {
        }
    }
}