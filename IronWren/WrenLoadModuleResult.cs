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

    /// <summary>
    /// The result of wren load module
    /// </summary>
    public class WrenLoadModuleResult
    {
        /// <summary>
        /// The source code for the module, or NULL if the module is not found.
        /// </summary>
        public string Source { get; set; }

        internal virtual WrenLoadModuleResultInternal GetStruct()
        {
            return new WrenLoadModuleResultInternal(Source, null);
        }
    }

    /// <summary>
    /// The result of wren load module
    /// </summary>
    public class WrenLoadModuleResult<T> : WrenLoadModuleResult
    {
        /// <summary>
        /// User data
        /// </summary>
        public T UserData { get; set; }

        /// <summary>
        /// An optional callback that will be called once Wren is done with the result.
        /// </summary>
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