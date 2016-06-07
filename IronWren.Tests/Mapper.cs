using IronWren.AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.Tests
{
    [TestClass]
    public sealed class Mapper
    {
        private static string finalizerOutput;
        private readonly WrenVM vm;
        private List<string> output = new List<string>();

        public Mapper()
        {
            vm = new WrenVM();
            vm.Config.Write += (vm, text) => output.Add(text);
            vm.Config.Error += (type, module, line, message) => Console.WriteLine($"Error [{type}] in module [{module}] at line {line}:{Environment.NewLine}{message}");
        }

        [TestMethod]
        public void NormalClass()
        {
            vm.AutoMap<WrenVector>();

            Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret("{\nvar vec = Vector.new(1, 2)\nvec.print()\n}"));
            Assert.AreEqual("Vector (1, 2)", output[0]);
            output.Clear();

            vm.CollectGarbage();
            Assert.AreEqual("Vector (1, 2) finalized!", finalizerOutput);

            Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret("var length = Vector.getLength(Vector.new(0, 2))"));
            vm.EnsureSlots(1);
            vm.GetVariable(WrenVM.InterpetModule, "length", 0);
            Assert.AreEqual(2, vm.GetSlotDouble(0));
        }

        [TestMethod]
        public void StaticClassInModule()
        {
            vm.AutoMap("math", typeof(WrenMath));

            Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret("var length = 1\n" + // this shouldn't be needed
                "import \"math\" for Math\n" +
                "var sin = Math.sin(Math.pi)\n"));

            vm.EnsureSlots(1);
            vm.GetVariable(WrenVM.InterpetModule, "sin", 0);

            Assert.AreEqual(Math.Sin(Math.PI), vm.GetSlotDouble(0), 1e-6);
        }

        [TestMethod]
        public void WithTwoClasses()
        {
            NormalClass();
            StaticClassInModule();
        }

        [WrenClass("Math")]
        private static class WrenMath
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

        [WrenClass("Vector")]
        private sealed class WrenVector
        {
            private const string constructorCode = "this.x = x";

            [WrenCode]
            private const string print = "print() {\nSystem.print(\"Vector (%(x), %(y))\")\n}";

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

            [WrenMethod("getLength", "vector")]
            private static void getLength(WrenVM vm)
            {
                var vector = vm.GetSlotForeign<WrenVector>(1);

                vm.SetSlotDouble(0, Math.Sqrt((vector.x * vector.x) + (vector.y * vector.y)));
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
                finalizerOutput = $"Vector ({x}, {y}) finalized!";
            }
        }
    }
}