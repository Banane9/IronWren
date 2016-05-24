using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Required attribute that marks functions as belonging to a property as get and/or set methods.
    /// <para/>
    /// Must have a single argument of type <see cref="WrenVM"/> and a return type of void.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class WrenPropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the argument for the property on the Wren side.
        /// </summary>
        public string Argument { get; }

        /// <summary>
        /// Gets the name of the property on the Wren side.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the property on the Wren side.
        /// </summary>
        public PropertyType Type { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenPropertyAttribute"/> class
        /// with the given details that marks a property that's not an indexer.
        /// </summary>
        /// <param name="type">Whether it's a getter or setter property.</param>
        /// <param name="name">The name of the property on the Wren side.</param>
        /// <param name="argument">The name of the argument for the property on the Wren side. Ignored for getters.</param>
        public WrenPropertyAttribute(PropertyType type, string name, string argument = "value")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name may not be null or whitespace!", nameof(name));

            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentException("Argument name may not be null or whitespace!", nameof(argument));

            Type = type;
            Name = name;
            Argument = argument;
        }
    }
}