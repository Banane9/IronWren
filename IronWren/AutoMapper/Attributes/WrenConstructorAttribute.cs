using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Optional attribute that specifies which arguments the constructor on the Wren side should have.
    /// Must have a single argument of type <see cref="WrenVM"/>.
    /// <para/>
    /// Multiple attributes can be used if there should be multiple constructors on the Wren side.
    /// <para/>
    /// If no attribute is provided, one constructor with zero arguments will be assumed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = true)]
    public sealed class WrenConstructorAttribute : Attribute
    {
        /// <summary>
        /// Gets the default arguments in case no attribute is provided.
        /// <para/>
        /// Constructor without arguments.
        /// </summary>
        public static string[] DefaultArguments { get; } = new string[0];

        /// <summary>
        /// Gets the named arguments for the constructor on the Wren side.
        /// </summary>
        public string[] Arguments { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenConstructorAttribute"/> class
        /// with the given named arguments for the constructor on the Wren side.
        /// </summary>
        /// <param name="arguments">An array containing the named arguments for the constructor.</param>
        public WrenConstructorAttribute(params string[] arguments)
        {
            arguments = arguments ?? new string[0];

            if (!arguments.All(arg => !string.IsNullOrWhiteSpace(arg)))
                throw new ArgumentException("Argument names may not be null or whitespace!", nameof(arguments));

            Arguments = arguments;
        }
    }
}