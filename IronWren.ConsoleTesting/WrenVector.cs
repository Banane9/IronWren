using IronWren.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.ConsoleTesting
{
    [WrenClass("Vector")]
    public sealed class WrenVector
    {
        private double x;
        private double y;

        public WrenVector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        [WrenConstructor("x", "y")]
        public WrenVector(WrenVM vm)
            : this(vm.GetSlotDouble(1), vm.GetSlotDouble(2))
        { }

        [WrenProperty(PropertyType.Get, "x")]
        public void GetX(WrenVM vm)
        {
            vm.SetSlotDouble(0, x);
        }

        [WrenProperty(PropertyType.Get, "y")]
        public void GetY(WrenVM vm)
        {
            vm.SetSlotDouble(0, y);
        }

        [WrenProperty(PropertyType.Set, "x")]
        public void SetX(WrenVM vm)
        {
            x = vm.GetSlotDouble(1);
        }

        [WrenProperty(PropertyType.Set, "y")]
        public void SetY(WrenVM vm)
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