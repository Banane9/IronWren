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
                Finalize = Marshal.GetFunctionPointerForDelegate((WrenFinalizerInternal)methods.FinalizeInternal);
            else
                Finalize = IntPtr.Zero;
        }
    }

    /// <summary>
    /// Contains the callbacks called by the <see cref="WrenVM"/> when a foreign object is allocated/finalized.
    /// </summary>
    public class WrenForeignClassMethods
    {
        private WrenVM usedVM;

        /// <summary>
        /// The callback invoked when the foreign object is created.
        /// <para/>
        /// This must be provided, except if the class will never be created.
        /// Inside the body of this, it must call <see cref="WrenVM.SetSlotNewForeign(int, object)"/> exactly once.
        /// </summary>
        public WrenForeignMethod Allocate { get; set; }

        /// <summary>
        /// The callback invoked when the garbage collector is about to collect a foreign object.
        /// <para/>
        /// Only the object is provided, as the method may not interact with the VM.
        /// <para/>
        /// This may be null if the foreign class does not need to finalize.
        /// </summary>
        public WrenFinalizer Finalize { get; set; }

        internal void AllocateInternal(IntPtr vm)
        {
            if (Allocate == null)
                return;

            usedVM = WrenVM.GetVM(vm);

            Allocate(usedVM);
        }

        internal void FinalizeInternal(IntPtr dataPtr)
        {
            var foreignObject = usedVM.GetAndFreeForeign(dataPtr);

            if (Finalize == null)
                return;

            Finalize(foreignObject);
        }
    }
}