using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Required attribute that marks functions as belonging to a indexer as get and/or set methods.
    /// <para/>
    /// Must have a single argument of type <see cref="WrenVM"/> and a return type of void.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class WrenIndexerAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the setter-argument for the indexer on the Wren side.
        /// </summary>
        public string Argument { get; }

        /// <summary>
        /// Gets the names of the arguments for the indexer on the Wren side.
        /// </summary>
        public string[] Arguments { get; }

        /// <summary>
        /// Gets the name of the property on the Wren side.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the property on the Wren side.
        /// </summary>
        public PropertyType Type { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenIndexerAttribute"/> class
        /// with the given details that marks an indexer.
        /// </summary>
        /// <param name="type">Whether it's a getter or setter indexer.</param>
        /// <param name="name">The name of the property on the Wren side.</param>
        /// <param name="argument">The name of the setter-argument for the indexer on the Wren side. Ignored for getters.</param>
        /// <param name="arguments">The names of the arguments for the indexer on the Wren side.</param>
        public WrenIndexerAttribute(PropertyType type, string argument, params string[] arguments)
        {
            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentException("Argument name may not be null or whitespace!", nameof(argument));

            if (arguments == null || arguments.Length == 0)
                throw new ArgumentException("Arguments may not be null or empty!", nameof(arguments));

            if (!arguments.All(arg => !string.IsNullOrWhiteSpace(arg)))
                throw new ArgumentException("None of the arguments may be null or whitespace!");

            Type = type;
            Argument = argument;
            Arguments = arguments;
        }
    }
}