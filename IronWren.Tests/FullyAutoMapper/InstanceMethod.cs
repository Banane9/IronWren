using IronWren.FullyAutoMapper;

namespace IronWren.Tests.FullyAutoMapper;

[TestClass]
public class InstanceMethod : Base
{
    [TestMethod]
    public void DoubleReturn()
    {
        vm.FullyAutoMap(typeof(InstanceTestClass));
        InterpretAssert("var inst = InstanceTestClass.new()");
        InterpretAssert("var val = inst.pi()");
        LoadVariableToSlot("val");
        Assert.AreEqual(System.Math.PI, vm.GetSlotDouble(0));
    }

    [TestMethod]
    public void IntReturn()
    {
        vm.FullyAutoMap(typeof(InstanceTestClass));
        InterpretAssert("var inst = InstanceTestClass.new()");
        InterpretAssert("var val = inst.fifty()");
        LoadVariableToSlot("val");
        Assert.AreEqual(50, (int)vm.GetSlotDouble(0));
    }

    [TestMethod]
    public void StringReturn()
    {
        vm.FullyAutoMap(typeof(InstanceTestClass));
        InterpretAssert("var inst = InstanceTestClass.new()");
        InterpretAssert("var val = inst.fruit()");
        LoadVariableToSlot("val");
        Assert.AreEqual("Banana", vm.GetSlotString(0));
    }

    [TestMethod]
    public void StringReturnNull()
    {
        vm.FullyAutoMap(typeof(InstanceTestClass));
        InterpretAssert("var inst = InstanceTestClass.new()");
        InterpretAssert("var val = inst.fruitNull()");
        LoadVariableToSlot("val");
        Assert.AreEqual(WrenType.Null, vm.GetSlotType(0));
    }

    [TestMethod]
    public void BoolTrueReturn()
    {
        vm.FullyAutoMap(typeof(InstanceTestClass));
        InterpretAssert("var inst = InstanceTestClass.new()");
        InterpretAssert("var val = inst.trueReturn()");
        LoadVariableToSlot("val");
        Assert.AreEqual(true, vm.GetSlotBool(0));
    }

    [TestMethod]
    public void BoolFalseReturn()
    {
        vm.FullyAutoMap(typeof(InstanceTestClass));
        InterpretAssert("var inst = InstanceTestClass.new()");
        InterpretAssert("var val = inst.falseReturn()");
        LoadVariableToSlot("val");
        Assert.AreEqual(false, vm.GetSlotBool(0));
    }

    private class InstanceTestClass
    {
        public double Pi() => System.Math.PI;
        public int Fifty() => 50;
        public string Fruit() => "Banana";
        public string? FruitNull() => null;
        public bool TrueReturn() => true;
        public bool FalseReturn() => false;
    }

    [TestMethod]
    public void DoubleParam()
    {
        vm.FullyAutoMap(typeof(InstanceParamTestClass));
        InterpretAssert("var inst = InstanceParamTestClass.new()");
        InterpretAssert("inst.setDouble(8.3)");
        var inst = LoadInstanceFromSlot<InstanceParamTestClass>("inst");
        Assert.AreEqual(8.3, inst.GetDouble());
    }

    [TestMethod]
    public void IntParam()
    {
        vm.FullyAutoMap(typeof(InstanceParamTestClass));
        InterpretAssert("var inst = InstanceParamTestClass.new()");
        InterpretAssert("inst.setInt(34)");
        var inst = LoadInstanceFromSlot<InstanceParamTestClass>("inst");
        Assert.AreEqual(34, inst.GetInt());
    }

    [TestMethod]
    public void StringParam()
    {
        vm.FullyAutoMap(typeof(InstanceParamTestClass));
        InterpretAssert("var inst = InstanceParamTestClass.new()");
        InterpretAssert("inst.setString(\"zebra\")");
        var inst = LoadInstanceFromSlot<InstanceParamTestClass>("inst");
        Assert.AreEqual("zebra", inst.GetString());
    }

    [TestMethod]
    public void StringParamNull()
    {
        vm.FullyAutoMap(typeof(InstanceParamTestClass));
        InterpretAssert("var inst = InstanceParamTestClass.new()");
        InterpretAssert("inst.setString(null)");
        var inst = LoadInstanceFromSlot<InstanceParamTestClass>("inst");
        Assert.AreEqual(null, inst.GetString());
    }

    [TestMethod]
    public void BoolTrueParam()
    {
        vm.FullyAutoMap(typeof(InstanceParamTestClass));
        InterpretAssert("var inst = InstanceParamTestClass.new()");
        InterpretAssert("inst.setBool(true)");
        var inst = LoadInstanceFromSlot<InstanceParamTestClass>("inst");
        Assert.AreEqual(true, inst.GetBool());
    }

    [TestMethod]
    public void BoolFalseParam()
    {
        vm.FullyAutoMap(typeof(InstanceParamTestClass));
        InterpretAssert("var inst = InstanceParamTestClass.new()");
        InterpretAssert("inst.setBool(false)");
        var inst = LoadInstanceFromSlot<InstanceParamTestClass>("inst");
        Assert.AreEqual(false, inst.GetBool());
    }

    [TestMethod]
    public void MultiParam()
    {
        vm.FullyAutoMap(typeof(InstanceParamTestClass));
        InterpretAssert("var inst = InstanceParamTestClass.new()");
        InterpretAssert("inst.setMulti(\"hippo\", 18, 1.79)");
        var inst = LoadInstanceFromSlot<InstanceParamTestClass>("inst");
        Assert.AreEqual("hippo", inst.GetString());
        Assert.AreEqual(18, inst.GetInt());
        Assert.AreEqual(1.79, inst.GetDouble());
    }

    [TestMethod]
    public void ParamAndReturn()
    {
        vm.FullyAutoMap(typeof(InstanceParamTestClass));
        InterpretAssert("var inst = InstanceParamTestClass.new()");
        InterpretAssert("var val = inst.setAndReturn(2.4)");
        LoadVariableToSlot("val");
        Assert.AreEqual(5.4, vm.GetSlotDouble(0));
    }

    private class InstanceParamTestClass
    {
        private double _double;
        private int _int;
        private string? _string;
        private bool _bool;
        public double GetDouble() => _double;
        public void SetDouble(double v) => _double = v;

        public int GetInt() => _int;
        public void SetInt(int v) => _int = v;

        public string? GetString() => _string;
        public void SetString(string v) => _string = v;

        public bool GetBool() => _bool;
        public void SetBool(bool v) => _bool = v;

        public void SetMulti(string param1, int param2, double param3)
        {
            _string = param1;
            _int = param2;
            _double = param3;
        }


        public double SetAndReturn(double valueIn)
        {
            return valueIn + 3;
        }
    }
}
