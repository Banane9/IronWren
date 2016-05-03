using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

#if true
#endif

namespace IronWren
{
    public class WrenVM
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

        [DllImport("wren_static_d", EntryPoint = "wrenNewVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr newVM(ref WrenConfig.Config config);

        [DllImport("wren_static_d", EntryPoint = "wrenNewVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr newVM(IntPtr config);

        [DllImport("wren_static_d", EntryPoint = "wrenInterpret", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenInterpretResult interpret(IntPtr vm, [MarshalAs(UnmanagedType.LPStr)]string source);
    }
}