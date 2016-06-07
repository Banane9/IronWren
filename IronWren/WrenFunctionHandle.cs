using System;

namespace IronWren
{
    /// <summary>
    /// Represents a handle to a Wren-function in the <see cref="WrenVM"/>.
    /// </summary>
    public sealed class WrenFunctionHandle : WrenHandle
    {
        internal WrenFunctionHandle(WrenVM vm, IntPtr handlePtr)
            : base(vm, handlePtr)
        { }
    }
}