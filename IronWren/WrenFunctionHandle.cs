using System;

namespace IronWren
{
    /// <summary>
    /// Represents a handle to a function in the WrenVM.
    /// <para/>
    /// Used by <see cref="WrenVM.ReleaseValue(WrenValueHandle)"/>, <see cref="WrenVM.Call(WrenFunctionHandle)"/>
    /// and gotten from <see cref="WrenVM.MakeCallHandle(string)"/>.
    /// </summary>
    public struct WrenFunctionHandle
    {
        private readonly IntPtr functionPtr;

        public WrenFunctionHandle(IntPtr functionPtr)
        {
            this.functionPtr = functionPtr;
        }

        public static explicit operator WrenFunctionHandle(IntPtr functionPtr)
        {
            return new WrenFunctionHandle(functionPtr);
        }

        public static implicit operator IntPtr(WrenFunctionHandle handle)
        {
            return handle.functionPtr;
        }
    }
}