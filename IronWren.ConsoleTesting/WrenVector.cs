using IronWren.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.ConsoleTesting
{
    [WrenClass("Vector")]
    public sealed class WrenVector
    {
        private static readonly string constructorCode = "this.x = x";

        [WrenCode]
        private static readonly string print = "print() {\nSystem.print(\"Vector (%(x), %(y))\")\n}";

        private double x;
        private double y;

        public WrenVector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        [WrenConstructor("x", "y", Code = "field:constructorCode")]
        private WrenVector(WrenVM vm)
        {
            y = vm.GetSlotDouble(2);
        }

        [WrenProperty(PropertyType.Get, "x")]
        private void getX(WrenVM vm)
        {
            vm.SetSlotDouble(0, x);
        }

        [WrenProperty(PropertyType.Get, "y")]
        private void getY(WrenVM vm)
        {
            vm.SetSlotDouble(0, y);
        }

        [WrenProperty(PropertyType.Set, "x")]
        private void setX(WrenVM vm)
        {
            x = vm.GetSlotDouble(1);
        }

        [WrenProperty(PropertyType.Set, "y")]
        private void setY(WrenVM vm)
        {
            y = vm.GetSlotDouble(1);
        }

        [WrenFinalizer]
        private void wrenFinalize()
        {
            Console.WriteLine($"Vector ({x}, {y}) finalized!");
        }
    }
}