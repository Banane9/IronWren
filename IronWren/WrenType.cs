namespace IronWren
{
    /// <summary>
    /// The type of an object stored in a slot.
    /// <para/>
    /// This is not necessarily the object's *class*, but instead its low level representation type.
    /// </summary>
    public enum WrenType
    {
        /// <summary>
        /// A boolean.
        /// </summary>
        Bool,

        /// <summary>
        /// A number (double).
        /// </summary>
        Number,

        /// <summary>
        /// An instance of a foreign class.
        /// </summary>
        Foreign,

        /// <summary>
        /// A list.
        /// </summary>
        List,

        /// <summary>
        /// A map.
        /// </summary>
        Map,

        /// <summary>
        /// No value.
        /// </summary>
        Null,

        /// <summary>
        /// A string or byte array.
        /// </summary>
        String,

        /// <summary>
        /// The object is of a type that isn't accessible by the C API.
        /// </summary>
        Unknown
    }
}