using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren
{
    /// <summary>
    /// Represents a handle to something in a <see cref="WrenVM"/>.
    /// </summary>
    public abstract class WrenHandle : IDisposable
    {
        private readonly WeakReference<WrenVM> vmRef;
        private bool released;

        internal IntPtr HandlePtr { get; }

        internal WrenHandle(WrenVM vm, IntPtr handlePtr)
        {
            vmRef = new WeakReference<WrenVM>(vm);

            HandlePtr = handlePtr;
        }

        /// <summary>
        /// Releases the handle in the <see cref="WrenVM"/> that created it and allows the GC to claim the memory.
        /// </summary>
        ~WrenHandle()
        {
            Release();
        }

        /// <summary>
        /// Releases the handle in the <see cref="WrenVM"/> that created it and allows the GC to claim the memory.
        /// </summary>
        public void Dispose()
        {
            Release();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the handle in the <see cref="WrenVM"/> that created it and allows the GC to claim the memory.
        /// </summary>
        public void Release()
        {
            WrenVM vm;
            if (released || !vmRef.TryGetTarget(out vm))
                return;

            vm.ReleaseHandle(this);
            released = true;
        }
    }
}