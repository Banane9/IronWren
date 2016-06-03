using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Gets thrown when a type that's being mapped by the <see cref="AutoMapper"/> contains duplicate signatures.
    /// </summary>
    public sealed class SignatureExistsException : Exception
    {
        internal SignatureExistsException(string signature, Type type)
            : base($"A signature of [{signature}] already exists on type {type.FullName}!")
        { }
    }
}