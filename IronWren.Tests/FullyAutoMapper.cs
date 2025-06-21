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
    public void InstanceIndex()
    {
        vm.FullyAutoMap<MyList>();

        Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret("var lst = MyList.new()\nlst.add(7)\nlst.add(8)\nlst.add(9)\nlst[1] = 10\nSystem.print(\"MyList (%(lst[0]), %(lst[1]), %(lst[2]))\")"));
        Assert.AreEqual("MyList (7, 10, 9)", output[0]);
        output.Clear();
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

    [TestMethod]
    public void StaticClassMethodDouble()
    {
        vm.FullyAutoMap("math", typeof(SupaMath));

        Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret(
            "import \"math\" for SupaMath\n" +
            "var sin = SupaMath.pi()\n"));

        vm.EnsureSlots(1);
        vm.GetVariable(WrenVM.MainModule, "sin", 0);

        Assert.AreEqual(System.Math.PI, vm.GetSlotDouble(0), 1e-6);
    }

    [TestMethod]
    public void StaticClassMethodInt()
    {
        vm.FullyAutoMap("math", typeof(SupaMath));

        Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret(
            "import \"math\" for SupaMath\n" +
            "var sin = SupaMath.fifty()\n"));

        vm.EnsureSlots(1);
        vm.GetVariable(WrenVM.MainModule, "sin", 0);

        Assert.AreEqual(50, vm.GetSlotDouble(0), 1e-6);
    }

    [TestMethod]
    public void StaticClassMethodString()
    {
        vm.FullyAutoMap("math", typeof(SupaMath));

        Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret(
            "import \"math\" for SupaMath\n" +
            "var sin = SupaMath.fruit()\n"));

        vm.EnsureSlots(1);
        vm.GetVariable(WrenVM.MainModule, "sin", 0);

        Assert.AreEqual("Banana", vm.GetSlotString(0));
    }

    private static class SupaMath
    {
        public static double Pi() => System.Math.PI;
        public static int Fifty() => 50;
        public static string Fruit() => "Banana";
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

    private class MyList
    {
        private readonly List<int> _list;

        public MyList()
        {
            _list = new List<int>();
        }

        public int this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public void Add(int item)
        {
            _list.Add(item);
        }
    }


}
