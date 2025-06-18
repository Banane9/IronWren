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
    public void NormalClass()
    {
        vm.FullyAutoMap<Vector>();

        Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret("var vec = Vector.new(1, 2)\nSystem.print(\"Vector (%(vec.x), %(vec.y))\")"));
        Assert.AreEqual("Vector (1, 2)", output[0]);
        output.Clear();

        Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret("vec.add(Vector.new(3, 4))\nSystem.print(\"Vector (%(vec.x), %(vec.y))\")"));
        Assert.AreEqual("Vector (4, 6)", output[0]);
        output.Clear();

        Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret("var length = Vector.getLength(Vector.new(0, 2))"));
        vm.EnsureSlots(1);
        vm.GetVariable(WrenVM.MainModule, "length", 0);
        Assert.AreEqual(2, vm.GetSlotDouble(0));
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

    private class Vector
    {
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public static double GetLength(Vector vec)
        {
            return System.Math.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y));
        }

        public void Add(Vector other)
        {
            X += other.X;
            Y += other.Y;
        }
    }
}
