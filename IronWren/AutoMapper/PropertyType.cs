using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Defines whether the property-method is a getter or setter.
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        /// Getter.
        /// </summary>
        Get,

        /// <summary>
        /// Setter.
        /// </summary>
        Set
    }
}