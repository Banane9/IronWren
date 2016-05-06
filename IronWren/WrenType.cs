namespace IronWren
{
    /// <summary>
    /// The type of an object stored in a slot.
    /// <para/>
    /// This is not necessarily the object's *class*, but instead its low level representation type.
    /// </summary>
    public enum WrenType
    {
        Bool,
        Number,
        Foreign,
        List,
        Null,
        String,

        /// <summary>
        /// The object is of a type that isn't accessible by the C API.
        /// </summary>
        Unknown
    }
}