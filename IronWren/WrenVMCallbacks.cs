using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronWren
{
    public sealed partial class WrenVM : SafeHandleZeroOrMinusOneIsInvalid
    {
        private readonly Dictionary<WrenForeignClassMethods, WrenForeignClassMethodsInternal> classMethods =
            new Dictionary<WrenForeignClassMethods, WrenForeignClassMethodsInternal>();

        internal WrenForeignClassMethodsInternal bindForeignClass(IntPtr vm, string module, string className)
        {
            ensureCorrectVM(vm);

            if (BindForeignClass == null)
                return new WrenForeignClassMethodsInternal();

            // Only one of the multiple possible BindForeignClass implementations must actually return the ForeignClassMethods
            var results = BindForeignClass.GetInvocationList().Cast<WrenBindForeignClass>()
                .Select(bindForeignClass => bindForeignClass(this, module, className));

            var result = results.Single(res => res != null);

            // Have to save struct, so the delegates aren't GCed
            var methods = new WrenForeignClassMethodsInternal(result);
            classMethods.Add(result, methods);

            return methods;
        }

        internal WrenForeignMethodInternal bindForeignMethod(IntPtr vm, string module, string className, bool isStatic, string signature)
        {
            ensureCorrectVM(vm);

            // Only one of the multiple possible BindForeignMethod implementations must actually return a method.
            var result = BindForeignMethod?.GetInvocationList().Cast<WrenBindForeignMethod>()
                .Select(bindForeignMethod => bindForeignMethod(this, module, className, isStatic, signature))
                .SingleOrDefault(res => res != null);

            if (result == null)
                return null;

            // Have to save the delegate so it isn't GCed
            return PreserveForeignMethod(result);
        }

        internal void error(IntPtr vm, WrenErrorType type, string module, int line, string message)
        {
            ensureCorrectVM(vm);

            Error?.Invoke(this, type, module, line, message);
        }

        internal WrenLoadModuleResultInternal loadModule(IntPtr vm, string name)
        {
            ensureCorrectVM(vm);

            // Only one of the multiple possible LoadModule implementations must actually return the source for the module
            var externalResult = LoadModule?.GetInvocationList().Cast<WrenLoadModule>()
                .Select(loadModule => loadModule(this, name))
                .FirstOrDefault(res => !string.IsNullOrWhiteSpace(res?.Source));

            if (externalResult == null)
                return default;

            return externalResult.GetStruct();
        }

        internal string resolveModule(IntPtr vm, string importer, string name)
        {
            ensureCorrectVM(vm);

            // if no ResolveModule subscriber resolves the name, return the input
            string result = name;

            if (ResolveModule == null)
                return name;

            var resolveResult = ResolveModule.GetInvocationList().Cast<WrenResolveModule>()
                .Select(resolveModule => resolveModule(this, importer, name))
                .FirstOrDefault(res => res != null);

            if (resolveResult == null)
                return name;

            return resolveResult;
        }

        internal void write(IntPtr vm, string text)
        {
            ensureCorrectVM(vm);

            Write?.Invoke(this, text);
        }

        /// <summary>
        /// The callback Wren uses to find a foreign class and get its foreign methods.
        /// <para/>
        /// When a foreign class is declared, this will be called with the class's
        /// module and name when the class body is executed. It should return the
        /// foreign functions uses to allocate and (optionally) finalize the bytes
        /// stored in the foreign object when an instance is created.
        /// <para/>
        /// Only one of the multiple possible BindForeignClass implementations must actually return the <see cref="WrenForeignClassMethods"/>.
        /// </summary>
        public event WrenBindForeignClass BindForeignClass;

        /// <summary>
        /// The callback Wren uses to find a foreign method and bind it to a class.
        /// <para/>
        /// When a foreign method is declared in a class, this will be called with the
        /// foreign method's module, class, and signature when the class body is
        /// executed. It should return a pointer to the foreign function that will be
        /// bound to that method.
        /// <para/>
        /// If the foreign function could not be found, this should return NULL and
        /// Wren will report it as runtime error.
        /// <para/>
        /// Only one of the multiple possible BindForeignMethod implementations must actually return a method.
        /// </summary>
        public event WrenBindForeignMethod BindForeignMethod;

        /// <summary>
        /// The callback Wren uses to report errors.
        /// <para/>
        /// When an error occurs, this will be called with the module name, line
        /// number, and an error message. If this is `NULL`, Wren doesn't report any
        /// errors.
        /// </summary>
        public event WrenError Error;

        /// <summary>
        /// The callback Wren uses to load a module.
        /// <para/>
        /// Since Wren does not talk directly to the file system, it relies on the
        /// embedder to physically locate and read the source code for a module. The
        /// first time an import appears, Wren will call this and pass in the name of
        /// the module being imported. The VM should return the soure code for that and
        /// Wren will take ownership over it.
        /// <para/>
        /// This will only be called once for any given module name. Wren caches the
        /// result internally so subsequent imports of the same module will use the
        /// previous source and not call this.
        /// <para/>
        /// If a module with the given name could not be found by the embedder, it
        /// should return NULL and Wren will report that as a runtime error.
        /// <para/>
        /// Only one of the multiple possible LoadModule implementations must actually return the source for the module.
        /// </summary>
        public event WrenLoadModule LoadModule;

        /// <summary>
        /// The callback Wren uses to resolve a module.
        /// <para/>
        /// Gives the host a chance to canonicalize the imported module name,
        /// potentially taking into account the (previously resolved) name of the module
        /// that contains the import. Typically, this is used to implement relative
        /// imports.
        /// </summary>
        public event WrenResolveModule ResolveModule;

        /// <summary>
        /// The callback Wren uses to display text when `System.print()` or the other
        /// related functions are called.
        /// <para/>
        /// If this is `NULL`, Wren discards any printed text.
        /// </summary>
        public event WrenWrite Write;
    }
}