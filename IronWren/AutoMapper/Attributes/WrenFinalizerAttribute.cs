using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Required attribute that marks a function as a class's finalizer.
    /// <para/>
    /// Must have no arguments and a return type of void.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class WrenFinalizerAttribute : Attribute
    {
        private static readonly ParameterExpression objParam = Expression.Parameter(typeof(object));

        internal static WrenFinalizer MakeFinalizer(Type type)
        {
            var finalizeMethod = type.GetRuntimeMethods()
                .SingleOrDefault(method => method.GetCustomAttribute<WrenFinalizerAttribute>() != null);

            if (finalizeMethod == null)
                return null;

            if (finalizeMethod.ReturnType != typeof(void) || finalizeMethod.GetParameters().Length != 0)
                ThrowHelper.ThrowSignatureInvalidException(finalizeMethod.Name, type, typeof(WrenFinalizerAttribute));

            // finalizeObj => ((TTarget)finalizeObj).[finalize]()
            return Expression.Lambda<WrenFinalizer>(
                Expression.Call(Expression.Convert(objParam, type), finalizeMethod),
                objParam).Compile();
        }
    }
}