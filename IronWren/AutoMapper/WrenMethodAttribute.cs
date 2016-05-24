using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Required attribute that marks functions as to be mirrored on the Wren side.
    /// <para/>
    /// Must have a single argument of type <see cref="WrenVM"/> and a return type of void.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class WrenMethodAttribute : Attribute
    {
        /// <summary>
        /// Gets the names of the arguments on the Wren side.
        /// </summary>
        public string[] Arguments { get; }

        /// <summary>
        /// Gets the name of the method on the Wren side.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenMethodAttribute"/> class
        /// with the given details about the method.
        /// </summary>
        /// <param name="name">The name of the method on the Wren side.</param>
        /// <param name="arguments">The names of the arguments on the Wren side.</param>
        public WrenMethodAttribute(string name, params string[] arguments)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name may not be null or whitespace!", nameof(name));

            Name = name;
            Arguments = arguments ?? new string[0];
        }
    }
}