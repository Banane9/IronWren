using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Contains extension methods for the <see cref="WrenVM"/> class to automatically map C# classes to Wren.
    /// </summary>
    public static class AutoMapper
    {
        /// <summary>
        /// A HashSet of VMs to which the lookup functions for the AutoMapper have been added.
        /// <para/>
        /// HashSet to make lookup O(1) instead of O(n) for lists.
        /// </summary>
        private static readonly HashSet<WrenVM> initializedVMs = new HashSet<WrenVM>();

        /// <summary>
        /// Gets or sets whether a module getting modified after being loaded throws an Exception.
        /// <para/>
        /// On by default.
        /// </summary>
        public static bool TreatModificationAfterLoadAsError { get; set; } = true;

        /// <summary>
        /// Automatically maps the given type to make it accessible from Wren. Optionally places it into a module
        /// other than the <see cref="WrenVM.InterpetModule"/>.
        /// </summary>
        /// <typeparam name="TTarget">The type to map.</typeparam>
        /// <param name="vm">The <see cref="WrenVM"/> to make the type available to.</param>
        /// <param name="module">The name of the module to place the type into.</param>
        public static void AutoMap<TTarget>(this WrenVM vm, string module = WrenVM.InterpetModule)
        {
            vm.AutoMap(module, typeof(TTarget));
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
            checkInitialization(vm);

            foreach (var target in targets.Select(type => type.GetTypeInfo()))
                map(target, module);
        }

        private static void checkInitialization(WrenVM vm)
        {
            if (initializedVMs.Contains(vm))
                return;

            vm.Config.LoadModule += loadAutoMapperModule;
            vm.Config.BindForeignMethod += bindAutoMapperMethod;
            vm.Config.BindForeignClass += bindAutoMapperClass;
        }

        private static void map(TypeInfo target, string module)
        {
            throw new NotImplementedException();
        }

        #region VM Config Methods

        private static WrenForeignClassMethods bindAutoMapperClass(WrenVM vm, string module, string className)
        {
            throw new NotImplementedException();
        }

        private static WrenForeignMethod bindAutoMapperMethod(WrenVM vm, string module, string className, bool isStatic, string signature)
        {
            throw new NotImplementedException();
        }

        private static string loadAutoMapperModule(WrenVM vm, string name)
        {
            throw new NotImplementedException();
        }

        #endregion VM Config Methods
    }
}