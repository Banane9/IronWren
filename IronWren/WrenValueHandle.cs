using System;

namespace IronWren
{
    /// <summary>
    /// Represents a handle to a value in the <see cref="WrenVM"/>. As long as this exists, the value won't be garbage collected.
    /// </summary>
    public sealed class WrenValueHandle : WrenHandle
    {
        internal WrenValueHandle()
            : base()
        { }
    }
}