using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Contains methods to generate Wren source definitions for classes and class elements.
    /// </summary>
    public static class Definition
    {
        /// <summary>
        /// Generates a foreign class definition from the given class and an optional class that is being inherited.
        /// </summary>
        /// <param name="class">The class to generate the definition from.</param>
        /// <param name="inherits">The optional class that is being inherited.</param>
        /// <returns>The foreign class definition in Wren.</returns>
        public static string MakeClass(TypeInfo @class, TypeInfo inherits = null)
        {
            if (@class == null)
                ThrowHelper.ThrowArgumentNullException(nameof(@class));

            var classAttribute = @class.GetCustomAttribute<WrenClassAttribute>();
            var inheritsClassAttribute = inherits?.GetCustomAttribute<WrenClassAttribute>();

            return MakeClass(true, classAttribute?.Name ?? @class.Name, inheritsClassAttribute?.Name ?? inherits?.Name);
        }

        /// <summary>
        /// Generates a class definition from the given details.
        /// </summary>
        /// <param name="isForeign">Whether the class is foreign or not.</param>
        /// <param name="name">The name of the class.</param>
        /// <param name="inherits">The optional name of the class that's being inherited.</param>
        /// <returns>The class definition in Wren.</returns>
        public static string MakeClass(bool isForeign, string name, string inherits = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                ThrowHelper.ThrowArgumentException("Name may not be null or whitespace!", nameof(name));

            return $"{(isForeign ? "foreign " : "")}class {name}{(inherits != null ? $" is {inherits}" : "")}";
        }

        /// <summary>
        /// Generates a constructor definition from the given arguments.
        /// </summary>
        /// <param name="arguments">The names of the arguments of the constructor.</param>
        /// <returns>The constructor definition in Wren.</returns>
        public static string MakeConstructor(params string[] arguments)
        {
            arguments = arguments ?? new string[0];

            if (!arguments.All(arg => !string.IsNullOrWhiteSpace(arg)))
                ThrowHelper.ThrowArgumentException("No argument may be null or whitespace!", nameof(arguments));

            return $"construct new({string.Join(", ", arguments)})";
        }

        /// <summary>
        /// Generates a foreign indexer definition from the given <see cref="WrenIndexerAttribute"/> marked method.
        /// </summary>
        /// <param name="indexer">The <see cref="WrenIndexerAttribute"/> marked method to generate the indexer from.</param>
        /// <returns>The foreign indexer definition in Wren.</returns>
        public static string MakeIndexer(MethodInfo indexer)
        {
            if (indexer == null)
                ThrowHelper.ThrowArgumentNullException(nameof(indexer));

            var propertyAttribute = indexer.GetCustomAttribute<WrenIndexerAttribute>();

            if (propertyAttribute == null)
                ThrowHelper.ThrowArgumentException("Method must have a WrenIndexerAttribute!", nameof(indexer));

            return MakeIndexer(true, indexer.IsStatic, propertyAttribute.Type, propertyAttribute.Arguments);
        }

        /// <summary>
        /// Generates an indexer definition from the given details.
        /// </summary>
        /// <param name="isForeign">Whether the indexer is foreign or not.</param>
        /// <param name="isStatic">Whether the indexer is static or not.</param>
        /// <param name="type">Whether it's a getter or setter indexer.</param>
        /// <param name="arguments">The names of the arguments of the indexer.</param>
        /// <returns>The indexer definition in Wren.</returns>
        public static string MakeIndexer(bool isForeign, bool isStatic, PropertyType type, params string[] arguments)
        {
            if (arguments == null)
                ThrowHelper.ThrowArgumentNullException(nameof(arguments));

            if (arguments.Length == 0)
                ThrowHelper.ThrowArgumentException("Indexers must have at least one argument!", nameof(arguments));

            if (!arguments.All(arg => !string.IsNullOrWhiteSpace(arg)))
                ThrowHelper.ThrowArgumentException("No argument may be null or whitespace!", nameof(arguments));

            if (type == PropertyType.Get)
                return $"{(isForeign ? "foreign " : "")}{(isStatic ? "static " : "")}[{string.Join(", ", arguments)}]";

            return $"{(isForeign ? "foreign " : "")}{(isStatic ? "static " : "")}[{string.Join(", ", arguments)}]=(value)";
        }

        /// <summary>
        /// Generates a method definition from the given <see cref="WrenMethodAttribute"/> marked method.
        /// </summary>
        /// <param name="method">The <see cref="WrenMethodAttribute"/> marked method to generate the definition from.</param>
        /// <returns>The method definition in Wren.</returns>
        public static string MakeMethod(MethodInfo method)
        {
            if (method == null)
                ThrowHelper.ThrowArgumentNullException(nameof(method));

            var methodAttribute = method.GetCustomAttribute<WrenMethodAttribute>();

            if (methodAttribute == null)
                ThrowHelper.ThrowArgumentException("Method must have a WrenMethodAttribute!");

            return MakeMethod(true, method.IsStatic, methodAttribute.Name, methodAttribute.Arguments);
        }

        /// <summary>
        /// Generates a method definition from the given details.
        /// </summary>
        /// <param name="isForeign">Whether the method is foreign or not.</param>
        /// <param name="isStatic">Whether the method is static or not.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="arguments">The names of the arguments of the method.</param>
        /// <returns>The method definition in Wren.</returns>
        public static string MakeMethod(bool isForeign, bool isStatic, string name, params string[] arguments)
        {
            if (string.IsNullOrWhiteSpace(name))
                ThrowHelper.ThrowArgumentException("Name may not be null or whitespace!", nameof(name));

            arguments = arguments ?? new string[0];

            if (!arguments.All(arg => !string.IsNullOrWhiteSpace(arg)))
                ThrowHelper.ThrowArgumentException("No argument may be null or whitespace!", nameof(arguments));

            return $"{(isForeign ? "foreign " : "")}{(isStatic ? "static " : "")}{name}({string.Join(", ", arguments)})";
        }

        /// <summary>
        /// Generates a property definition from the given <see cref="WrenPropertyAttribute"/> marked method.
        /// </summary>
        /// <param name="property">The <see cref="WrenPropertyAttribute"/> marked method to generate the definition from.</param>
        /// <returns>The property definition in Wren.</returns>
        public static string MakeProperty(MethodInfo property)
        {
            if (property == null)
                ThrowHelper.ThrowArgumentNullException(nameof(property));

            var propertyAttribute = property.GetCustomAttribute<WrenPropertyAttribute>();

            if (propertyAttribute == null)
                ThrowHelper.ThrowArgumentException("Method must have a WrenPropertyAttribute!");

            return MakeProperty(true, property.IsStatic, propertyAttribute.Type, propertyAttribute.Name);
        }

        /// <summary>
        /// Generates a property definition from the given details.
        /// </summary>
        /// <param name="isForeign">Whether the property is foreign or not.</param>
        /// <param name="isStatic">Whether the property is static or not.</param>
        /// <param name="type">Whether it's a getter or setter property.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The property definition in Wren.</returns>
        public static string MakeProperty(bool isForeign, bool isStatic, PropertyType type, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                ThrowHelper.ThrowArgumentException("Name may not be null or whitespace!", nameof(name));

            if (type == PropertyType.Get)
                return $"{(isForeign ? "foreign " : "")}{(isStatic ? "static " : "")}{name}";

            return $"{(isForeign ? "foreign " : "")}{(isStatic ? "static " : "")}{name}=(value)";
        }
    }
}