using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace IronWren
{
    public class WrenVM : IDisposable
    {
        // Also stops CodeMaid from reorganizing the file
#if DEBUG
        private const string wren = "Wren/wren-debug";
#else
        private const string wren = "Wren/wren";
#endif

        private static Dictionary<IntPtr, WrenVM> vms = new Dictionary<IntPtr, WrenVM>();

        /// <summary>
        /// The name of the module in which calls to <see cref="WrenVM.Interpret(string)"/> are evaluated.
        /// </summary>
        public const string InterpetModule = "main";

        private WrenConfig config;
        private IntPtr vm;

        /// <summary>
        /// Creates a new instance of the <see cref="WrenVM"/> class with the given config for the VM.
        /// </summary>
        /// <param name="config">The config for the VM. Will be kept safe by this class.</param>
        public WrenVM(WrenConfig config)
        {
            vm = newVM(ref config.config);

            vms.Add(vm, this);

            // keep the config/delegates inside it alive
            this.config = config;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenVM"/> class with the default config for the VM.
        /// </summary>
        public WrenVM()
        {
            vm = newVM(IntPtr.Zero);

            vms.Add(vm, this);
        }

        /// <summary>
        /// Gets the <see cref="WrenVM"/> associated with the given IntPtr.
        /// </summary>
        /// <param name="ptr">The IntPtr to the VM.</param>
        /// <returns>The <see cref="WrenVM"/> object.</returns>
        public static WrenVM GetVM(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentException("Pointer can't be a null pointer!", nameof(ptr));

            if (!vms.ContainsKey(ptr))
                throw new ArgumentException("No VM with that pointer found!", nameof(ptr));

            return vms[ptr];
        }

        /// <summary>
        /// Calls the given method, using the receiver and arguments previously set up on the stack.
        ///
        /// The method must have been created by a call to <see cref="MakeCallHandle(string)"/>.
        /// The arguments to the method must be already on the stack. The receiver should be
        /// in slot 0 with the remaining arguments following it, in order. It is an
        /// error if the number of arguments provided does not match the method's signature.
        ///
        /// After this returns, you can access the return value from slot 0 on the stack.
        /// </summary>
        /// <param name="callHandle"></param>
        /// <returns></returns>
        public WrenInterpretResult Call(IntPtr callHandle)
        {
            return call(vm, callHandle);
        }

        /// <summary>
        /// Immediately run the garbage collector to free unused memory.
        /// </summary>
        public void CollectGarbage()
        {
            collectGarbage(vm);
        }

        /// <summary>
        /// Frees the memory used by the VM. This makes it unusable.
        /// </summary>
        public void Dispose()
        {
            freeVM(vm);
            vm = IntPtr.Zero;
        }

        /// <summary>
        /// Looks up the top level variable with the given name in the given module and stores it in the given slot.
        /// </summary>
        /// <param name="module">The module that the variable is in.</param>
        /// <param name="name">The name of the variable.</param>
        /// <param name="slot">The slot to store it in.</param>
        public void GetVariable(string module, string name, int slot)
        {
            getVariable(vm, module, name, slot);
        }

        /// <summary>
        /// Runs the given string of Wren source code in a new fiber in the VM.
        /// </summary>
        /// <param name="source">The Wren source code to run.</param>
        /// <returns>The status of the interpretion.</returns>
        public WrenInterpretResult Interpret(string source)
        {
            return interpret(vm, source);
        }

        /// <summary>
        /// Creates a handle that can be used to invoke a method with the given signature on
        /// a receiver and arguments that are set up on the stack.
        ///
        /// This handle can be used repeatedly to directly invoke that method from
        /// code using <see cref="Call(IntPtr)"/>.
        ///
        /// When you are done with this handle, it must be released using <see cref="ReleaseValue(IntPtr)"/>.
        /// </summary>
        /// <param name="signature">The signature of the method. It has to include the braces and an underscore for each argument.
        /// For example a function with one argument would look like: oneArgument(_).</param>
        /// <returns>A handle to the function.</returns>
        public IntPtr MakeCallHandle(string signature)
        {
            return makeCallHandle(vm, signature);
        }

        /// <summary>
        /// Ensures that the foreign method stack has at least the given number of slots available for use,
        /// growing the stack if needed.
        ///
        /// Does not shrink the stack if it has more than enough slots.
        ///
        /// It is an error to call this from a finalizer.
        /// </summary>
        /// <param name="count">The minimum number of slots.</param>
        public void EnsureSlots(int count)
        {
            ensureSlots(vm, count);
        }

        /// <summary>
        /// Stores the given string in the given slot.
        ///
        /// The text is copied to a new string within Wren's heap, so you can free
        /// memory used by it after this is called. The length is calculated using
        /// strlen(). If the string may contain any null bytes in the middle, then you
        /// should use <see cref="SetSlotBytes()"/> instead.
        /// </summary>
        /// <param name="slot">The slot to write the string to.</param>
        /// <param name="text">The string to write to the slot.</param>
        public void SetSlotString(int slot, string text)
        {
            setSlotString(vm, slot, text);
        }

        /// <summary>
        /// Creates a new instance of the foreign class stored in [classSlot] with [size]
        /// bytes of raw storage and places the resulting object in [slot].
        ///
        /// This does not invoke the foreign class's constructor on the new instance. If
        /// you need that to happen, call the constructor from Wren, which will then
        /// call the allocator foreign method. In there, call this to create the object
        /// and then the constructor will be invoked when the allocator returns.
        ///
        /// Returns a pointer to the foreign object's data.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="classSlot"></param>
        /// <param name="size">The size in bytes.</param>
        /// <returns></returns>
        public IntPtr SetSlotNewForeign(int slot, int classSlot, uint size)
        {
            return setSlotNewForeign(vm, slot, classSlot, size);
        }

        [DllImport(wren, EntryPoint = "wrenSetSlotNewForeign", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr setSlotNewForeign(IntPtr vm, int slot, int classSlot, uint size);

        [DllImport(wren, EntryPoint = "wrenSetSlotString", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotString(IntPtr vm, int slot, [MarshalAs(UnmanagedType.LPStr), In]string text);

        [DllImport(wren, EntryPoint = "wrenEnsureSlots", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ensureSlots(IntPtr vm, int count);

        [DllImport(wren, EntryPoint = "wrenCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenInterpretResult call(IntPtr vm, IntPtr callHandle);

        [DllImport(wren, EntryPoint = "wrenCollectGarbage", CallingConvention = CallingConvention.Cdecl)]
        private static extern void collectGarbage(IntPtr vm);

        [DllImport(wren, EntryPoint = "wrenGetVariable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void getVariable(IntPtr vm,
            [MarshalAs(UnmanagedType.LPStr), In]string module, [MarshalAs(UnmanagedType.LPStr), In]string name, int slot);

        [DllImport(wren, EntryPoint = "wrenInterpret", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenInterpretResult interpret(IntPtr vm, [MarshalAs(UnmanagedType.LPStr), In]string source);

        [DllImport(wren, EntryPoint = "wrenMakeCallHandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr makeCallHandle(IntPtr vm, [MarshalAs(UnmanagedType.LPStr), In]string signature);

        #region VM Lifecycle

        [DllImport(wren, EntryPoint = "wrenFreeVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern void freeVM(IntPtr vm);

        [DllImport(wren, EntryPoint = "wrenNewVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr newVM([In]ref WrenConfig.Config config);

        [DllImport(wren, EntryPoint = "wrenNewVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr newVM(IntPtr config);

        #endregion VM Lifecycle
    }
}