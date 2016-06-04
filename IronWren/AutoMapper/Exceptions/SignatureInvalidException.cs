using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Gets thrown when a method decorated with one of the AutoMapper attributes doesn't match the required signature.
    /// </summary>
    public sealed class SignatureInvalidException : Exception
    {
        internal SignatureInvalidException(string name, Type type, Type attribute)
            : base($"The signature of [{name}] of type [{type}] doesn't match the signature required for the {attribute.Name}!")
        { }
    }
}