using IronWren.AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace IronWren.FullyAutoMapper
{
    public static class FullyAutoDefinition
    {
        public static string MakeClass(TypeInfo @class, TypeInfo inherits = null)
        {
            if (@class == null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(@class));
            }

            return Definition.MakeClass(true, @class?.Name ?? @class.Name, inherits?.Name ?? inherits?.Name);
        }

        public static string MakeConstructor(ConstructorInfo constructor)
        {
            var spam = constructor.GetParameters();
            string[] parameters = spam.Select(x => StringCase.ToCamelCase(x.Name)).ToArray();
            return Definition.MakeConstructor(parameters);
        }

        public static (string getter, string setter) MakeIndexer(PropertyInfo indexer)
        {
            if (indexer == null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(indexer));
            }
            string[] parameters = indexer.GetIndexParameters().Select(x => StringCase.ToCamelCase(x.Name)).ToArray();

            string getter = null;
            string setter = null;
            MethodInfo getMethod = indexer.GetGetMethod();
            MethodInfo setMethod = indexer.GetSetMethod();
            if (getMethod != null)
            {
                getter = Definition.MakeIndexer(true, getMethod.IsStatic, PropertyType.Get, parameters);
            }
            if (setMethod != null)
            {
                setter = Definition.MakeIndexer(true, setMethod.IsStatic, PropertyType.Set, parameters);
            }
            return (getter, setter);
        }

        public static string MakeMethod(MethodInfo method)
        {
            if (method == null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(method));
            }

            string[] parameters = method.GetParameters().Select(x => StringCase.ToCamelCase(x.Name)).ToArray();
            return Definition.MakeMethod(true, method.IsStatic, StringCase.ToCamelCase(method.Name), parameters);
        }

        public static (string getter, string setter) MakeProperty(PropertyInfo property)
        {
            if (property == null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(property));
            }

            string getter = null;
            string setter = null;
            MethodInfo getMethod = property.GetGetMethod();
            MethodInfo setMethod = property.GetSetMethod();
            if (getMethod != null)
            {
                getter = Definition.MakeProperty(true, getMethod.IsStatic, PropertyType.Get, StringCase.ToCamelCase(property.Name));
            }
            if (setMethod != null)
            {
                setter = Definition.MakeProperty(true, setMethod.IsStatic, PropertyType.Set, StringCase.ToCamelCase(property.Name));
            }
            return (getter, setter);
        }

    }
}
