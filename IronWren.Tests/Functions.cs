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
            vm.Write += (vm, text) => output.Add(text);
            vm.Error += (vm, type, module, line, message) => Console.WriteLine($"Error [{type}] in module [{module}] at line {line}:{Environment.NewLine}{message}");
        }

        [TestMethod]
        public void WithArguments()
        {
            Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret("var test = Fn.new { |text|\n" +
                "System.print(text)\n" +
                "}"));

            var fnHandle = vm.MakeCallHandle("call(_)");

            vm.EnsureSlots(2);
            vm.GetVariable(WrenVM.MainModule, "test", 0);
            vm.SetSlotString(1, "testing!");

            Assert.AreEqual(WrenInterpretResult.Success, vm.Call(fnHandle));

            Assert.AreEqual("testing!", output[0]);

            fnHandle.Dispose();
        }

        [TestMethod]
        public void WithoutArguments()
        {
            Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret("var test = Fn.new {\n" +
                "System.print(\"test called!\")\n" +
                "}"));

            var fnHandle = vm.MakeCallHandle("call()");

            vm.EnsureSlots(1);
            vm.GetVariable(WrenVM.MainModule, "test", 0);

            Assert.AreEqual(WrenInterpretResult.Success, vm.Call(fnHandle));

            Assert.AreEqual("test called!", output[0]);

            fnHandle.Dispose();
        }
    }
}