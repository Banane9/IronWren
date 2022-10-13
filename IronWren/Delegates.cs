using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

// Stop CodeMaid from reorganizing the file
#if DEBUG
#endif

namespace IronWren
{
    #region WrenBindForeignClass

    /// <summary>
    /// The callback Wren uses to find a foreign class and get its foreign methods.
    /// <para/>
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
        IntPtr vm, [MarshalAs(UnmanagedType.LPStr)] string module, [MarshalAs(UnmanagedType.LPStr)] string className);

    #endregion WrenBindForeignClass

    #region WrenBindForeignMethod

    /// <summary>
    /// The callback Wren uses to find a foreign method and bind it to a class.
    /// <para/>
    /// When a foreign method is declared in a class, this will be called with the
    /// foreign method's module, class, and signature when the class body is
    /// executed. It should return a pointer to the foreign function that will be
    /// bound to that method.
    /// <para/>
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

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.FunctionPtr)]
    internal delegate WrenForeignMethodInternal WrenBindForeignMethodInternal(
        IntPtr vm, [MarshalAs(UnmanagedType.LPStr)] string module,
        [MarshalAs(UnmanagedType.LPStr)] string className, bool isStatic, [MarshalAs(UnmanagedType.LPStr)] string signature);

    #endregion WrenBindForeignMethod

    #region WrenFinalizer

    /// <summary>
    /// The callback invoked when the garbage collector is about to collect a foreign object.
    /// <para/>
    /// This may be null if the foreign class does not need to finalize.
    /// </summary>
    /// <param name="foreignObject">The foreign object about to be collected.</param>
    public delegate void WrenFinalizer(object foreignObject);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WrenFinalizerInternal(IntPtr data);

    #endregion WrenFinalizer

    #region WrenForeignMethod

    /// <summary>
    /// A function callable from Wren code, but implemented in C#.
    /// </summary>
    /// <param name="vm">The instance of the VM that is calling the method.</param>
    public delegate void WrenForeignMethod(WrenVM vm);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WrenForeignMethodInternal(IntPtr vm);

    #endregion WrenForeignMethod

    #region WrenLoadModule

    /// <summary>
    /// The callback Wren uses to load a module.
    /// <para/>
    /// Since Wren does not talk directly to the file system, it relies on the
    /// embedder to physically locate and read the source code for a module. The
    /// first time an import appears, Wren will call this and pass in the name of
    /// the module being imported. The VM should return the source code for that
    /// module.
    /// <para/>
    /// This will only be called once for any given module name. Wren caches the
    /// result internally so subsequent imports of the same module will use the
    /// previous source and not call this.
    /// <para/>
    /// If a module with the given name could not be found by the embedder, it
    /// should return NULL and Wren will report that as a runtime error.
    /// </summary>
    /// <param name="vm">The instance of the VM that is calling the method.</param>
    /// <param name="name">The name of the module to load.</param>
    /// <returns>The source code of the module.</returns>
    public delegate WrenLoadModuleResult WrenLoadModule(WrenVM vm, string name);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate WrenLoadModuleResultInternal WrenLoadModuleInternal(IntPtr vm, [MarshalAs(UnmanagedType.LPStr)] string name);

    public delegate void WrenLoadModuleCallback<T>(WrenVM vm, string name, WrenLoadModuleResult<T> result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WrenLoadModuleCallbackInternal(IntPtr vm, [MarshalAs(UnmanagedType.LPStr)] string name, WrenLoadModuleResultInternal result);

    /// <summary>
    /// Gives the host a chance to canonicalize the imported module name,
    /// potentially taking into account the (previously resolved) name of the module
    /// that contains the import. Typically, this is used to implement relative
    /// imports.
    /// </summary>
    /// <param name="vm">The instance of the VM that is calling the method.</param>
    /// <param name="importer">The name of the module that the import is occurring within.</param>
    /// <param name="name">The name of the module that is being imported</param>
    /// <returns>The resolved module name that will be passed to <see cref="WrenLoadModule"/></returns>
    public delegate string WrenResolveModule(WrenVM vm, string importer, string name);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.LPStr)]
    internal delegate string WrenResolveModuleInternal(IntPtr vm, [MarshalAs(UnmanagedType.LPStr)] string importer, [MarshalAs(UnmanagedType.LPStr)] string name);

    #endregion WrenLoadModule

    /// <summary>
    /// Called to report an error to the user.
    /// <para/>
    /// An error detected during compile time is reported by calling this once with <see cref="WrenErrorType.Compile"/>,
    /// the name of the module and line where the error occurs, and the compiler's error message.
    /// <para/>
    /// A runtime error is reported by calling this once with <see cref="WrenErrorType.Runtime"/>,
    /// no module or line, and the runtime error's message.
    /// After that, a series of <see cref="WrenErrorType.StackTrace"/> calls are made for each line in the stack trace.
    /// Each of those has the module and line where the method or function is defined and [message] is the name of the method or function.
    /// </summary>
    /// <param name="type">The type of error being reported.</param>
    /// <param name="module">The name of the module that the error occured in.</param>
    /// <param name="line">The line number of the error. Negative (-1) if not applicable.</param>
    /// <param name="message">The error's message.</param>
    public delegate void WrenError(WrenVM vm, WrenErrorType type, [MarshalAs(UnmanagedType.LPStr)] string module, int line, [MarshalAs(UnmanagedType.LPStr)] string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WrenErrorInternal(IntPtr vm, WrenErrorType type, [MarshalAs(UnmanagedType.LPStr)] string module, int line, [MarshalAs(UnmanagedType.LPStr)] string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr WrenReallocateInternal(IntPtr memory, nuint size, IntPtr userData);

    #region WrenWrite

    /// <summary>
    /// The callback Wren uses to display text when `System.print()` or the other
    /// related functions are called.
    /// <para/>
    /// If this is `NULL`, Wren discards any printed text.
    /// </summary>
    /// <param name="vm">The instance of the VM that is calling the method.</param>
    /// <param name="text">The text to be printed.</param>
    public delegate void WrenWrite(WrenVM vm, string text);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WrenWriteInternal(IntPtr vm, [MarshalAs(UnmanagedType.LPStr)] string text);

    #endregion WrenWrite
}