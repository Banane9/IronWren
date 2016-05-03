namespace IronWren
{
    public enum WrenErrorType
    {
        // A syntax or resolution error detected at compile time.
        WREN_ERROR_COMPILE,

        // The error message for a runtime error.
        WREN_ERROR_RUNTIME,

        // One entry of a runtime error's stack trace.
        WREN_ERROR_STACK_TRACE
    }
}