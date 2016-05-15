using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Contains extension methods for the <see cref="WrenVM"/> class to automatically map C# classes to Wren.
    /// </summary>
    public static class AutoMapper
    {
        /// <summary>
        /// Dictionary of generated modules mapped to their names.
        /// </summary>
        private static readonly Dictionary<string, Module> generatedModules = new Dictionary<string, Module>();

        /// <summary>
        /// HashSet of VMs to which the lookup functions for the AutoMapper have been added.
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
        /// Automatically maps the given type to make its public interface accessible from Wren. Optionally places it into a module
        /// other than the <see cref="WrenVM.InterpetModule"/>.
        /// <para/>
        /// If no other module is specified, the generated code will be interpreted immediately.
        /// </summary>
        /// <typeparam name="TTarget">The type to map.</typeparam>
        /// <param name="vm">The <see cref="WrenVM"/> to make the type available to.</param>
        /// <param name="moduleName">The name of the module to place the type into.</param>
        public static void AutoMap<TTarget>(this WrenVM vm, string moduleName = WrenVM.InterpetModule)
        {
            vm.AutoMap(moduleName, typeof(TTarget));
        }

        /// <summary>
        /// Automatically maps the given types to make their public interfaces accessible from Wren. Optionally places them into a module
        /// other than the <see cref="WrenVM.InterpetModule"/>.
        /// <para/>
        /// If no other module is specified, the generated code will be interpreted immediately.
        /// </summary>
        /// <param name="vm">The <see cref="WrenVM"/> to make the types available to.</param>
        /// <param name="moduleName">The name of the module to place the types into.</param>
        /// <param name="targets">The types to map.</param>
        public static void AutoMap(this WrenVM vm, string moduleName = WrenVM.InterpetModule, params Type[] targets)
        {
            checkInitialization(vm);

            StringBuilder source;
            if (moduleName != WrenVM.InterpetModule)
            {
                var module = generatedModules.ContainsKey(moduleName) ? generatedModules[moduleName] : new Module();
                source = module.Source;
            }
            else
                source = new StringBuilder();

            foreach (var target in targets.Select(type => type.GetTypeInfo()))
                map(target, source);

            if (moduleName == WrenVM.InterpetModule)
                vm.Interpret(source.ToString());
        }

        private static void checkInitialization(WrenVM vm)
        {
            if (initializedVMs.Contains(vm))
                return;

            vm.Config.LoadModule += loadAutoMapperModule;
            vm.Config.BindForeignMethod += bindAutoMapperMethod;
            vm.Config.BindForeignClass += bindAutoMapperClass;
        }

        private static StringBuilder map(TypeInfo target, StringBuilder source)
        {
            // TODO: Inheritance?
            source.AppendLine($"foreign class {target.Name} {{");

            mapConstants(target, source);
            mapProperties(target, source);
            mapConstructors(target, source);
            mapMethods(target, source);
            // TODO: Map Events?

            source.AppendLine("}");

            return source;
        }

        #region Mappers

        private static void mapConstants(TypeInfo target, StringBuilder source)
        {
            throw new NotImplementedException();
        }

        private static void mapConstructors(TypeInfo target, StringBuilder source)
        {
            throw new NotImplementedException();
        }

        private static void mapMethods(TypeInfo target, StringBuilder source)
        {
            throw new NotImplementedException();
        }

        private static void mapProperties(TypeInfo target, StringBuilder source)
        {
            throw new NotImplementedException();
        }

        #endregion Mappers

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
            if (!generatedModules.ContainsKey(name))
                return null;

            return generatedModules[name].GetSource();
        }

        #endregion VM Config Methods
    }
}