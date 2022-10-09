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

        public readonly nuint InitialHeapSize;

        public readonly nuint MinHeapSize;

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