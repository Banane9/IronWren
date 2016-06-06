using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.Tests
{
    [TestClass]
    public sealed class Functions
    {
        private readonly WrenVM vm;
        private List<string> output = new List<string>();

        public Functions()
        {
            vm = new WrenVM();
            vm.Config.Write += (vm, text) => output.Add(text);
        }

        [TestMethod]
        public void WithArguments()
        {
            Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret("var test = Fn.new { |text|\n" +
                "System.print(text)\n" +
                "}"));

            var fnHandle = vm.MakeCallHandle("call(_)");

            vm.EnsureSlots(2);
            vm.GetVariable(WrenVM.InterpetModule, "test", 0);
            vm.SetSlotString(1, "testing!");

            Assert.AreEqual(WrenInterpretResult.Success, vm.Call(fnHandle));

            Assert.AreEqual("testing!", output[0]);

            vm.ReleaseHandle(fnHandle);
        }

        [TestMethod]
        public void WithoutArguments()
        {
            Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret("var test = Fn.new {\n" +
                "System.print(\"test called!\")\n" +
                "}"));

            var fnHandle = vm.MakeCallHandle("call()");

            vm.EnsureSlots(1);
            vm.GetVariable(WrenVM.InterpetModule, "test", 0);

            Assert.AreEqual(WrenInterpretResult.Success, vm.Call(fnHandle));

            Assert.AreEqual("test called!", output[0]);

            vm.ReleaseHandle(fnHandle);
        }
    }
}