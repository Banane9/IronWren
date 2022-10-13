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
        /// <summary>
        /// Gets the prefix used to designate that the constructor's code is stored in the given field.
        /// </summary>
        public const string FieldPrefix = "field:";

        private const int prefixLength = 6;

        private static readonly MethodInfo setSlotNewForeign = typeof(WrenVM).GetTypeInfo().GetDeclaredMethod("SetSlotNewForeign");

        private static readonly ConstantExpression slot = Expression.Constant(0);

        private static readonly ParameterExpression vmParam = Expression.Parameter(typeof(WrenVM));

        private string code;

        /// <summary>
        /// Gets the named arguments for the constructor on the Wren side.
        /// </summary>
        public string[] Arguments { get; }

        /// <summary>
        /// Sets the Wren code of the constructor which must include the neccessary new lines (\n).
        /// <para/>
        /// The code can also be stored in a field if the name of the field is prefixed
        /// with the <see cref="FieldPrefix"/> ("field:").
        /// </summary>
        public string Code
        {
            set { code = value; }
            get
            {
                ThrowHelper.ThrowNotSupportedException();
                return null;
            }
        }

        /// <summary>
        /// Gets whether the constructor has any Wren code associated with it.
        /// </summary>
        public bool HasCode
        {
            get { return code != null; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WrenConstructorAttribute"/> class
        /// with the given named arguments for the constructor on the Wren side.
        /// </summary>
        /// <param name="arguments">An array containing the named arguments for the constructor.</param>
        public WrenConstructorAttribute(params string[] arguments)
        {
            arguments = arguments ?? new string[0];

            if (!arguments.All(arg => !string.IsNullOrWhiteSpace(arg)))
                ThrowHelper.ThrowArgumentException("Argument names may not be null or whitespace!", nameof(arguments));

            Arguments = arguments;
        }

        internal static ConstructorDetails GetConstructorDetails(Type type)
        {
            var constructor = type.GetTypeInfo().DeclaredConstructors
                .Select(ctor => new ConstructorDetails(ctor, ctor.GetCustomAttributes<WrenConstructorAttribute>().ToArray()))
                .SingleOrDefault(ctor => ctor.Attributes.Length > 0);

            if (constructor == null)
                return null;

            var parameters = constructor.Info.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(WrenVM))
                ThrowHelper.ThrowSignatureInvalidException(constructor.Info.Name, type, typeof(WrenConstructorAttribute));

            return constructor;
        }

        internal static WrenForeignMethod MakeAllocator(Type type)
        {
            var constructor = GetConstructorDetails(type);

            if (constructor == null)
                return null;

            // vm => vm.SetSlotNewForeign(0, new [TTarget](vm));
            return Expression.Lambda<WrenForeignMethod>(
                Expression.Call(vmParam, setSlotNewForeign, slot, Expression.New(constructor.Info, vmParam)),
                vmParam).Compile();
        }

        internal string GetCode(Type type)
        {
            if (!code.StartsWith(FieldPrefix))
                return code;

            return (string)type.GetTypeInfo().GetDeclaredField(code.Substring(prefixLength)).GetValue(null);
        }

        internal sealed class ConstructorDetails
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