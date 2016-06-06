namespace IronWren
{
    /// <summary>
    /// Lists the possible errors reported by the <see cref="WrenVM"/>.
    /// </summary>
    public enum WrenErrorType
    {
        /// <summary>
        /// A syntax or resolution error detected at compile time.
        /// </summary>
        Compile,

        /// <summary>
        /// The error message for a runtime error.
        /// </summary>
        Runtime,

        /// <summary>
        /// One entry of a runtime error's stack trace.
        /// </summary>
        StackTrace
    }
}