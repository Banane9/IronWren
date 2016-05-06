using System;
using System.Runtime.InteropServices;

namespace IronWren
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WrenForeignClassMethodsInternal
    {
        public IntPtr Allocate;

        public IntPtr Finalize;

        public WrenForeignClassMethodsInternal(WrenForeignClassMethods methods)
        {
            if (methods.Allocate != null)
                Allocate = Marshal.GetFunctionPointerForDelegate((WrenForeignMethodInternal)methods.AllocateInternal);
            else
                Allocate = IntPtr.Zero;

            if (methods.Finalize != null)
                Finalize = Marshal.GetFunctionPointerForDelegate(methods.Finalize);
            else
                Finalize = IntPtr.Zero;
        }
    }

    public class WrenForeignClassMethods
    {
        /// <summary>
        /// The callback invoked when the foreign object is created.
        ///
        /// This must be provided. Inside the body of this, it must call
        /// <see cref="WrenVM.SetSlotNewForeign(int, int, uint)"/> exactly once.
        /// </summary>
        public WrenForeignMethod Allocate;

        /// <summary>
        /// The callback invoked when the garbage collector is about to collect a foreign object's memory.
        ///
        /// This may be null if the foreign class does not need to finalize.
        /// </summary>
        public WrenFinalizer Finalize;

        internal void AllocateInternal(IntPtr vm)
        {
            if (Allocate == null)
                return;

            Allocate(WrenVM.GetVM(vm));
        }
    }
}