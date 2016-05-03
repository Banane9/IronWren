using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

#if true
#endif

namespace IronWren
{
    public class WrenVM : IDisposable
    {
        private IntPtr vm;

        public WrenVM(WrenConfig config)
        {
            vm = newVM(ref config.config);
        }

        public WrenVM()
        {
            vm = newVM(IntPtr.Zero);
        }

        public WrenInterpretResult Interpret(string source)
        {
            return interpret(vm, source);
        }

        [DllImport("wren", EntryPoint = "wrenNewVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr newVM(ref WrenConfig.Config config);

        [DllImport("wren", EntryPoint = "wrenNewVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr newVM(IntPtr config);

        [DllImport("wren", EntryPoint = "wrenInterpret", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenInterpretResult interpret(IntPtr vm, [MarshalAs(UnmanagedType.LPStr)]string source);

        [DllImport("wren", EntryPoint = "wrenFreeVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern void freeVM(IntPtr vm);

        /// <summary>
        /// Frees the memory used by the VM. This makes it unusable.
        /// </summary>
        public void Dispose()
        {
            freeVM(vm);
            vm = IntPtr.Zero;
        }
    }
}