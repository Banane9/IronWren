using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace IronWren
{
    #region WrenBindForeignClass

    /// <summary>
    /// The callback Wren uses to find a foreign class and get its foreign methods.
    ///
    /// When a foreign class is declared, this will be called with the class's
    /// module and name when the class body is executed. It should return the
    /// foreign functions used to allocate and (optionally) finalize the bytes
    /// stored in the foreign object when an instance is created.
    /// </summary>
    /// <param name="vm">The instance of the VM that is calling the method.</param>
    /// <param name="module">The name of the module that the class is declared in.</param>
    /// <param name="className">The name of the class.</param>
    /// <returns>The methods used to allocate and optionally finalize the bytes of the foreign object.</returns>
    public delegate WrenForeignClassMethods WrenBindForeignClass(WrenVM vm, string module, string className);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr WrenBindForeignClassInternal(IntPtr vm, IntPtr module, IntPtr className);

    #endregion WrenBindForeignClass

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.FunctionPtr)]
    public delegate WrenForeignMethod WrenBindForeignMethod(IntPtr vm, [MarshalAs(UnmanagedType.LPStr)]string module, [MarshalAs(UnmanagedType.LPStr)]string className, bool isStatic, [MarshalAs(UnmanagedType.LPStr)]string signature);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenError(WrenErrorType type, [MarshalAs(UnmanagedType.LPStr)]string module, int line, [MarshalAs(UnmanagedType.LPStr)]string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenFinalizer(IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenForeignMethod(IntPtr vm);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.LPStr)]
    public delegate string WrenLoadModule(IntPtr vm, [MarshalAs(UnmanagedType.LPStr)]string name);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenReallocate(IntPtr memory, int size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenWrite(IntPtr vm, [MarshalAs(UnmanagedType.LPStr)]string text);
}