using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronWren.AutoMapper.StructureMapping
{
    internal static class NameHelper
    {
        public static string GetSignature(this FieldInfo field, PropertyType type)
        {
            if (type == PropertyType.Get)
                return field.Name;

            return $"{field.Name}=(_)";
        }

        public static string GetSignature(this PropertyInfo property, PropertyType type)
        {
            var indexParameters = property.GetIndexParameters().Select(i => "_").ToArray();

            if (indexParameters.Length > 0)
            {
                if (type == PropertyType.Get)
                    return $"[{string.Join(",", indexParameters)}]";

                return $"[{string.Join(",", indexParameters)}]=(_)";
            }

            if (type == PropertyType.Get)
                return property.Name;

            return $"{property.Name}=(_)";
        }

        public static string GetSignature(this ConstructorInfo constructor)
        {
            return $"new({string.Join(",", constructor.GetParameters().Select(p => "_"))})";
        }

        public static string GetSignature(this MethodInfo method)
        {
            return $"{method.Name}({string.Join(",", method.GetParameters().Select(p => "_"))})";
        }
    }
}