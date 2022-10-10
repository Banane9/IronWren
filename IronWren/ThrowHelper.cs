using IronWren.AutoMapper;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace IronWren
{
    internal static class ThrowHelper
    {
        public static void ThrowArgumentException(string message, string paramName = "")
        {
            throw new ArgumentException(message, paramName);
        }

        public static void ThrowInvalidOperationException(string message)
        {
            throw new InvalidOperationException(message);
        }

        internal static void ThrowArgumentNullException(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }

        internal static void ThrowKeyNotFoundException(string message)
        {
            throw new KeyNotFoundException(message);
        }

        internal static void ThrowLoadedModuleModifiedException(string moduleName)
        {
            throw new LoadedModuleModifiedException(moduleName);
        }

        internal static void ThrowNotSupportedException()
        {
            throw new NotSupportedException();
        }

        internal static void ThrowSignatureExistsException(string signature, Type type)
        {
            throw new SignatureExistsException(signature, type);
        }

        internal static void ThrowSignatureInvalidException(string name, Type type, Type attribute)
        {
            throw new SignatureInvalidException(name, type, attribute);
        }
    }
}