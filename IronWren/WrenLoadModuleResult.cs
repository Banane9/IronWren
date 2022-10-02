using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IronWren
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WrenLoadModuleResultInternal
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string source;

        public IntPtr onComplete;

        public IntPtr userData;
    }

    public class WrenLoadModuleResult
    {
        public string Source { get; set; }
    }
}
