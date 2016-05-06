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
    internal delegate WrenForeignClassMethodsInternal WrenBindForeignClassInternal(
        IntPtr vm, [MarshalAs(UnmanagedType.LPStr)]string module, [MarshalAs(UnmanagedType.LPStr)]string className);

    #endregion WrenBindForeignClass

    #region WrenForeignMethod

    /// <summary>
    /// The callback Wren uses to find a foreign method and bind it to a class.
    ///
    /// When a foreign method is declared in a class, this will be called with the
    /// foreign method's module, class, and signature when the class body is
    /// executed. It should return a pointer to the foreign function that will be
    /// bound to that method.
    ///
    /// If the foreign function could not be found, this should return NULL and
    /// Wren will report it as runtime error.
    /// </summary>
    /// <param name="vm">The instance of the VM that is calling the method.</param>
    /// <param name="module">The name of the module that the class is declared in.</param>
    /// <param name="className">The name of the class.</param>
    /// <param name="isStatic">Whether the declared method is static or not.</param>
    /// <param name="signature">The signature of the method.</param>
    /// <returns>The bound function or null if none was found.</returns>
    public delegate WrenForeignMethod WrenBindForeignMethod(WrenVM vm, string module, string className, bool isStatic, string signature);

    /// <summary>
    /// A function callable from Wren code, but implemented in C#.
    /// </summary>
    /// <param name="vm">The instance of the VM that is calling the method.</param>
    public delegate void WrenForeignMethod(WrenVM vm);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.FunctionPtr)]
    internal delegate WrenForeignMethodInternal WrenBindForeignMethodInternal(
        IntPtr vm, [MarshalAs(UnmanagedType.LPStr)]string module,
        [MarshalAs(UnmanagedType.LPStr)]string className, bool isStatic, [MarshalAs(UnmanagedType.LPStr)]string signature);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WrenForeignMethodInternal(IntPtr vm);

    #endregion WrenForeignMethod

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenError(WrenErrorType type, [MarshalAs(UnmanagedType.LPStr)]string module, int line, [MarshalAs(UnmanagedType.LPStr)]string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenFinalizer(IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.LPStr)]
    public delegate string WrenLoadModule(IntPtr vm, [MarshalAs(UnmanagedType.LPStr)]string name);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenReallocate(IntPtr memory, int size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenWrite(IntPtr vm, [MarshalAs(UnmanagedType.LPStr)]string text);
}