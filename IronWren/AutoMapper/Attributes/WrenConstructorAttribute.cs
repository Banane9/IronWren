using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Required attribute that specifies which arguments the constructor on the Wren side should have.
    /// Must have a single argument of type <see cref="WrenVM"/>.
    /// <para/>
    /// Multiple attributes can be used if there should be multiple constructors on the Wren side.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = true)]
    public sealed class WrenConstructorAttribute : Attribute
    {
        private static readonly MethodInfo setSlotNewForeign = typeof(WrenVM).GetTypeInfo().GetDeclaredMethod("SetSlotNewForeign");

        private static readonly ConstantExpression slot = Expression.Constant(0);

        private static readonly ParameterExpression vmParam = Expression.Parameter(typeof(WrenVM));

        /// <summary>
        /// Gets the default arguments in case no attribute is provided.
        /// <para/>
        /// Constructor without arguments.
        /// </summary>
        public static string[] DefaultArguments { get; } = new string[0];

        /// <summary>
        /// Gets the named arguments for the constructor on the Wren side.
        /// </summary>
        public string[] Arguments { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenConstructorAttribute"/> class
        /// with the given named arguments for the constructor on the Wren side.
        /// </summary>
        /// <param name="arguments">An array containing the named arguments for the constructor.</param>
        public WrenConstructorAttribute(params string[] arguments)
        {
            arguments = arguments ?? new string[0];

            if (!arguments.All(arg => !string.IsNullOrWhiteSpace(arg)))
                throw new ArgumentException("Argument names may not be null or whitespace!", nameof(arguments));

            Arguments = arguments;
        }

        internal static ConstructorDetails GetConstructorDetails(Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors
                .Select(ctor => new ConstructorDetails(ctor, ctor.GetCustomAttributes<WrenConstructorAttribute>().ToArray()))
                .SingleOrDefault(ctor => ctor.Attributes.Length > 0);
        }

        internal static WrenForeignMethod MakeAllocator(Type type)
        {
            var constructor = GetConstructorDetails(type);

            if (constructor == null)
                return null;

            var parameters = constructor.Info.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(WrenVM))
                throw new SignatureInvalidException(constructor.Info.Name, type, typeof(WrenConstructorAttribute));

            // vm => vm.SetSlotNewForeign(0, new [TTarget](vm));
            return Expression.Lambda<WrenForeignMethod>(
                Expression.Call(vmParam, setSlotNewForeign, slot, Expression.New(constructor.Info, vmParam)),
                vmParam).Compile();
        }

        internal class ConstructorDetails
        {
            public WrenConstructorAttribute[] Attributes { get; }
            public ConstructorInfo Info { get; }

            public ConstructorDetails(ConstructorInfo info, WrenConstructorAttribute[] attributes)
            {
                Info = info;
                Attributes = attributes;
            }
        }
    }
}