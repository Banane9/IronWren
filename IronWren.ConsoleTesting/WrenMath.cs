using IronWren.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.ConsoleTesting
{
    public static class WrenMath
    {
        [WrenProperty(PropertyType.Get, "pi")]
        public static void GetPi(WrenVM vm)
        {
            vm.SetSlotDouble(0, Math.PI);
        }

        [WrenMethod("sin", "a")]
        public static void Sin(WrenVM vm)
        {
            vm.SetSlotDouble(0, Math.Sin(vm.GetSlotDouble(1)));
        }
    }
}