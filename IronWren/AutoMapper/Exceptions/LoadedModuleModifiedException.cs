using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Gets thrown when an <see cref="AutoMapper"/> generated module gets modified after being loaded by Wren.
    /// </summary>
    public sealed class LoadedModuleModifiedException : Exception
    {
        internal LoadedModuleModifiedException(string moduleName)
            : base($"The AutoMapped module [{moduleName}] was modified after being loaded by Wren!")
        { }
    }
}