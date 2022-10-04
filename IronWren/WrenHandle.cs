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

        protected WrenHandle() : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            Debug.Assert(vm != null);
            vm.ReleaseHandle(this);
            return true;
        }

        internal void SetVm(WrenVM vm)
        {
            this.vm = vm;
        }
    }
}