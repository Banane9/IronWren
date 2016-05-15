using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace IronWren
{
    /// <summary>
    /// Represents an instance of the VM running Wren.
    /// </summary>
    public class WrenVM : IDisposable
    {
        // Also stops CodeMaid from reorganizing the file
#if DEBUG
        private const string wren = "Native/wren-debug";
#else
        private const string wren = "Native/wren";
#endif

        private static Dictionary<IntPtr, WrenVM> vms = new Dictionary<IntPtr, WrenVM>();

        /// <summary>
        /// The name of the module in which calls to <see cref="Interpret(string)"/> are evaluated.
        /// </summary>
        public const string InterpetModule = "main";

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
            vm = newVM(ref config.config);
            vms.Add(vm, this);

            config.Used = true;

            // keep the config/delegates inside it alive
            Config = config;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenVM"/> class with the default config for the VM.
        /// </summary>
        public WrenVM()
            : this(new WrenConfig())
        { }

        /// <summary>
        /// Gets the <see cref="WrenVM"/> associated with the given IntPtr.
        /// </summary>
        /// <param name="ptr">The IntPtr to the VM.</param>
        /// <returns>The <see cref="WrenVM"/> object.</returns>
        internal static WrenVM GetVM(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentException("Pointer can't be a null pointer!", nameof(ptr));

            if (!vms.ContainsKey(ptr))
                throw new ArgumentException("No VM with that pointer found!", nameof(ptr));

            return vms[ptr];
        }

        #region Managed Wrappers

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

        #region Function Interactions

        /// <summary>
        /// Creates a handle that can be used to invoke a method with the given signature on
        /// a receiver and arguments that are set up on the stack.
        /// <para/>
        /// This handle can be used repeatedly to directly invoke that method from
        /// code using <see cref="Call(WrenFunctionHandle)"/>.
        /// <para/>
        /// When you are done with this handle, it must be released using <see cref="ReleaseValue(WrenFunctionHandle)"/>.
        /// </summary>
        /// <param name="signature">
        /// The signature of the method. It has to include the braces and an underscore for each argument.
        /// For example a function with one argument would look like: oneArgument(_).
        /// </param>
        /// <returns>A handle to the function.</returns>
        public WrenFunctionHandle MakeCallHandle(string signature)
        {
            return (WrenFunctionHandle)makeCallHandle(vm, signature);
        }

        /// <summary>
        /// Calls the given method, using the receiver and arguments previously set up on the stack.
        /// <para/>
        /// The method must have been created by a call to <see cref="MakeCallHandle(string)"/>.
        /// The arguments to the method must be already on the stack. The receiver should be
        /// in slot 0 with the remaining arguments following it, in order. It is an
        /// error if the number of arguments provided does not match the method's signature.
        /// <para/>
        /// After this returns, you can access the return value from slot 0 on the stack.
        /// </summary>
        /// <param name="handle">The call handle of the function.</param>
        /// <returns>The status of the interpretion.</returns>
        public WrenInterpretResult Call(WrenFunctionHandle handle)
        {
            return call(vm, handle);
        }

        /// <summary>
        /// Releases the reference stored in the given function handle.
        /// After calling this, the function handle can no longer be used.
        /// </summary>
        /// <param name="handle">The function handle to release.</param>
        public void ReleaseValue(WrenFunctionHandle handle)
        {
            releaseValue(vm, handle);
        }

        #endregion Function Interactions

        #region Slot Interactions

        /// <summary>
        /// Gets the type of the object in the given slot.
        /// </summary>
        /// <param name="slot">The slot to get the type for.</param>
        /// <returns>The type of data in the slot.</returns>
        public WrenType GetSlotType(int slot)
        {
            return getSlotType(vm, slot);
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
        /// Stores null in the given slot.
        /// </summary>
        /// <param name="slot">The slot to write null to.</param>
        public void SetSlotNull(int slot)
        {
            setSlotNull(vm, slot);
        }

        #region Slot Count

        /// <summary>
        /// Ensures that the foreign method stack has at least the given number of slots available for use,
        /// growing the stack if needed.
        /// <para/>
        /// Does not shrink the stack if it has more than enough slots.
        /// <para/>
        /// It is an error to call this from a finalizer.
        /// </summary>
        /// <param name="count">The minimum number of slots.</param>
        public void EnsureSlots(int count)
        {
            ensureSlots(vm, count);
        }

        /// <summary>
        /// Returns the number of slots available to the current foreign method.
        /// </summary>
        /// <returns>The number of slots available to the current foreign method.</returns>
        public int GetSlotCount()
        {
            return getSlotCount(vm);
        }

        #endregion Slot Count

        #region Value

        /// <summary>
        /// Creates a handle for the value stored in the given slot.
        /// <para/>
        /// This will prevent the object that is referred to from being garbage collected
        /// until the handle is released by calling <see cref="WrenVM.ReleaseValue(WrenValueHandle)"/>.
        /// </summary>
        /// <param name="slot">The slot containing the value to create a handle for.</param>
        /// <returns>A handle to the value in the slot.</returns>
        public WrenValueHandle GetSlotValue(int slot)
        {
            return (WrenValueHandle)getSlotValue(vm, slot);
        }

        /// <summary>
        /// Stores the value captured by the given value handle in the given slot.
        /// <para/>
        /// This does not release the handle for the value. You must call <see cref="WrenVM.ReleaseValue(WrenValueHandle)"/> to do so.
        /// </summary>
        /// <param name="slot">The slot to write the value captured by the value handle to.</param>
        /// <param name="value">The value handle.</param>
        public void SetSlotValue(int slot, WrenValueHandle value)
        {
            setSlotValue(vm, slot, value);
        }

        /// <summary>
        /// Releases the reference stored in the given value.
        /// After calling this, the value handle can no longer be used.
        /// </summary>
        /// <param name="value">The value handle to release.</param>
        public void ReleaseValue(WrenValueHandle value)
        {
            releaseValue(vm, value);
        }

        #endregion Value

        #region Bool

        /// <summary>
        /// Reads a boolean value from the given slot.
        /// <para/>
        /// It is an error to call this if the slot does not contain a boolean value.
        /// </summary>
        /// <param name="slot">The slot to read the boolean from.</param>
        /// <returns>The boolean contained in the slot.</returns>
        public bool GetSlotBool(int slot)
        {
            return getSlotBool(vm, slot);
        }

        /// <summary>
        /// Stores the given boolean value in the given slot.
        /// </summary>
        /// <param name="slot">The slot to write the boolean to.</param>
        /// <param name="value">The boolean to write to the slot.</param>
        public void SetSlotBool(int slot, bool value)
        {
            setSlotBool(vm, slot, value);
        }

        #endregion Bool

        #region Bytes

        /// <summary>
        /// Reads a byte array from the given slot. Uee this, if the content can contain null bytes.
        /// <para/>
        /// It is an error to call this if the slot does not contain a string.
        /// </summary>
        /// <param name="slot">The slot to read the byte array from.</param>
        /// <returns>The byte array contained in the slot.</returns>
        public byte[] GetSlotBytes(int slot)
        {
            int length;
            return getSlotBytes(vm, slot, out length);
        }

        /// <summary>
        /// Stores the given byte array in the given slot.
        /// <para/>
        /// The bytes are copied to a new string within Wren's heap, so you can free
        /// memory used by them after this is called.
        /// </summary>
        /// <param name="slot">The slot to write the byte array to.</param>
        /// <param name="value">The byte array to write to the slot.</param>
        public void SetSlotBytes(int slot, byte[] value)
        {
            setSlotBytes(vm, slot, value, (uint)value.Length);
        }

        #endregion Bytes

        #region Double

        /// <summary>
        /// Reads a number from the given slot.
        /// <para/>
        /// It is an error to call this if the slot does not contain a number.
        /// </summary>
        /// <param name="slot">The slot to read the double from.</param>
        /// <returns>The double contained in the slot.</returns>
        public double GetSlotDouble(int slot)
        {
            return getSlotDouble(vm, slot);
        }

        /// <summary>
        /// Stores the given numeric value in the given slot.
        /// </summary>
        /// <param name="slot">The slot to write the double to.</param>
        /// <param name="value">The double to write to the slot.</param>
        public void SetSlotDouble(int slot, double value)
        {
            setSlotDouble(vm, slot, value);
        }

        #endregion Double

        #region Foreign

        /// <summary>
        /// Reads a foreign object from the given slot and returns a pointer to the foreign data stored with it.
        /// <para/>
        /// It is an error to call this if the slot does not contain an instance of a foreign class.
        /// </summary>
        /// <param name="slot">The slot to read the pointer to the foreign data from.</param>
        /// <returns>The pointer to the foreign data.</returns>
        public IntPtr GetSlotForeign(int slot)
        {
            return getSlotForeign(vm, slot);
        }

        /// <summary>
        /// Creates a new instance of the foreign class stored in [classSlot] with the given amount of
        /// bytes of raw storage and places the resulting object in the given slot.
        /// <para/>
        /// This does not invoke the foreign class's constructor on the new instance. If
        /// you need that to happen, call the constructor from Wren, which will then
        /// call the allocator foreign method. In there, call this to create the object
        /// and then the constructor will be invoked when the allocator returns.
        /// <para/>
        /// Returns a pointer to the foreign object's data.
        /// </summary>
        /// <param name="slot">The slot to create the new instance of the foreign class in.</param>
        /// <param name="classSlot"></param>
        /// <param name="size">The size of data in bytes.</param>
        /// <returns>A pointer to the foreign object's data.</returns>
        public IntPtr SetSlotNewForeign(int slot, int classSlot, uint size)
        {
            return setSlotNewForeign(vm, slot, classSlot, size);
        }

        #endregion Foreign

        #region String

        /// <summary>
        /// Reads a string from the given slot.
        /// <para/>
        /// The memory for the returned string is owned by Wren. You can inspect it
        /// while in your foreign method, but cannot keep a pointer to it after the
        /// function returns, since the garbage collector may reclaim it.
        /// <para/>
        /// It is an error to call this if the slot does not contain a string.
        /// </summary>
        /// <param name="slot">The slot to read the string from.</param>
        /// <returns>The string contained in the slot.</returns>
        public string GetSlotString(int slot)
        {
            var contentPtr = getSlotString(vm, slot);
            return Marshal.PtrToStringAnsi(contentPtr);
        }

        /// <summary>
        /// Stores the given string in the given slot.
        /// <para/>
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

        #endregion String

        #region List

        /// <summary>
        /// Stores a new empty list in the given slot.
        /// </summary>
        /// <param name="slot">The slot to create the new empty list in.</param>
        public void SetSlotNewList(int slot)
        {
            setSlotNewList(vm, slot);
        }

        /// <summary>
        /// Takes the value stored at the given elementSlot and inserts it
        /// at the given index into the list stored at the given listSlot.
        /// <para/>
        /// As in Wren, negative indexes can be used to insert from the end.
        /// To append an element, use `-1` for the index.
        /// </summary>
        /// <param name="listSlot">The slot containing the list to insert into.</param>
        /// <param name="index">
        /// The index that the new element is supposed to have in the list.
        /// -1 to append at the end.
        /// </param>
        /// <param name="elementSlot">The slot containing the element to insert.</param>
        public void InsertInList(int listSlot, int index, int elementSlot)
        {
            insertInList(vm, listSlot, index, elementSlot);
        }

        #endregion List

        #endregion Slot Interactions

        #endregion Managed Wrappers

        #region Native Bindings

        [DllImport(wren, EntryPoint = "wrenInterpret", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenInterpretResult interpret(IntPtr vm, [MarshalAs(UnmanagedType.LPStr), In]string source);

        #region Slot Interactions

        [DllImport(wren, EntryPoint = "wrenGetSlotType", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenType getSlotType(IntPtr vm, int slot);

        [DllImport(wren, EntryPoint = "wrenGetVariable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void getVariable(IntPtr vm,
            [MarshalAs(UnmanagedType.LPStr), In]string module, [MarshalAs(UnmanagedType.LPStr), In]string name, int slot);

        [DllImport(wren, EntryPoint = "wrenSetSlotNull", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotNull(IntPtr vm, int slot);

        #region Slot Count

        [DllImport(wren, EntryPoint = "wrenGetSlotCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getSlotCount(IntPtr vm);

        [DllImport(wren, EntryPoint = "wrenEnsureSlots", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ensureSlots(IntPtr vm, int count);

        #endregion Slot Count

        #region Value

        [DllImport(wren, EntryPoint = "wrenGetSlotValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getSlotValue(IntPtr vm, int slot);

        [DllImport(wren, EntryPoint = "wrenSetSlotValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotValue(IntPtr vm, int slot, IntPtr value);

        [DllImport(wren, EntryPoint = "wrenReleaseValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern void releaseValue(IntPtr vm, IntPtr value);

        #endregion Value

        #region Bool

        [DllImport(wren, EntryPoint = "wrenGetSlotBool", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool getSlotBool(IntPtr vm, int slot);

        [DllImport(wren, EntryPoint = "wrenSetSlotBool", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotBool(IntPtr vm, int slot, bool value);

        #endregion Bool

        #region Bytes

        [DllImport(wren, EntryPoint = "wrenGetSlotBytes", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] // sbyte?
        private static extern byte[] getSlotBytes(IntPtr vm, int slot, out int length);

        [DllImport(wren, EntryPoint = "wrenSetSlotBytes", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotBytes(IntPtr vm, int slot, //sbyte?
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]byte[] bytes, uint length);

        #endregion Bytes

        #region Double

        [DllImport(wren, EntryPoint = "wrenGetSlotDouble", CallingConvention = CallingConvention.Cdecl)]
        private static extern double getSlotDouble(IntPtr vm, int slot);

        [DllImport(wren, EntryPoint = "wrenSetSlotDouble", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotDouble(IntPtr vm, int slot, double value);

        #endregion Double

        #region Foreign

        [DllImport(wren, EntryPoint = "wrenGetSlotForeign", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getSlotForeign(IntPtr vm, int slot);

        [DllImport(wren, EntryPoint = "wrenSetSlotNewForeign", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr setSlotNewForeign(IntPtr vm, int slot, int classSlot, uint size);

        #endregion Foreign

        #region String

        [DllImport(wren, EntryPoint = "wrenGetSlotString", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getSlotString(IntPtr vm, int slot);

        [DllImport(wren, EntryPoint = "wrenSetSlotString", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotString(IntPtr vm, int slot, [MarshalAs(UnmanagedType.LPStr), In]string text);

        #endregion String

        #region List

        [DllImport(wren, EntryPoint = "wrenSetSlotNewList", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotNewList(IntPtr vm, int slot);

        [DllImport(wren, EntryPoint = "wrenInsertInList", CallingConvention = CallingConvention.Cdecl)]
        private static extern void insertInList(IntPtr vm, int listSlot, int index, int elementSlot);

        #endregion List

        #endregion Slot Interactions

        #region Function Interactions

        [DllImport(wren, EntryPoint = "wrenMakeCallHandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr makeCallHandle(IntPtr vm, [MarshalAs(UnmanagedType.LPStr), In]string signature);

        [DllImport(wren, EntryPoint = "wrenCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenInterpretResult call(IntPtr vm, IntPtr callHandle);

        #endregion Function Interactions

        #region VM Lifecycle

        [DllImport(wren, EntryPoint = "wrenNewVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr newVM([In]ref WrenConfig.Config config);

        [DllImport(wren, EntryPoint = "wrenCollectGarbage", CallingConvention = CallingConvention.Cdecl)]
        private static extern void collectGarbage(IntPtr vm);

        [DllImport(wren, EntryPoint = "wrenFreeVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern void freeVM(IntPtr vm);

        #endregion VM Lifecycle

        #endregion Native Bindings
    }
}