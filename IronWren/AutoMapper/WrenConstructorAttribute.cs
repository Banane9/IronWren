using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Optional attribute that specifies how many arguments the constructor(s) on the Wren side should have.
    /// <para/>
    /// If no attribute is provided, one constructor with zero arguments will be assumed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public sealed class WrenConstructorAttribute : Attribute
    {
        /// <summary>
        /// Gets the default arguments in case no attribute is provided.
        /// <para/>
        /// One constructor without arguments.
        /// </summary>
        public static string[][] DefaultArguments { get; } = new[] { new string[0] };

        /// <summary>
        /// Gets the different named arguments for the constructor(s) on the Wren side.
        /// </summary>
        public string[][] Arguments { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenConstructorAttribute"/> class
        /// with the given named arguments for the constructor(s) on the Wren side.
        /// </summary>
        /// <param name="arguments">An array of arrays containing the named arguments for the constructor(s).</param>
        public WrenConstructorAttribute(params string[][] arguments)
        {
            arguments = arguments ?? new string[0][];

            if (!arguments.All(args => args.All(arg => arg != null)))
                throw new ArgumentOutOfRangeException(nameof(arguments), "Argument count must be greater or equal to zero!");

            Arguments = arguments;
        }
    }
}