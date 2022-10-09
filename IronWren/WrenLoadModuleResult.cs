using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

// Stop CodeMaid from reorganizing the file
#if DEBUG
#endif

namespace IronWren
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WrenLoadModuleResultInternal
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string Source;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public readonly WrenLoadModuleCallbackInternal OnComplete;

        private readonly IntPtr UserData;

        public WrenLoadModuleResultInternal(string source, WrenLoadModuleCallbackInternal onComplete)
        {
            Source = source;
            OnComplete = onComplete;
            UserData = IntPtr.Zero;
        }
    }

    public class WrenLoadModuleResult
    {
        public string Source { get; set; }

        internal virtual WrenLoadModuleResultInternal GetStruct()
        {
            return new WrenLoadModuleResultInternal(Source, null);
        }
    }

    public class WrenLoadModuleResult<T> : WrenLoadModuleResult
    {
        public T UserData { get; set; }

        public WrenLoadModuleCallback<T> OnComplete { get; set; }

        internal override WrenLoadModuleResultInternal GetStruct()
        {
            void onComplete(IntPtr vm, string name, WrenLoadModuleResultInternal result)
            {
                OnComplete?.Invoke(WrenVM.GetVM(vm), name, this);
            }

            return new WrenLoadModuleResultInternal(Source, onComplete);
        }
    }
}