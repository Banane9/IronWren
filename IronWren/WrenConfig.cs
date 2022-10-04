using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Stop CodeMaid from reorganizing the file
#if DEBUG
#endif

namespace IronWren
{
    /// <summary>
    /// Represents the configuration used by the <see cref="WrenVM"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class WrenConfig
    {
        [MarshalAs(UnmanagedType.FunctionPtr)]
        private readonly WrenReallocate reallocate;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        private readonly WrenResolveModuleInternal resolveModule;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        private readonly WrenLoadModuleInternal loadModule;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        private readonly WrenBindForeignMethodInternal bindForeignMethod;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        private readonly WrenBindForeignClassInternal bindForeignClass;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        private readonly WrenWriteInternal write;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        private readonly WrenErrorInternal error;

        private uint initialHeapSize;

        private uint minHeapSize;

        private int heapGrowthPercent;

        private readonly IntPtr userData;

        /// <summary>
        /// Gets whether this <see cref="WrenConfig"/> has been used to create a VM already.
        /// <para/>
        /// Some values can't be changed anymore after it has been used, as it won't have any effect.
        /// </summary>
        public bool Used { get; private set; }

        /// <summary>
        /// The callback Wren will use to allocate, reallocate, and deallocate memory.
        /// </summary>
        internal WrenReallocate Reallocate
        {
            get { return reallocate; }
        }

        /// <summary>
        /// The callback Wren uses to resolve a module.
        /// <para/>
        /// Gives the host a chance to canonicalize the imported module name,
        /// potentially taking into account the (previously resolved) name of the module
        /// that contains the import. Typically, this is used to implement relative
        /// imports.
        /// </summary>
        public WrenResolveModule ResolveModule { get; set; }

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
        public WrenLoadModule LoadModule { get; set; }

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
        public WrenBindForeignMethod BindForeignMethod { get; set; }

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
        public WrenBindForeignClass BindForeignClass { get; set; }

        /// <summary>
        /// The callback Wren uses to display text when `System.print()` or the other
        /// related functions are called.
        /// <para/>
        /// If this is `NULL`, Wren discards any printed text.
        /// </summary>
        public WrenWrite Write { get; set; }

        /// <summary>
        /// The callback Wren uses to report errors.
        /// <para/>
        /// When an error occurs, this will be called with the module name, line
        /// number, and an error message. If this is `NULL`, Wren doesn't report any
        /// errors.
        /// </summary>
        public WrenError Error { get; set; }

        /// <summary>
        /// The number of bytes Wren will allocate before triggering the first garbage
        /// collection.
        /// <para/>
        /// If zero, defaults to 10MiB.
        /// <para/>
        /// Can't be changed after the <see cref="WrenConfig"/> has been used to create a <see cref="WrenVM"/>.
        /// </summary>
        public uint InitialHeapSize
        {
            get { return initialHeapSize; }
            set
            {
                if (Used)
                    throw new InvalidOperationException($"Can't change {nameof(InitialHeapSize)} after the config has been used!");

                initialHeapSize = value;
            }
        }

        /// <summary>
        /// After a collection occurs, the threshold for the next collection is
        /// determined based on the number of bytes remaining in use. This allows Wren
        /// to shrink its memory usage automatically after reclaiming a large amount
        /// of memory.
        /// <para/>
        /// This can be used to ensure that the heap does not get too small, which can
        /// in turn lead to a large number of collections afterwards as the heap grows
        /// back to a usable size.
        /// <para/>
        /// If zero, defaults to 1MiB.
        /// <para/>
        /// Can't be changed after the <see cref="WrenConfig"/> has been used to create a <see cref="WrenVM"/>.
        /// </summary>
        public uint MinHeapSize
        {
            get { return minHeapSize; }
            set
            {
                if (Used)
                    throw new InvalidOperationException($"Can't change {nameof(MinHeapSize)} after the config has been used!");

                minHeapSize = value;
            }
        }

        /// <summary>
        /// Wren will grow (and shrink) the heap automatically as the number of bytes
        /// remaining in use after a collection changes. This number determines the
        /// amount of additional memory Wren will use after a collection, as a
        /// percentage of the current heap size.
        /// <para/>
        /// For example, say that this is 50. After a garbage collection, Wren there
        /// are 400 bytes of memory still in use. That means the next collection will
        /// be triggered after a total of 600 bytes are allocated (including the 400
        /// already in use.
        /// <para/>
        /// Setting this to a smaller number wastes less memory, but triggers more
        /// frequent garbage collections.
        /// <para/>
        /// If zero, defaults to 50%.
        /// <para/>
        /// Can't be changed after the <see cref="WrenConfig"/> has been used to create a <see cref="WrenVM"/>.
        /// </summary>
        public int HeapGrowthPercent
        {
            get { return heapGrowthPercent; }
            set
            {
                if (Used)
                    throw new InvalidOperationException($"Can't change {nameof(HeapGrowthPercent)} after the config has been used!");

                heapGrowthPercent = value;
            }
        }

        /// <summary>
        /// Returns the internal config struct expected by Wren and marks this <see cref="WrenConfig"/> as used.
        /// </summary>
        /// <returns>The internal config struct expected by Wren.</returns>
        internal WrenConfig UseConfig()
        {
            Used = true;
            return this;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenConfig"/> class with the default settings.
        /// </summary>
        public WrenConfig()
        {
            initConfiguration(this);

            resolveModule = resolveModuleImpl;
            loadModule = loadModuleImpl;
            bindForeignMethod = bindForeignMethodImpl;
            bindForeignClass = bindForeignClassImpl;
            write = writeImpl;
            error = errorImpl;
        }

        private string resolveModuleImpl(IntPtr vm, string importer, string name)
        {
            // if no ResolveModule subscriber resolves the name, return the input
            string result = name;

            if (ResolveModule == null)
                return name;

            var resolveResult = ResolveModule.GetInvocationList().Cast<WrenResolveModule>()
                .Select(resolveModule => resolveModule(WrenVM.GetVM(vm), importer, name))
                .FirstOrDefault(res => res != null);

            if (resolveResult == null)
                return name;

            return resolveResult;
        }

        private WrenLoadModuleResultInternal loadModuleImpl(IntPtr vm, string name)
        {
            // Only one of the multiple possible LoadModule implementations must actually return the source for the module
            var externalResult = LoadModule?.GetInvocationList().Cast<WrenLoadModule>()
                .Select(loadModule => loadModule(WrenVM.GetVM(vm), name))
                .FirstOrDefault(res => !string.IsNullOrWhiteSpace(res?.Source));

            if (externalResult == null)
                return default;

            return externalResult.GetStruct();
        }

        private WrenForeignMethodInternal bindForeignMethodImpl(IntPtr vmPtr, string module, string className, bool isStatic, string signature)
        {
            var vm = WrenVM.GetVM(vmPtr);

            // Only one of the multiple possible BindForeignMethod implementations must actually return a method.
            var result = BindForeignMethod?.GetInvocationList().Cast<WrenBindForeignMethod>()
                .Select(bindForeignMethod => bindForeignMethod(vm, module, className, isStatic, signature))
                .SingleOrDefault(res => res != null);

            if (result == null)
                return null;

            // Have to save the delegate so it isn't GCed
            return vm.PreserveForeignMethod(result);
        }

        private static Dictionary<WrenForeignClassMethods, WrenForeignClassMethodsInternal> classMethods =
            new Dictionary<WrenForeignClassMethods, WrenForeignClassMethodsInternal>();

        private WrenForeignClassMethodsInternal bindForeignClassImpl(IntPtr vm, string module, string className)
        {
            if (BindForeignClass == null)
                return new WrenForeignClassMethodsInternal();

            // Only one of the multiple possible BindForeignClass implementations must actually return the ForeignClassMethods
            var results = BindForeignClass.GetInvocationList().Cast<WrenBindForeignClass>()
                .Select(bindForeignClass => bindForeignClass(WrenVM.GetVM(vm), module, className));

            var result = results.Single(res => res != null);

            // Have to save struct, so the delegates aren't GCed
            var methods = new WrenForeignClassMethodsInternal(result);
            classMethods.Add(result, methods);

            return methods;
        }

        private void writeImpl(IntPtr vm, string text)
        {
            Write?.Invoke(WrenVM.GetVM(vm), text);
        }

        private void errorImpl(IntPtr vm, WrenErrorType type, string module, int line, string message)
        {
            Error?.Invoke(WrenVM.GetVM(vm), type, module, line, message);
        }

        [DllImport(WrenVM.WrenLib, EntryPoint = "wrenInitConfiguration", CallingConvention = CallingConvention.Cdecl)]
        private static extern void initConfiguration([Out] WrenConfig config);
    }
}