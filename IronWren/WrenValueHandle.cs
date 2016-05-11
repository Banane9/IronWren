using System;

namespace IronWren
{
    /// <summary>
    /// Represents a handle to a value in the WrenVM.
    /// <para/>
    /// Used by <see cref="WrenVM.ReleaseValue(WrenValueHandle)"/>, <see cref="WrenVM.SetSlotValue(int, WrenValueHandle)"/>
    /// and gotten from <see cref="WrenVM.GetSlotValue(int)"/>.
    /// </summary>
    public struct WrenValueHandle
    {
        private readonly IntPtr valuePtr;

        public WrenValueHandle(IntPtr valuePtr)
        {
            this.valuePtr = valuePtr;
        }

        public static explicit operator WrenValueHandle(IntPtr valuePtr)
        {
            return new WrenValueHandle(valuePtr);
        }

        public static implicit operator IntPtr(WrenValueHandle value)
        {
            return value.valuePtr;
        }
    }
}