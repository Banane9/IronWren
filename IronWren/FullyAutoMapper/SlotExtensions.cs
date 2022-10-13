using System;

namespace IronWren.FullyAutoMapper
{
    public static class SlotExtensions
    {
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
                    break;

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
