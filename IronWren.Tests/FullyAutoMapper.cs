using IronWren.FullyAutoMapper;

namespace IronWren.Tests;

[TestClass]
public class FullyAutoMapperTests
{
    private readonly WrenVM vm;
    private List<string> output = new List<string>();

    public FullyAutoMapperTests()
    {
        vm = new WrenVM();
        vm.Write += (vm, text) => output.Add(text);
        vm.Error += (vm, type, module, line, message) => Console.WriteLine($"Error [{type}] in module [{module}] at line {line}:{Environment.NewLine}{message}");
    }

    [TestMethod]
    public void StaticClassInModule()
    {
        vm.FullyAutoMap("math", typeof(Math));

        Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret(
            "import \"math\" for Math\n" +
            "var sin = Math.sin(Math.pi)\n"));

        vm.EnsureSlots(1);
        vm.GetVariable(WrenVM.MainModule, "sin", 0);

        Assert.AreEqual(System.Math.Sin(System.Math.PI), vm.GetSlotDouble(0), 1e-6);
    }

    private static class Math
    {
        public static double Pi => System.Math.PI;

        public static double Sin(double value)
        {
            return System.Math.Sin(value);
        }
    }
}
