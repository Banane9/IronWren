using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper
{
    /// <summary>
    /// Required attribute that marks a field containing Wren code which will be added into the foreign class definition.
    /// Must be static and of type string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class WrenCodeAttribute : Attribute
    {
        internal static IEnumerable<FieldInfo> GetFields(Type type)
        {
            var fields = type.GetTypeInfo().DeclaredFields
                .Where(field => field.GetCustomAttribute<WrenCodeAttribute>() != null).ToArray();

            var aggregateException = new AggregateException(validateFields(fields));
            if (aggregateException.InnerExceptions.Count > 0)
                throw aggregateException;

            return fields;
        }

        private static IEnumerable<SignatureInvalidException> validateFields(IEnumerable<FieldInfo> fields)
        {
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(string) || !field.IsStatic)
                    yield return new SignatureInvalidException(field.Name, field.DeclaringType, typeof(WrenCodeAttribute));
            }
        }
    }
}