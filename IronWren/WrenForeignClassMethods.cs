using System.Runtime.InteropServices;

namespace IronWren
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WrenForeignClassMethods
    {
        // The callback invoked when the foreign object is created.
        //
        // This must be provided. Inside the body of this, it must call
        // [wrenAllocateForeign] exactly once.
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WrenForeignMethod allocate;

        // The callback invoked when the garbage collector is about to collecto a
        // foreign object's memory.
        //
        // This may be `NULL` if the foreign class does not need to finalize.
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WrenFinalizer finalize;
    }
}