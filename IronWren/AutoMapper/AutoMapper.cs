using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Contains extension methods for the <see cref="WrenVM"/> class to automatically map attribute-decorated C# classes to Wren.
    /// </summary>
    public static class AutoMapper
    {
        /// <summary>
        /// Dictionary of generated modules mapped to their names and VMs.
        /// <para/>
        /// Also used for the VMs to which the lookup functions for the AutoMapper have been added.
        /// </summary>
        private static readonly ConditionalWeakTable<WrenVM, Dictionary<string, ForeignModule>> generatedModules =
            new ConditionalWeakTable<WrenVM, Dictionary<string, ForeignModule>>();

        private static readonly ConditionalWeakTable<WrenVM, Dictionary<string, ForeignClass>> mainModuleClasses =
            new ConditionalWeakTable<WrenVM, Dictionary<string, ForeignClass>>();

        /// <summary>
        /// Gets or sets whether a module getting modified after being loaded throws a <see cref="LoadedModuleModifiedException"/>.
        /// <para/>
        /// On by default.
        /// </summary>
        public static bool TreatModificationAfterLoadAsError { get; set; } = true;

        /// <summary>
        /// Automatically maps the given type to make its marked interface accessible from Wren.
        /// Optionally places it into a module other than the <see cref="WrenVM.MainModule"/>.
        /// <para/>
        /// If no other module is specified, the generated code will be interpreted immediately.
        /// </summary>
        /// <typeparam name="TTarget">The type to map.</typeparam>
        /// <param name="vm">The <see cref="WrenVM"/> to make the type available to.</param>
        /// <param name="moduleName">The name of the module to place the type into.</param>
        public static void AutoMap<TTarget>(this WrenVM vm, string moduleName = WrenVM.MainModule)
        {
            vm.AutoMap(moduleName, typeof(TTarget));
        }

        /// <summary>
        /// Automatically maps the given types to make their marked interfaces accessible from Wren.
        /// Places them into the <see cref="WrenVM.MainModule"/> where the generated code will be interpreted immediately.
        /// </summary>
        /// <param name="vm">The <see cref="WrenVM"/> to make the types available to.</param>
        /// <param name="targets">The types to map.</param>
        public static void AutoMap(this WrenVM vm, params Type[] targets)
        {
            vm.AutoMap(WrenVM.MainModule, targets);
        }

        /// <summary>
        /// Automatically maps the given types to make their marked interfaces accessible from Wren.
        /// If the module name is <see cref="WrenVM.MainModule"/>, the code will be interpreted immediately.
        /// </summary>
        /// <param name="vm">The <see cref="WrenVM"/> to make the types available to.</param>
        /// <param name="moduleName">The name of the module to place the types into.</param>
        /// <param name="targets">The types to map.</param>
        public static void AutoMap(this WrenVM vm, string moduleName, params Type[] targets)
        {
            checkInitialization(vm);

            if (moduleName == WrenVM.MainModule)
            {
                // Will never fail due to setup in checkInitialization
                mainModuleClasses.TryGetValue(vm, out var classes);

                foreach (var target in targets)
                {
                    var foreignClass = new ForeignClass(target);

                    classes.Add(foreignClass.Name, foreignClass);

                    vm.Interpret(moduleName, foreignClass.Source);
                }

                return;
            }

            // Will never fail due to setup in checkInitialization
            generatedModules.TryGetValue(vm, out var modules);

            if (modules.TryGetValue(moduleName, out var module))
            {
                if (module.Used && TreatModificationAfterLoadAsError)
                    ThrowHelper.ThrowLoadedModuleModifiedException(moduleName);
            }
            else
            {
                module = new ForeignModule();
                modules.Add(moduleName, module);
            }

            foreach (var target in targets)
                module.Add(target);
        }

        private static void checkInitialization(WrenVM vm)
        {
            if (generatedModules.TryGetValue(vm, out var module))
                return;

            vm.LoadModule += loadAutoMapperModule;
            vm.BindForeignMethod += bindAutoMapperMethod;
            vm.BindForeignClass += bindAutoMapperClass;

            generatedModules.Add(vm, new Dictionary<string, ForeignModule>());
            mainModuleClasses.Add(vm, new Dictionary<string, ForeignClass>());
        }

        #region VM Config Methods

        private static WrenForeignClassMethods bindAutoMapperClass(WrenVM vm, string module, string className)
        {
            Dictionary<string, ForeignClass> classes;
            if (module == WrenVM.MainModule && mainModuleClasses.TryGetValue(vm, out classes))
                return classes?[className]?.Bind();

            Dictionary<string, ForeignModule> modules;
            if (generatedModules.TryGetValue(vm, out modules))
                return modules?[module]?.Classes?[className]?.Bind();

            return null;
        }

        private static WrenForeignMethod bindAutoMapperMethod(WrenVM vm, string module, string className, bool isStatic, string signature)
        {
            Dictionary<string, ForeignClass> classes;
            if (module == WrenVM.MainModule && mainModuleClasses.TryGetValue(vm, out classes))
                return classes?[className]?.Functions?[signature];

            Dictionary<string, ForeignModule> modules;
            if (generatedModules.TryGetValue(vm, out modules))
                return modules?[module]?.Classes?[className]?.Functions?[signature];

            return null;
        }

        private static WrenLoadModuleResult loadAutoMapperModule(WrenVM vm, string name)
        {
            Dictionary<string, ForeignModule> modules;
            if (generatedModules.TryGetValue(vm, out modules))
                return new WrenLoadModuleResult { Source = modules?[name]?.GetSource() };

            return null;
        }

        #endregion VM Config Methods
    }
}