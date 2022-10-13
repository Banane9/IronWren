using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Optional attribute to specify the name that the class should have on the Wren side.
    /// <para/>
    /// If no attribute is provided, the name of the class in C# will be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class WrenClassAttribute : Attribute
    {
        /// <summary>
        /// Gets the name that the class should have on the Wren side.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenClassAttribute"/> class
        /// with the given name.
        /// </summary>
        /// <param name="name">The name that the class should have on the Wren side.</param>
        public WrenClassAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                ThrowHelper.ThrowArgumentException("Name can't be null or whitespace!", nameof(name));

            Name = name;
        }
    }
}