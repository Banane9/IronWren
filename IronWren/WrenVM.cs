using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace IronWren
{
    /// <summary>
    /// Represents an instance of a VM running Wren.
    /// </summary>
    public sealed partial class WrenVM : IDisposable
    {
        /// <summary>
        /// The name of the module in which calls to <see cref="Interpret(string)"/> are evaluated.
        /// </summary>
        public const string MainModule = "main";

        private static readonly Dictionary<IntPtr, WeakReference<WrenVM>> vms = new Dictionary<IntPtr, WeakReference<WrenVM>>();

        private readonly HashSet<WrenForeignMethodInternal> foreignMethods = new HashSet<WrenForeignMethodInternal>();
        private IntPtr vm;

        /// <summary>
        /// Gets the config used for this VM.
        /// </summary>
        public WrenConfig Config { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenVM"/> class with the given config for the VM.
        /// </summary>
        /// <param name="config">The config for the VM. Will be kept safe by this class.</param>
        public WrenVM(WrenConfig config)
        {
            Config = config;
            var configStruct = config.UseConfig();

            vm = newVM(ref configStruct);
            vms.Add(vm, new WeakReference<WrenVM>(this));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenVM"/> class with the default config for the VM.
        /// </summary>
        public WrenVM()
            : this(new WrenConfig())
        { }

        /// <summary>
        /// Frees the memory used by the VM if it hasn't been disposed of.
        /// </summary>
        ~WrenVM()
        {
            vms.Remove(vm);

            freeVM(vm);
        }

        /// <summary>
        /// Frees the memory used by the VM. This makes it unusable.
        /// </summary>
        public void Dispose()
        {
            vms.Remove(vm);

            freeVM(vm);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the <see cref="WrenVM"/> associated with the given IntPtr.
        /// </summary>
        /// <param name="ptr">The IntPtr to the VM.</param>
        /// <returns>The <see cref="WrenVM"/> object.</returns>
        internal static WrenVM GetVM(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentException("Pointer can't be a null pointer!", nameof(ptr));

            if (!vms.TryGetValue(ptr, out var wrVM))
                throw new ArgumentException("No VM with that pointer found!", nameof(ptr));

            if (!wrVM.TryGetTarget(out var vm))
                throw new Exception("The VM instance was garbage collected and still received a callback!");

            return vm;
        }

        internal WrenForeignMethodInternal PreserveForeignMethod(WrenForeignMethod foreignMethod)
        {
            WrenForeignMethodInternal wrappedForeignMethod = vmPtr => foreignMethod(this);

            foreignMethods.Add(wrappedForeignMethod);

            return wrappedForeignMethod;
        }
    }
}