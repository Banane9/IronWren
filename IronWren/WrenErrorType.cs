namespace IronWren
{
    public enum WrenErrorType
    {
        // A syntax or resolution error detected at compile time.
        Compile,

        // The error message for a runtime error.
        Runtime,

        // One entry of a runtime error's stack trace.
        StackTrace
    }
}