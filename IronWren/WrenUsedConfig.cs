using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

// Stop CodeMaid from reorganizing the file
#if DEBUG
#endif

namespace IronWren
{
    /// <summary>
    /// Represents the configuration in use by a <see cref="WrenVM"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class WrenUsedConfig
    {
        internal static readonly WrenUsedConfig DefaultConfig = new();

        [MarshalAs(UnmanagedType.FunctionPtr)]
        private readonly WrenReallocateInternal reallocate;

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

        /// <summary>
        /// The number of bytes Wren will allocate before triggering the first garbage
        /// collection.
        ///
        /// If zero, defaults to 10MB.
        /// </summary>
        public readonly nuint InitialHeapSize;

        /// <summary>
        /// After a collection occurs, the threshold for the next collection is
        /// determined based on the number of bytes remaining in use. This allows Wren
        /// to shrink its memory usage automatically after reclaiming a large amount
        /// of memory.
        ///
        /// This can be used to ensure that the heap does not get too small, which can
        /// in turn lead to a large number of collections afterwards as the heap grows
        /// back to a usable size.
        ///
        /// If zero, defaults to 1MB.
        /// </summary>
        public readonly nuint MinHeapSize;

        /// <summary>
        /// Wren will resize the heap automatically as the number of bytes
        /// remaining in use after a collection changes. This number determines the
        /// amount of additional memory Wren will use after a collection, as a
        /// percentage of the current heap size.
        ///
        /// For example, say that this is 50. After a garbage collection, when there
        /// are 400 bytes of memory still in use, the next collection will be triggered
        /// after a total of 600 bytes are allocated (including the 400 already in
        /// use.)
        ///
        /// Setting this to a smaller number wastes less memory, but triggers more
        /// frequent garbage collections.
        ///
        /// If zero, defaults to 50.
        /// </summary>
        public readonly int HeapGrowthPercent;

        private readonly IntPtr userData;

        static WrenUsedConfig()
        {
            initConfiguration(DefaultConfig);
        }

        internal WrenUsedConfig(WrenConfig config, WrenVM vm)
        {
            initConfiguration(this);

            InitialHeapSize = config.InitialHeapSize;
            MinHeapSize = config.MinHeapSize;
            HeapGrowthPercent = config.HeapGrowthPercent;

            reallocate = DefaultConfig.reallocate;
            resolveModule = vm.resolveModule;
            loadModule = vm.loadModule;
            bindForeignMethod = vm.bindForeignMethod;
            bindForeignClass = vm.bindForeignClass;
            write = vm.write;
            error = vm.error;
        }

        private WrenUsedConfig()
        { }

        [DllImport(WrenVM.WrenLib, EntryPoint = "wrenInitConfiguration", CallingConvention = CallingConvention.Cdecl)]
        private static extern void initConfiguration([Out] WrenUsedConfig config);
    }
}