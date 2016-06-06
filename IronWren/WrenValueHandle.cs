using System;

namespace IronWren
{
    /// <summary>
    /// Represents a handle to a value in the <see cref="WrenVM"/>.
    /// <para/>
    /// Used by <see cref="WrenVM.ReleaseValue(WrenValueHandle)"/>, <see cref="WrenVM.SetSlotHandle(int, WrenValueHandle)"/>
    /// and gotten from <see cref="WrenVM.GetSlotHandle(int)"/>.
    /// </summary>
    public struct WrenValueHandle
    {
        internal IntPtr HandlePtr { get; }

        internal WrenValueHandle(IntPtr handlePtr)
        {
            HandlePtr = handlePtr;
        }
    }
}