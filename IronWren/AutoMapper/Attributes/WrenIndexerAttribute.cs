using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        /// Gets the names of the arguments for the indexer on the Wren side.
        /// </summary>
        public string[] Arguments { get; }

        /// <summary>
        /// Gets the type of the property on the Wren side.
        /// </summary>
        public PropertyType Type { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenIndexerAttribute"/> class
        /// with the given details that marks an indexer.
        /// </summary>
        /// <param name="type">Whether it's a getter or setter indexer.</param>
        /// <param name="arguments">The names of the arguments for the indexer on the Wren side.</param>
        public WrenIndexerAttribute(PropertyType type, params string[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
                ThrowHelper.ThrowArgumentException("Arguments may not be null or empty!", nameof(arguments));

            if (!arguments.All(arg => !string.IsNullOrWhiteSpace(arg)))
                ThrowHelper.ThrowArgumentException("None of the arguments may be null or whitespace!");

            Type = type;
            Arguments = arguments;
        }

        internal static IEnumerable<MethodDetails<WrenIndexerAttribute>> GetMethodDetails(Type type)
        {
            var methods = type.GetTypeInfo().DeclaredMethods
                .Select(func => new MethodDetails<WrenIndexerAttribute>(func, func.GetCustomAttribute<WrenIndexerAttribute>()))
                .Where(ctor => ctor.Attribute != null).ToArray();

            var aggregateException = new AggregateException(validateMethods(methods));
            if (aggregateException.InnerExceptions.Count > 0)
                throw aggregateException;

            return methods;
        }

        private static IEnumerable<SignatureInvalidException> validateMethods(IEnumerable<MethodDetails<WrenIndexerAttribute>> methods)
        {
            foreach (var method in methods)
            {
                var parameters = method.Info.GetParameters();

                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(WrenVM) || method.Info.ReturnType != typeof(void))
                    yield return new SignatureInvalidException(method.Info.Name, method.Info.DeclaringType, typeof(WrenIndexerAttribute));
            }
        }
    }
}