using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace IronWren
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.FunctionPtr)]
    public delegate WrenForeignClassMethods WrenBindForeignClass(WrenVM vm, [MarshalAs(UnmanagedType.LPStr)]string module, [MarshalAs(UnmanagedType.LPStr)]string className);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.FunctionPtr)]
    public delegate WrenForeignMethod WrenBindForeignMethod(WrenVM vm, [MarshalAs(UnmanagedType.LPStr)]string module, [MarshalAs(UnmanagedType.LPStr)]string className, bool isStatic, [MarshalAs(UnmanagedType.LPStr)]string signature);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenError(WrenErrorType type, [MarshalAs(UnmanagedType.LPStr)]string module, int line, [MarshalAs(UnmanagedType.LPStr)]string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenFinalizer(IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenForeignMethod(WrenVM vm);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.LPStr)]
    public delegate string WrenLoadModule(WrenVM vm, [MarshalAs(UnmanagedType.LPStr)]string name);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenReallocate(IntPtr memory, int size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WrenWrite(WrenVM vm, [MarshalAs(UnmanagedType.LPStr)]string text);
}