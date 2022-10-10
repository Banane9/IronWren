using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;

namespace IronWren
{
    public sealed partial class WrenVM : SafeHandleZeroOrMinusOneIsInvalid
    {
        // Also stops CodeMaid from reorganizing the file
#if DEBUG
        internal const string WrenLib = "Native/wren";
#else
        internal const string WrenLib = "Native/wren";
#endif

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            freeVM(handle);
            return true;
        }

        #region Managed Wrappers

        /// <summary>
        /// Get the current wren version number.
        ///
        /// Can be used to range checks over versions.
        /// </summary>
        public static int GetVersionNumber()
        {
            return getVersionNumber();
        }

        /// <summary>
        /// Runs the given string of Wren source code in a new fiber in the VM.
        /// </summary>
        /// <param name="source">The Wren source code to run.</param>
        /// <returns>The status of the interpretion.</returns>
        public WrenInterpretResult Interpret(string module, string source)
        {
            return interpret(this, module, source);
        }

        /// <summary>
        /// Runs the given string of Wren source code in a new fiber in the VM.
        /// </summary>
        /// <param name="source">The Wren source code to run.</param>
        /// <returns>The status of the interpretion.</returns>
        public WrenInterpretResult Interpret(string source)
        {
            return Interpret(MainModule, source);
        }

        /// <summary>
        /// Immediately run the garbage collector to free unused memory.
        /// </summary>
        public void CollectGarbage()
        {
            collectGarbage(this);
        }

        /// <summary>
        /// Releases the handle and allows the GC to claim the memory.
        /// </summary>
        internal void ReleaseHandle(IntPtr wrenHandle)
        {
            releaseHandle(this, wrenHandle);
        }

        #region Function Interactions

        /// <summary>
        /// Creates a handle that can be used to invoke a method with the given signature on
        /// a receiver and arguments that are set up on the stack.
        /// <para/>
        /// This handle can be used repeatedly to directly invoke that method from
        /// code using <see cref="Call(WrenFunctionHandle)"/>.
        /// <para/>
        /// When you are done with this handle, it must be released using <see cref="WrenHandle.Release"/>.
        /// </summary>
        /// <param name="signature">
        /// The signature of the method. It has to include the braces and an underscore for each argument.
        /// For example a function with one argument would look like: oneArgument(_).
        /// </param>
        /// <returns>A handle to the function.</returns>
        public WrenFunctionHandle MakeCallHandle(string signature)
        {
            var handle = makeCallHandle(this, signature);
            handle.SetVM(this);
            return handle;
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
        public WrenInterpretResult Call(WrenFunctionHandle wrenHandle)
        {
            return call(this, wrenHandle);
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
            return getSlotType(this, slot);
        }

        /// <summary>
        /// Looks up the top level variable with <paramref name="name"/> in resolved <paramref name="module"/>,
        /// returns false if not found. The module must be imported at the time,
        /// use wrenHasModule to ensure that before calling.
        /// </summary>
        /// <param name="module">The module to look for the variable in.</param>
        /// <param name="name">The name of the variable to look for.</param>
        /// <returns>True if a variable with the name is resolved in the module.</returns>
        public bool HasVariable(string module, string name)
        {
            return hasVariable(this, module, name);
        }

        /// <summary>
        /// Returns true if <paramref name="module"/> has been imported/resolved before, false if not.
        /// </summary>
        /// <param name="module">The module to check for.</param>
        /// <returns>True if the module has been imported/resolved before, false if not.</returns>
        public bool HasModule(string module)
        {
            return hasModule(this, module);
        }

        /// <summary>
        /// Looks up the top level variable with the given name in the given module and stores it in the given slot.
        /// </summary>
        /// <param name="module">The module that the variable is in.</param>
        /// <param name="name">The name of the variable.</param>
        /// <param name="slot">The slot to store it in.</param>
        public void GetVariable(string module, string name, int slot)
        {
            getVariable(this, module, name, slot);
        }

        /// <summary>
        /// Sets the current fiber to be aborted, and uses the value in <paramref name="slot"/> as the
        /// runtime error object.
        /// </summary>
        /// <param name="slot">The slot containing runtime error object.</param>
        public void AbortFiber(int slot)
        {
            abortFiber(this, slot);
        }

        /// <summary>
        /// Stores null in the given slot(s).
        /// </summary>
        /// <param name="slots">The slot(s) to write null to.</param>
        public void SetSlotNull(params int[] slots)
        {
            if (slots == null)
                ThrowHelper.ThrowArgumentNullException(nameof(slots));

            foreach (var slot in slots)
                setSlotNull(this, slot);
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
            ensureSlots(this, count);
        }

        /// <summary>
        /// Returns the number of slots available to the current foreign method.
        /// </summary>
        /// <returns>The number of slots available to the current foreign method.</returns>
        public int GetSlotCount()
        {
            return getSlotCount(this);
        }

        #endregion Slot Count

        #region Handle

        /// <summary>
        /// Creates a handle for the value stored in the given slot.
        /// <para/>
        /// This will prevent the object that is referred to from being garbage collected
        /// until the handle is released by calling <see cref="WrenHandle.Release"/>.
        /// </summary>
        /// <param name="slot">The slot containing the value to create a handle for.</param>
        /// <returns>A handle to the value in the slot.</returns>
        public WrenValueHandle GetSlotHandle(int slot)
        {
            var handle = getSlotHandle(this, slot);
            handle.SetVM(this);
            return handle;
        }

        /// <summary>
        /// Stores the value captured by the given value handle in the given slot.
        /// <para/>
        /// This does not release the handle for the value. You must call <see cref="WrenHandle.Release"/> to do so.
        /// </summary>
        /// <param name="slot">The slot to write the value captured by the value handle to.</param>
        /// <param name="handle">The value handle.</param>
        public void SetSlotHandle(int slot, WrenValueHandle wrenHandle)
        {
            setSlotHandle(this, slot, wrenHandle);
        }

        #endregion Handle

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
            return getSlotBool(this, slot);
        }

        /// <summary>
        /// Stores the given boolean value in the given slot.
        /// </summary>
        /// <param name="slot">The slot to write the boolean to.</param>
        /// <param name="value">The boolean to write to the slot.</param>
        public void SetSlotBool(int slot, bool value)
        {
            setSlotBool(this, slot, value);
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
            var bytesPtr = getSlotBytes(this, slot, out length);

            var bytes = new byte[length];
            Marshal.Copy(bytesPtr, bytes, 0, length);

            return bytes;
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
            var bytesPtr = GCHandle.Alloc(value, GCHandleType.Pinned);

            setSlotBytes(this, slot, bytesPtr.AddrOfPinnedObject(), (nuint)value.Length);

            bytesPtr.Free();
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
            return getSlotDouble(this, slot);
        }

        /// <summary>
        /// Stores the given numeric value in the given slot.
        /// </summary>
        /// <param name="slot">The slot to write the double to.</param>
        /// <param name="value">The double to write to the slot.</param>
        public void SetSlotDouble(int slot, double value)
        {
            setSlotDouble(this, slot, value);
        }

        #endregion Double

        #region Foreign

        private readonly Dictionary<int, object> foreignObjects = new Dictionary<int, object>();

        private int foreignObjectCounter = 0;

        /// <summary>
        /// Reads a foreign object from the given slot.
        /// <para/>
        /// It is an error to call this if the slot does not contain an instance of a foreign class.
        /// </summary>
        /// <param name="slot">The slot to read the foreign object from.</param>
        /// <typeparam name="T">The type of the foreign object.</typeparam>
        /// <returns>The foreign object.</returns>
        public T GetSlotForeign<T>(int slot)
        {
            return (T)GetSlotForeign(slot);
        }

        /// <summary>
        /// Reads a foreign object from the given slot.
        /// <para/>
        /// It is an error to call this if the slot does not contain an instance of a foreign class.
        /// </summary>
        /// <param name="slot">The slot to read the foreign object from.</param>
        /// <returns>The foreign object.</returns>
        public object GetSlotForeign(int slot)
        {
            var dataPtr = getSlotForeign(this, slot);
            var objectId = Marshal.ReadInt32(dataPtr);

            if (!foreignObjects.ContainsKey(objectId))
                ThrowHelper.ThrowKeyNotFoundException("No foreign object with that Id found!");

            return foreignObjects[objectId];
        }

        /// <summary>
        /// Stores the given object in the given slot.
        /// </summary>
        /// <param name="slot">The slot to store the data in.</param>
        /// <param name="foreignObject">The object to store.</param>
        public void SetSlotNewForeign(int slot, object foreignObject)
        {
            var objectId = foreignObjectCounter++;

            foreignObjects.Add(objectId, foreignObject);

            var dataPtr = setSlotNewForeign(this, slot, 0, sizeof(int));
            Marshal.WriteInt32(dataPtr, objectId);
        }

        internal object GetAndFreeForeign(IntPtr dataPtr)
        {
            var objectId = Marshal.ReadInt32(dataPtr);

            if (!foreignObjects.ContainsKey(objectId))
                ThrowHelper.ThrowKeyNotFoundException("No foreign object with that Id found!");

            var foreignObject = foreignObjects[objectId];

            foreignObjects.Remove(objectId);

            return foreignObject;
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
            var contentPtr = getSlotString(this, slot);
            return Marshal.PtrToStringAnsi(contentPtr);
        }

        /// <summary>
        /// Stores the given string in the given slot.
        /// <para/>
        /// The text is copied to a new string within Wren's heap, so you can free memory used by it after this is called.
        /// The length is calculated using strlen(). If the string may contain any null bytes in the middle,
        /// then you should use <see cref="SetSlotBytes(int, byte[])"/> instead.
        /// </summary>
        /// <param name="slot">The slot to write the string to.</param>
        /// <param name="text">The string to write to the slot.</param>
        public void SetSlotString(int slot, string text)
        {
            setSlotString(this, slot, text);
        }

        #endregion String

        #region List

        /// <summary>
        /// Stores a new empty list in the given slot.
        /// </summary>
        /// <param name="slot">The slot to create the new empty list in.</param>
        public void SetSlotNewList(int slot)
        {
            setSlotNewList(this, slot);
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
            insertInList(this, listSlot, index, elementSlot);
        }

        /// <summary>
        /// Reads element <paramref name="index"/> from the list in <paramref name="listSlot"/> and stores it in
        /// <paramref name="elementSlot"/>.
        /// </summary>
        /// <param name="listSlot">The slot containing the list.</param>
        /// <param name="index">The index in the list of the element to retrieve.</param>
        /// <param name="elementSlot">The slot to place the retrieved list element into.</param>
        public void GetListElement(int listSlot, int index, int elementSlot)
        {
            getListElement(this, listSlot, index, elementSlot);
        }

        /// <summary>
        /// Sets the value stored at <paramref name="index"/> in the list at <paramref name="listSlot"/>,
        /// to the value from <paramref name="elementSlot"/>.
        /// </summary>
        /// <param name="listSlot">The slot containing the list.</param>
        /// <param name="index">The index in the list where the element should be placed.</param>
        /// <param name="elementSlot">The slot containing the element to place into the list.</param>
        public void SetListElement(int listSlot, int index, int elementSlot)
        {
            setListElement(this, listSlot, index, elementSlot);
        }

        /// <summary>
        /// Returns the number of elements in the list stored in <paramref name="slot"/>.
        /// </summary>
        /// <param name="slot">The slot containing the list.</param>
        /// <returns>The number of elements in the list.</returns>
        public int GetListCount(int slot)
        {
            return getListCount(this, slot);
        }

        #endregion List

        #region Map

        /// <summary>
        /// Stores a new empty map in <paramref name="slot"/>.
        /// </summary>
        /// <param name="slot">The slot to place the new map into.</param>
        public void SetSlotNewMap(int slot)
        {
            setSlotNewMap(this, slot);
        }

        /// <summary>
        /// Returns the number of entries in the map stored in <paramref name="slot"/>.
        /// </summary>
        /// <param name="slot">The slot containing the map.</param>
        public int GetMapCount(int slot)
        {
            return getMapCount(this, slot);
        }

        /// <summary>
        /// Returns true if the key in <paramref name="keySlot"/> is found in the map placed in <paramref name="mapSlot"/>.
        /// </summary>
        /// <param name="mapSlot">The slot containing the map.</param>
        /// <param name="keySlot">The slot containing the key to look for.</param>
        public bool GetMapContainsKey(int mapSlot, int keySlot)
        {
            return getMapContainsKey(this, mapSlot, keySlot);
        }

        /// <summary>
        /// Retrieves a value with the key in <paramref name="keySlot"/> from the map in <paramref name="mapSlot"/> and
        /// stores it in <paramref name="valueSlot"/>.
        /// </summary>
        /// <param name="mapSlot">The slot containing the map.</param>
        /// <param name="keySlot">The slot containing the key to look for.</param>
        /// <param name="valueSlot">The slot to store the found value in.</param>
        public void GetMapValue(int mapSlot, int keySlot, int valueSlot)
        {
            getMapValue(this, mapSlot, keySlot, valueSlot);
        }

        /// <summary>
        /// Takes the value stored at <paramref name="valueSlot"/> and inserts it into the map stored
        /// at <paramref name="mapSlot"/> with key <paramref name="keySlot"/>.
        /// </summary>
        /// <param name="mapSlot">The slot containing the map.</param>
        /// <param name="keySlot">The slot containing the key to use.</param>
        /// /// <param name="keySlot">The slot containing the value to use.</param>
        public void SetMapValue(int mapSlot, int keySlot, int valueSlot)
        {
            setMapValue(this, mapSlot, keySlot, valueSlot);
        }

        /// <summary>
        /// Removes a value from the map in <paramref name="mapSlot"/>, with the key from <paramref name="keySlot"/>,
        /// and place it in <paramref name="removedValueSlot"/>. If not found, <paramref name="removedValueSlot"/> is
        /// set to null, the same behaviour as the Wren Map API.
        /// </summary>
        /// <param name="mapSlot">The slot containing the map.</param>
        /// <param name="keySlot">The slot containing the key to look for.</param>
        /// <param name="removedValueSlot">The slot to place the removed value in, or null if the key is not found.</param>
        public void RemoveMapValue(int mapSlot, int keySlot, int removedValueSlot)
        {
            removeMapValue(this, mapSlot, keySlot, removedValueSlot);
        }

        #endregion Map

        #endregion Slot Interactions

        #endregion Managed Wrappers

        #region Native Bindings

        [DllImport(WrenLib, EntryPoint = "wrenGetVersionNumber", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getVersionNumber();

        [DllImport(WrenLib, EntryPoint = "wrenInterpret", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenInterpretResult interpret([In] WrenVM vm, [MarshalAs(UnmanagedType.LPStr), In] string module, [MarshalAs(UnmanagedType.LPStr), In] string source);

        #region Slot Interactions

        [DllImport(WrenLib, EntryPoint = "wrenGetSlotType", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenType getSlotType([In] WrenVM vm, [In] int slot);

        [DllImport(WrenLib, EntryPoint = "wrenGetVariable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void getVariable([In] WrenVM vm,
            [MarshalAs(UnmanagedType.LPStr), In] string module, [MarshalAs(UnmanagedType.LPStr), In] string name, [In] int slot);

        [DllImport(WrenLib, EntryPoint = "wrenHasVariable", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool hasVariable([In] WrenVM vm, [MarshalAs(UnmanagedType.LPStr), In] string module, [MarshalAs(UnmanagedType.LPStr), In] string name);

        [DllImport(WrenLib, EntryPoint = "wrenHasModule", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool hasModule([In] WrenVM vm, [MarshalAs(UnmanagedType.LPStr), In] string module);

        [DllImport(WrenLib, EntryPoint = "wrenSetSlotNull", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotNull([In] WrenVM vm, [In] int slot);

        #region Slot Count

        [DllImport(WrenLib, EntryPoint = "wrenGetSlotCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getSlotCount([In] WrenVM vm);

        [DllImport(WrenLib, EntryPoint = "wrenEnsureSlots", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ensureSlots([In] WrenVM vm, [In] int count);

        #endregion Slot Count

        #region Value

        [DllImport(WrenLib, EntryPoint = "wrenGetSlotHandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenValueHandle getSlotHandle([In] WrenVM vm, [In] int slot);

        [DllImport(WrenLib, EntryPoint = "wrenSetSlotHandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotHandle([In] WrenVM vm, [In] int slot, [In] WrenHandle wrenHandle);

        [DllImport(WrenLib, EntryPoint = "wrenReleaseHandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern void releaseHandle([In] WrenVM vm, [In] IntPtr wrenHandle);

        #endregion Value

        #region Bool

        [DllImport(WrenLib, EntryPoint = "wrenGetSlotBool", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool getSlotBool([In] WrenVM vm, [In] int slot);

        [DllImport(WrenLib, EntryPoint = "wrenSetSlotBool", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotBool([In] WrenVM vm, [In] int slot, [In] bool value);

        #endregion Bool

        #region Bytes

        [DllImport(WrenLib, EntryPoint = "wrenGetSlotBytes", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getSlotBytes([In] WrenVM vm, [In] int slot, [Out] out int length);

        [DllImport(WrenLib, EntryPoint = "wrenSetSlotBytes", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotBytes([In] WrenVM vm, [In] int slot, [In] IntPtr bytes, [In] nuint length);

        #endregion Bytes

        #region Double

        [DllImport(WrenLib, EntryPoint = "wrenGetSlotDouble", CallingConvention = CallingConvention.Cdecl)]
        private static extern double getSlotDouble([In] WrenVM vm, [In] int slot);

        [DllImport(WrenLib, EntryPoint = "wrenSetSlotDouble", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotDouble([In] WrenVM vm, [In] int slot, [In] double value);

        #endregion Double

        #region Foreign

        [DllImport(WrenLib, EntryPoint = "wrenGetSlotForeign", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getSlotForeign([In] WrenVM vm, [In] int slot);

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
        /// <param name="handle">The handle.</param>
        /// <param name="slot">The slot to create the new instance of the foreign class in.</param>
        /// <param name="classSlot"></param>
        /// <param name="size">The size of data in bytes.</param>
        /// <returns>A pointer to the foreign object's data.</returns>
        [DllImport(WrenLib, EntryPoint = "wrenSetSlotNewForeign", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr setSlotNewForeign([In] WrenVM vm, [In] int slot, [In] int classSlot, [In] nuint size);

        #endregion Foreign

        #region String

        [DllImport(WrenLib, EntryPoint = "wrenGetSlotString", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getSlotString([In] WrenVM vm, [In] int slot);

        [DllImport(WrenLib, EntryPoint = "wrenSetSlotString", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotString([In] WrenVM vm, [In] int slot, [MarshalAs(UnmanagedType.LPStr), In] string text);

        #endregion String

        #region List

        [DllImport(WrenLib, EntryPoint = "wrenSetSlotNewList", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotNewList([In] WrenVM vm, [In] int slot);

        [DllImport(WrenLib, EntryPoint = "wrenInsertInList", CallingConvention = CallingConvention.Cdecl)]
        private static extern void insertInList([In] WrenVM vm, [In] int listSlot, [In] int index, [In] int elementSlot);

        [DllImport(WrenLib, EntryPoint = "wrenGetListCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getListCount([In] WrenVM vm, [In] int slot);

        [DllImport(WrenLib, EntryPoint = "wrenGetListElement", CallingConvention = CallingConvention.Cdecl)]
        private static extern void getListElement([In] WrenVM vm, [In] int listSlot, [In] int index, [In] int elementSlot);

        [DllImport(WrenLib, EntryPoint = "wrenSetListElement", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setListElement([In] WrenVM vm, [In] int listSlot, [In] int index, [In] int elementSlot);

        #endregion List

        #region Map

        [DllImport(WrenLib, EntryPoint = "wrenSetSlotNewMap", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSlotNewMap([In] WrenVM vm, [In] int slot);

        [DllImport(WrenLib, EntryPoint = "wrenGetMapCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getMapCount([In] WrenVM vm, [In] int slot);

        [DllImport(WrenLib, EntryPoint = "wrenGetMapContainsKey", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool getMapContainsKey([In] WrenVM vm, [In] int mapSlot, [In] int keySlot);

        [DllImport(WrenLib, EntryPoint = "wrenGetMapValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern void getMapValue([In] WrenVM vm, [In] int mapSlot, [In] int keySlot, [In] int valueSlot);

        [DllImport(WrenLib, EntryPoint = "wrenSetMapValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setMapValue([In] WrenVM vm, [In] int mapSlot, [In] int keySlot, [In] int valueSlot);

        [DllImport(WrenLib, EntryPoint = "wrenRemoveMapValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern void removeMapValue([In] WrenVM vm, [In] int mapSlot, [In] int keySlot, [In] int removedValueSlot);

        #endregion Map

        #endregion Slot Interactions

        #region Function Interactions

        [DllImport(WrenLib, EntryPoint = "wrenMakeCallHandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenFunctionHandle makeCallHandle([In] WrenVM vm, [MarshalAs(UnmanagedType.LPStr), In] string signature);

        [DllImport(WrenLib, EntryPoint = "wrenCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrenInterpretResult call([In] WrenVM vm, [In] WrenFunctionHandle callHandle);

        #endregion Function Interactions

        #region VM Lifecycle

        [DllImport(WrenLib, EntryPoint = "wrenNewVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr newVM(WrenUsedConfig config);

        [DllImport(WrenLib, EntryPoint = "wrenCollectGarbage", CallingConvention = CallingConvention.Cdecl)]
        private static extern void collectGarbage([In] WrenVM vm);

        [DllImport(WrenLib, EntryPoint = "wrenFreeVM", CallingConvention = CallingConvention.Cdecl)]
        private static extern void freeVM([In] IntPtr vm);

        [DllImport(WrenLib, EntryPoint = "wrenAbortFiber", CallingConvention = CallingConvention.Cdecl)]
        private static extern void abortFiber([In] WrenVM vm, [In] int slot);

        [DllImport(WrenLib, EntryPoint = "wrenGetUserData", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getUserData([In] WrenVM vm);

        [DllImport(WrenLib, EntryPoint = "wrenSetUserData", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setUserData([In] WrenVM vm, [In] IntPtr userData);

        #endregion VM Lifecycle

        #endregion Native Bindings
    }
}