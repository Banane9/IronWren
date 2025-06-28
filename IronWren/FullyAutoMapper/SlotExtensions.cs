using System;
using System.Linq.Expressions;

namespace IronWren.FullyAutoMapper
{
    public static class SlotExtensions
    {

        private static Expression SetSlotNullable(ParameterExpression vm, Expression getterExpr, ParameterExpression getterResult, Expression ifFalse)
        {
            return Expression.Block(
                variables: [getterResult],
                Expression.Assign(getterResult, getterExpr),
                Expression.IfThenElse(
                    test: Expression.Equal(getterResult, Expression.Constant(null)),
                    ifTrue: Expression.Call(
                        instance: vm,
                        methodName: nameof(WrenVM.SetSlotNull),
                        null,
                        Expression.Constant(new int[] { 0 })
                        ),
                    ifFalse: ifFalse
                    ));
        }

        private static Expression GetSlotNullable(ParameterExpression vm, ConstantExpression slotExpr, Expression ifFalse)
        {
            var slotTypeVAr = Expression.Variable(typeof(WrenType), "slotType");
            return Expression.Condition(
                    test: Expression.Equal(
                        Expression.Call(vm, methodName: nameof(WrenVM.GetSlotType), null, slotExpr),
                        Expression.Constant(WrenType.Null)),
                    ifTrue: Expression.Constant(null, ifFalse.Type),
                    ifFalse: ifFalse
                    );
        }

        public static Expression SetSlotExpression(Type methodReturnType, ParameterExpression vm, Expression getterExpr)
        {
            Expression func;
            if (methodReturnType == typeof(void))
            {
                // Functions return null by default in wren, let's follow that convention
                func = Expression.Block(
                    getterExpr,
                    Expression.Call(
                        instance: vm,
                        methodName: nameof(WrenVM.SetSlotNull),
                        null,
                        Expression.Constant(new int[] { 0 })
                        )
                    );
            }
            else if (methodReturnType == typeof(double)) //double
            {
                func = Expression.Call(
                    instance: vm,
                    methodName: nameof(WrenVM.SetSlotDouble),
                    null,
                    Expression.Constant(0), getterExpr
                    );
            }
            else if (methodReturnType == typeof(int) 
                  || methodReturnType == typeof(sbyte)
                  || methodReturnType == typeof(byte)
                  || methodReturnType == typeof(short)
                  || methodReturnType == typeof(ushort)
                  || methodReturnType == typeof(uint)
                  || methodReturnType == typeof(long)
                  || methodReturnType == typeof(ulong)
                  || methodReturnType == typeof(float)
                  || methodReturnType == typeof(decimal)
                  )
            {
                // Castable to double
                func = Expression.Call(
                    instance: vm,
                    methodName: nameof(WrenVM.SetSlotDouble),
                    null,
                    Expression.Constant(0), Expression.Convert(getterExpr, typeof(double))
                    );
            }
            else if (methodReturnType == typeof(bool))
            {
                func = Expression.Call(
                    instance: vm,
                    methodName: nameof(WrenVM.SetSlotBool),
                    null,
                    Expression.Constant(0), getterExpr
                    );
            }
            else if (methodReturnType == typeof(string))
            {
                var getterResult = Expression.Variable(typeof(string), "getterResult");
                func = SetSlotNullable(vm, getterExpr, getterResult,
                    ifFalse: Expression.Call(
                        instance: vm,
                        methodName: nameof(WrenVM.SetSlotString),
                        null,
                        Expression.Constant(0), getterResult
                        ));
            }
            else if (methodReturnType == typeof(byte[]))
            {
                var getterResult = Expression.Variable(typeof(byte[]), "getterResult");
                func = SetSlotNullable(vm, getterExpr, getterResult,
                    ifFalse: Expression.Call(
                        instance: vm,
                        methodName: nameof(WrenVM.SetSlotBytes),
                        null,
                        Expression.Constant(0), getterResult
                        ));
            }
            else if (methodReturnType.IsValueType)
            {
                func = Expression.Block(
                    // It would be better to keep handles to defined types and use SetSlotHandle?
                    Expression.Call(
                        instance: vm,
                        methodName: nameof(WrenVM.GetVariable),
                        null,
                        Expression.Constant(WrenVM.MainModule), Expression.Constant(methodReturnType.Name), Expression.Constant(0)
                        ),
                    Expression.Call(
                        instance: vm,
                        methodName: nameof(WrenVM.SetSlotNewForeign),
                        null,
                        Expression.Constant(0), getterExpr
                    )
                );
            }
            else // Nullable foreign
            {
                var getterResult = Expression.Variable(typeof(object), "getterResult");
                func = SetSlotNullable(vm, getterExpr, getterResult,
                    ifFalse: Expression.Block(
                        // It would be better to keep handles to defined types and use SetSlotHandle?
                        Expression.Call(
                            instance: vm,
                            methodName: nameof(WrenVM.GetVariable),
                            null,
                            Expression.Constant(WrenVM.MainModule), Expression.Constant(methodReturnType.Name), Expression.Constant(0)
                            ),
                        Expression.Call(
                            instance: vm,
                            methodName: nameof(WrenVM.SetSlotNewForeign),
                            null,
                            Expression.Constant(0), getterResult
                        )
                    ));
            }
            return func;
        }

        public static Expression GetSlotExpression(ParameterExpression vm, int slot, Type parameterType)
        {
            var slotExpr = Expression.Constant(slot);
            // TODO: maybe add GetSlotType and check to ensure it matches?
            if (parameterType == typeof(bool))
            {
                return Expression.Call(vm, methodName: nameof(WrenVM.GetSlotBool), null, slotExpr);
            }
            else if (parameterType == typeof(double))
            {
                return Expression.Call(vm, methodName: nameof(WrenVM.GetSlotDouble), null, slotExpr);
            }
            else if (parameterType == typeof(int))
            {
                return Expression.Convert(
                    Expression.Call(vm, nameof(WrenVM.GetSlotDouble), null, slotExpr),
                    parameterType
                        );
            }
            else if (parameterType == typeof(string))
            {
                return GetSlotNullable(vm, slotExpr,
                    ifFalse: Expression.Call(vm, nameof(WrenVM.GetSlotString), null, slotExpr));
            }
            else if (parameterType == typeof(byte[]))
            {
                return GetSlotNullable(vm, slotExpr,
                    ifFalse: Expression.Call(vm, nameof(WrenVM.GetSlotBytes), null, slotExpr));
            }
            else if (parameterType.IsValueType)
            {
                return Expression.Call(vm, nameof(WrenVM.GetSlotForeign), null, slotExpr);
            }
            else
            {
                return GetSlotNullable(vm, slotExpr,
                    ifFalse: Expression.Call(vm, nameof(WrenVM.GetSlotForeign), null, slotExpr)
                    );
            }
        }

        public static void SetSlotValue(WrenVM vm, int slot, object obj)
        {
            switch (obj)
            {
                case null:
                    vm.SetSlotNull(slot);
                    break;

                case sbyte sbyteVal:
                    vm.SetSlotDouble(slot, sbyteVal);
                    break;

                case byte byteVal:
                    vm.SetSlotDouble(slot, byteVal);
                    break;

                case short shortVal:
                    vm.SetSlotDouble(slot, shortVal);
                    break;

                case ushort ushortVal:
                    vm.SetSlotDouble(slot, ushortVal);
                    break;

                case int intVal:
                    vm.SetSlotDouble(slot, intVal);
                    break;

                case uint uintVal:
                    vm.SetSlotDouble(slot, uintVal);
                    break;

                case long longVal:
                    vm.SetSlotDouble(slot, longVal);
                    break;

                case ulong ulongVal:
                    vm.SetSlotDouble(slot, ulongVal);
                    break;

                case float floatVal:
                    vm.SetSlotDouble(slot, floatVal);
                    break;

                case double doubleVal:
                    vm.SetSlotDouble(slot, doubleVal);
                    break;

                case decimal decimalVal:
                    vm.SetSlotDouble(slot, (double)decimalVal);
                    break;

                case string stringVal:
                    vm.SetSlotString(slot, stringVal);
                    break;

                case byte[] byteArrayVal:
                    vm.SetSlotBytes(slot, byteArrayVal);
                    break;

                case bool boolVal:
                    vm.SetSlotBool(slot, boolVal);
                    break;

                default:
                    // It would be better to keep handles to defined types and use SetSlotHandle
                    vm.GetVariable(WrenVM.MainModule, obj.GetType().Name, 0);
                    vm.SetSlotNewForeign(0, obj);
                    break;
            }
        }

        public static object GetSlotValue(WrenVM vm, int slot, Type type)
        {
            var slotType = vm.GetSlotType(slot);

            object result;
            switch (slotType)
            {
                case WrenType.Bool:
                    result = vm.GetSlotBool(slot);
                    break;

                case WrenType.Number:
                    double dbl = vm.GetSlotDouble(slot);
                    if (type == typeof(sbyte))
                    {
                        result = (sbyte)dbl;
                    }
                    else if (type == typeof(byte))
                    {
                        result = (byte)dbl;
                    }
                    else if (type == typeof(short))
                    {
                        result = (short)dbl;
                    }
                    else if (type == typeof(ushort))
                    {
                        result = (ushort)dbl;
                    }
                    else if (type == typeof(int))
                    {
                        result = (int)dbl;
                    }
                    else if (type == typeof(uint))
                    {
                        result = (uint)dbl;
                    }
                    else if (type == typeof(long))
                    {
                        result = (long)dbl;
                    }
                    else if (type == typeof(ulong))
                    {
                        result = (ulong)dbl;
                    }
                    else if (type == typeof(decimal))
                    {
                        result = (decimal)dbl;
                    }
                    else
                    {
                        result = dbl;
                    }
                    break;

                case WrenType.Foreign:
                    result = vm.GetSlotForeign(slot);
                    break;

                case WrenType.List:
                    throw new NotImplementedException($"Slot type {slotType} not implemented.");

                case WrenType.Map:
                    throw new NotImplementedException($"Slot type {slotType} not implemented.");

                case WrenType.Null:
                    result = null;
                    return result;

                case WrenType.String:
                    if (type == typeof(string))
                    {
                        result = vm.GetSlotString(slot);
                    }
                    else
                    {
                        result = vm.GetSlotBytes(slot);
                    }
                    break;

                case WrenType.Unknown:
                    throw new Exception($"Slot has 'Unknown' slot type, which can not be retrieved");

                default:
                    throw new NotImplementedException($"Slot type {slotType} not implemented.");
            }

            if (result.GetType() != type)
            {
                throw new Exception($"Type should be {type}, but was {result.GetType()}");
            }

            return result;
        }
    }
}
