using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren
{
    /// <summary>
    /// Lists the possible results reported by <see cref="WrenVM.Interpret(string)"/>.
    /// </summary>
    public enum WrenInterpretResult
    {
        /// <summary>
        /// Interpretation was successful.
        /// </summary>
        Success,

        /// <summary>
        /// There was a compiletime error during interpretation.
        /// </summary>
        CompileError,

        /// <summary>
        /// There was a runtime error during interpretation.
        /// </summary>
        RuntimeError
    }
}