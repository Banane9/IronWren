using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Required attribute that marks functions as to be mirrored on the Wren side.
    /// <para/>
    /// Must have a single argument of type <see cref="WrenVM"/> and a return type of void.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
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
                ThrowHelper.ThrowArgumentException("Name may not be null or whitespace!", nameof(name));

            Name = name;
            Arguments = arguments ?? new string[0];
        }

        internal static IEnumerable<MethodDetails<WrenMethodAttribute>> GetMethodDetails(Type type)
        {
            var methods = type.GetTypeInfo().DeclaredMethods
                .Select(func => new MethodDetails<WrenMethodAttribute>(func, func.GetCustomAttribute<WrenMethodAttribute>()))
                .Where(ctor => ctor.Attribute != null).ToArray();

            var aggregateException = new AggregateException(validateMethods(methods));
            if (aggregateException.InnerExceptions.Count > 0)
                throw aggregateException;

            return methods;
        }

        private static IEnumerable<SignatureInvalidException> validateMethods(IEnumerable<MethodDetails<WrenMethodAttribute>> methods)
        {
            foreach (var method in methods)
            {
                var parameters = method.Info.GetParameters();

                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(WrenVM) || method.Info.ReturnType != typeof(void))
                    yield return new SignatureInvalidException(method.Info.Name, method.Info.DeclaringType, typeof(WrenMethodAttribute));
            }
        }
    }
}