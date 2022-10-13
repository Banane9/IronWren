﻿using IronWren.AutoMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace IronWren.FullyAutoMapper
{
    /// <summary>
    /// Contains the bindings generated by the AutoMapper.
    /// </summary>
    internal sealed class FullyAutoForeignClass
    {
        private static readonly MethodInfo genericGetSlotForeign = typeof(WrenVM).GetTypeInfo()
                    .GetDeclaredMethods(nameof(WrenVM.GetSlotForeign)).Single(method => method.IsGenericMethod);

        private static readonly ConstantExpression slot = Expression.Constant(0);
        private static readonly ParameterExpression vmParam = Expression.Parameter(typeof(WrenVM));

        private readonly WrenForeignClassMethods classMethods;

        private readonly Dictionary<string, WrenForeignMethod> functions = new Dictionary<string, WrenForeignMethod>();

        private readonly MethodCallExpression getSlotForeign;

        private readonly TypeInfo target;

        /// <summary>
        /// Gets the <see cref="WrenForeignMethod"/>s that are part of the class.
        /// <para/>
        /// Includes everything (methods, properties, indexers).
        /// </summary>
        public ReadOnlyDictionary<string, WrenForeignMethod> Functions { get; }

        /// <summary>
        /// Gets the name of the class on the Wren side.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the Wren source code for the foreign class.
        /// </summary>
        public string Source { get; }

        public FullyAutoForeignClass(Type target)
        {
            this.target = target.GetTypeInfo();

            // Abstract + Sealed = Static
            if (this.target.IsAbstract && !this.target.IsSealed)
                ThrowHelper.ThrowArgumentException("The target type can't be abstract!", nameof(target));

            if (this.target.ContainsGenericParameters)
                ThrowHelper.ThrowArgumentException("The target type can't have remaining generic parameters!", nameof(target));

            getSlotForeign = Expression.Call(vmParam, genericGetSlotForeign.MakeGenericMethod(target), slot);

            Functions = new ReadOnlyDictionary<string, WrenForeignMethod>(functions);

            var classAttribute = this.target.GetCustomAttribute<WrenClassAttribute>();
            Name = classAttribute?.Name ?? this.target.Name;

            classMethods = new WrenForeignClassMethods
            {
                Allocate = MakeAllocator(),
            };

            Source = generateSource();

            System.Diagnostics.Debug.WriteLine(Source);
        }

        internal WrenForeignClassMethods Bind()
        {
            return classMethods;
        }

        private WrenForeignMethod MakeAllocator()
        {
            // TODO: Optimise with Compiled Expressions
            var constructor = target.GetConstructors().FirstOrDefault();
            return new WrenForeignMethod(vm =>
            {
                var prms = constructor.GetParameters();
                var args = new object[prms.Length];
                for (int i = 0; i < prms.Length; i++)
                {
                    var prm = prms[i];
                    args[i] = SlotExtensions.GetSlotValue(vm, i + 1, prm.ParameterType);
                }
                var result = Activator.CreateInstance(target.AsType(), args);
                vm.SetSlotNewForeign(0, result);
            });
        }

        private void addSignature(string signature, MethodInfo method)
        {
            if (functions.ContainsKey(signature))
                ThrowHelper.ThrowSignatureExistsException(signature, target.AsType());

            functions.Add(signature, getInvoker(method));
        }

        private string generateSource()
        {
            var sourceBuilder = new StringBuilder();

            sourceBuilder.AppendLine($"foreign class {Name} {{");

            makeConstructors(sourceBuilder);
            makeProperties(sourceBuilder);
            makeMethods(sourceBuilder);

            //foreach (var field in WrenCodeAttribute.GetFields(target.AsType()))
            //    sourceBuilder.AppendLine((string)field.GetValue(null));

            sourceBuilder.AppendLine("}");

            return sourceBuilder.ToString();
        }

        private WrenForeignMethod getInvoker(MethodInfo method)
        {
            // TODO: Optimise with Compiled Expressions
            return new WrenForeignMethod(vm =>
            {
                var prms = method.GetParameters();
                var caller = method.IsStatic ? null : vm.GetSlotForeign(0);
                var args = new object[prms.Length];
                for (int i = 0; i < prms.Length; i++)
                {
                    var prm = prms[i];
                    args[i] = SlotExtensions.GetSlotValue(vm, i + 1, prm.ParameterType);
                }
                var result = method.Invoke(caller, args);
                SlotExtensions.SetSlotValue(vm, 0, result);
            });
        }

        private void makeConstructors(StringBuilder sourceBuilder)
        {
            var constructor = target.GetConstructors().FirstOrDefault(shouldNotIgnore);

            if (constructor == null)
                return;

            sourceBuilder.AppendLine(FullyAutoDefinition.MakeConstructor(constructor) + " {}");
        }

        private void makeMethods(StringBuilder sourceBuilder)
        {
            foreach (var method in target.GetMethods().Where(shouldNotIgnore))
            {
                var signature = Signature.MakeMethod(StringCase.ToCamelCase(method.Name), method.GetParameters().Length);
                addSignature(signature, method);

                sourceBuilder.AppendLine(FullyAutoDefinition.MakeMethod(method));
            }
        }

        private void makeProperties(StringBuilder sourceBuilder)
        {
            foreach (var property in target.GetProperties().Where(shouldNotIgnore))
            {
                var numIndexParameters = property.GetIndexParameters().Length;
                if (numIndexParameters > 0)
                {
                    // Indexer
                    var definitions = FullyAutoDefinition.MakeIndexer(property);
                    if (definitions.getter != null)
                    {
                        addSignature(Signature.MakeIndexer(PropertyType.Get, numIndexParameters), property.GetGetMethod());
                        sourceBuilder.AppendLine(definitions.getter);
                    }
                    if (definitions.setter != null)
                    {
                        addSignature(Signature.MakeIndexer(PropertyType.Set, numIndexParameters), property.GetSetMethod());
                        sourceBuilder.AppendLine(definitions.setter);
                    }
                }
                else
                {
                    // Property
                    string propName = StringCase.ToCamelCase(property.Name);
                    var definitions = FullyAutoDefinition.MakeProperty(property);
                    if (definitions.getter != null)
                    {
                        addSignature(Signature.MakeProperty(PropertyType.Get, propName), property.GetGetMethod());
                        sourceBuilder.AppendLine(definitions.getter);
                    }
                    if (definitions.setter != null)
                    {
                        addSignature(Signature.MakeProperty(PropertyType.Set, propName), property.GetSetMethod());
                        sourceBuilder.AppendLine(definitions.setter);
                    }
                }
            }
        }

        private bool shouldNotIgnore(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttribute<WrenIgnoreAttribute>() == null;
        }
    }
}
