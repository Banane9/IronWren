using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

namespace IronWren
{
    /// <summary>
    /// Represents a handle to something in a <see cref="WrenVM"/>.
    /// </summary>
    public abstract class WrenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private WrenVM vm;

        internal WrenHandle()
            : base(true)
        { }

        internal void SetVM(WrenVM vm)
        {
            this.vm = vm;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            vm.ReleaseHandle(handle);
            return true;
        }
    }
}