using System;

namespace IronWren
{
    /// <summary>
    /// Represents a handle to a Wren-function in the <see cref="WrenVM"/>.
    /// <para/>
    /// Used by <see cref="WrenVM.ReleaseHandle(WrenValueHandle)"/>, <see cref="WrenVM.Call(WrenFunctionHandle)"/>
    /// and gotten from <see cref="WrenVM.MakeCallHandle(string)"/>.
    /// </summary>
    public struct WrenFunctionHandle
    {
        internal IntPtr HandlePtr { get; }

        internal WrenFunctionHandle(IntPtr handlePtr)
        {
            HandlePtr = handlePtr;
        }
    }
}