using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public WrenPropertyAttribute(PropertyType type, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                ThrowHelper.ThrowArgumentException("Name may not be null or whitespace!", nameof(name));

            Type = type;
            Name = name;
        }

        internal static IEnumerable<MethodDetails<WrenPropertyAttribute>> GetMethodDetails(Type type)
        {
            var methods = type.GetTypeInfo().DeclaredMethods
                .Select(func => new MethodDetails<WrenPropertyAttribute>(func, func.GetCustomAttribute<WrenPropertyAttribute>()))
                .Where(ctor => ctor.Attribute != null).ToArray();

            var aggregateException = new AggregateException(validateMethods(methods));
            if (aggregateException.InnerExceptions.Count > 0)
                throw aggregateException;

            return methods;
        }

        private static IEnumerable<SignatureInvalidException> validateMethods(IEnumerable<MethodDetails<WrenPropertyAttribute>> methods)
        {
            foreach (var method in methods)
            {
                var parameters = method.Info.GetParameters();

                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(WrenVM) || method.Info.ReturnType != typeof(void))
                    yield return new SignatureInvalidException(method.Info.Name, method.Info.DeclaringType, typeof(WrenConstructorAttribute));
            }
        }
    }
}