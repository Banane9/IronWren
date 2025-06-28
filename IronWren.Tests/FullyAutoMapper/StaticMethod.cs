using IronWren.FullyAutoMapper;

namespace IronWren.Tests.FullyAutoMapper;

[TestClass]
public class StaticMethod : Base
{
    [TestMethod]
    public void DoubleReturn()
    {
        vm.FullyAutoMap(typeof(StaticTestClass));
        InterpretAssert("var val = StaticTestClass.pi()");
        LoadVariableToSlot("val");
        Assert.AreEqual(System.Math.PI, vm.GetSlotDouble(0));
    }

    [TestMethod]
    public void IntReturn()
    {
        vm.FullyAutoMap(typeof(StaticTestClass));
        InterpretAssert("var val = StaticTestClass.fifty()");
        LoadVariableToSlot("val");
        Assert.AreEqual(50, (int)vm.GetSlotDouble(0));
    }

    [TestMethod]
    public void StringReturn()
    {
        vm.FullyAutoMap(typeof(StaticTestClass));
        InterpretAssert("var val = StaticTestClass.fruit()");
        LoadVariableToSlot("val");
        Assert.AreEqual("Banana", vm.GetSlotString(0));
    }

    [TestMethod]
    public void StringReturnNull()
    {
        vm.FullyAutoMap(typeof(StaticTestClass));
        InterpretAssert("var val = StaticTestClass.fruitNull()");
        LoadVariableToSlot("val");
        Assert.AreEqual(WrenType.Null, vm.GetSlotType(0));
    }

    [TestMethod]
    public void BoolTrueReturn()
    {
        vm.FullyAutoMap(typeof(StaticTestClass));
        InterpretAssert("var val = StaticTestClass.trueReturn()");
        LoadVariableToSlot("val");
        Assert.AreEqual(true, vm.GetSlotBool(0));
    }

    [TestMethod]
    public void BoolFalseReturn()
    {
        vm.FullyAutoMap(typeof(StaticTestClass));
        InterpretAssert("var val = StaticTestClass.falseReturn()");
        LoadVariableToSlot("val");
        Assert.AreEqual(false, vm.GetSlotBool(0));
    }


    private static class StaticTestClass
    {
        public static double Pi() => System.Math.PI;
        public static int Fifty() => 50;
        public static string Fruit() => "Banana";
        public static string? FruitNull() => null;
        public static bool TrueReturn() => true;
        public static bool FalseReturn() => false;
    }

    [TestMethod]
    public void DoubleParam()
    {
        vm.FullyAutoMap(typeof(StaticParamTestClass));
        InterpretAssert("StaticParamTestClass.setDouble(8.3)");
        Assert.AreEqual(8.3, StaticParamTestClass.GetDouble());
    }

    [TestMethod]
    public void IntParam()
    {
        vm.FullyAutoMap(typeof(StaticParamTestClass));
        InterpretAssert("StaticParamTestClass.setInt(76)");
        Assert.AreEqual(76, StaticParamTestClass.GetInt());
    }

    [TestMethod]
    public void StringParam()
    {
        vm.FullyAutoMap(typeof(StaticParamTestClass));
        InterpretAssert("StaticParamTestClass.setString(\"giraffe\")");
        Assert.AreEqual("giraffe", StaticParamTestClass.GetString());
    }

    [TestMethod]
    public void StringParamNull()
    {
        StaticParamTestClass.SetString("notNUll");
        vm.FullyAutoMap(typeof(StaticParamTestClass));
        InterpretAssert("StaticParamTestClass.setString(null)");
        Assert.AreEqual(null, StaticParamTestClass.GetString());
    }

    [TestMethod]
    public void BoolParamTrue()
    {
        vm.FullyAutoMap(typeof(StaticParamTestClass));
        InterpretAssert("StaticParamTestClass.setBool(true)");
        Assert.AreEqual(true, StaticParamTestClass.GetBool());
    }

    [TestMethod]
    public void BoolParamFalse()
    {
        vm.FullyAutoMap(typeof(StaticParamTestClass));
        InterpretAssert("StaticParamTestClass.setBool(false)");
        Assert.AreEqual(false, StaticParamTestClass.GetBool());
    }

    [TestMethod]
    public void MultiParam()
    {
        vm.FullyAutoMap(typeof(StaticParamTestClass));
        InterpretAssert("StaticParamTestClass.setMulti(\"elephant\", 19, 10.4)");
        Assert.AreEqual("elephant", StaticParamTestClass.GetString());
        Assert.AreEqual(19, StaticParamTestClass.GetInt());
        Assert.AreEqual(10.4, StaticParamTestClass.GetDouble());
    }

    [TestMethod]
    public void ReturnsNullIfVoid()
    {
        vm.FullyAutoMap(typeof(StaticParamTestClass));
        InterpretAssert("var shouldBeNull = StaticParamTestClass.setDouble(8.3)");
        LoadVariableToSlot("shouldBeNull");
        Assert.AreEqual(WrenType.Null, vm.GetSlotType(0));
    }

    [TestMethod]
    public void ParamAndReturn()
    {
        vm.FullyAutoMap(typeof(StaticParamTestClass));
        InterpretAssert("var val = StaticParamTestClass.setAndReturn(2.3)");
        LoadVariableToSlot("val");
        Assert.AreEqual(5.3, vm.GetSlotDouble(0));
    }

    private static class StaticParamTestClass
    {
        private static double __double;
        private static int __int;
        private static string? __string;
        private static bool __bool;
        public static double GetDouble() => __double;
        public static void SetDouble(double v) => __double = v;

        public static int GetInt() => __int;
        public static void SetInt(int v) => __int = v;

        public static string? GetString() => __string;
        public static void SetString(string v) => __string = v;

        public static bool GetBool() => __bool;
        public static void SetBool(bool v) => __bool = v;

        public static void SetMulti(string param1, int param2, double param3)
        {
            __string = param1;
            __int = param2;
            __double = param3;
        }


        public static double SetAndReturn(double valueIn)
        {
            return valueIn + 3;
        }
    }
}
