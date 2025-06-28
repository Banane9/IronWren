using IronWren.FullyAutoMapper;

namespace IronWren.Tests.FullyAutoMapper;

[TestClass]
public class Constructor : Base
{
    [TestMethod]
    public void NoParameter()
    {
        vm.FullyAutoMap<TestClassNoParam>();
        InterpretAssert("var testClassNoParam = TestClassNoParam.new()");
        _ = LoadInstanceFromSlot<TestClassNoParam>("testClassNoParam");
    }

    private class TestClassNoParam
    {
        public TestClassNoParam()
        {
        }
    }

    [TestMethod]
    public void StringParameter()
    {
        vm.FullyAutoMap<TestClassString>();
        InterpretAssert("var testClassStrInst = TestClassString.new(\"hello there\")");
        var testClassStrInst = LoadInstanceFromSlot<TestClassString>("testClassStrInst");
        Assert.AreEqual("hello there", testClassStrInst.GetParameter());
    }

    [TestMethod]
    public void StringParameterNull()
    {
        vm.FullyAutoMap<TestClassString>();
        InterpretAssert("var testClassStrInst = TestClassString.new(null)");
        var testClassStrInst = LoadInstanceFromSlot<TestClassString>("testClassStrInst");
        Assert.AreEqual(null, testClassStrInst.GetParameter());
    }

    private class TestClassString
    {
        private readonly string _parameter1;

        public TestClassString(string parameter1)
        {
            _parameter1 = parameter1;
        }

        public string GetParameter() => _parameter1;
    }

    [TestMethod]
    public void DoubleParameter()
    {
        vm.FullyAutoMap<TestClassDbl>();
        InterpretAssert("var testClassDblInst = TestClassDbl.new(2.74)");
        var testClassDblInst = LoadInstanceFromSlot<TestClassDbl>("testClassDblInst");
        Assert.AreEqual(2.74, testClassDblInst.GetParameter());
    }

    private class TestClassDbl
    {
        private readonly double _parameter1;

        public TestClassDbl(double parameter1)
        {
            _parameter1 = parameter1;
        }

        public double GetParameter() => _parameter1;
    }

    [TestMethod]
    public void IntParameter()
    {
        vm.FullyAutoMap<TestClassInt>();
        InterpretAssert("var testClassIntInst = TestClassInt.new(75)");
        var testClassIntInst = LoadInstanceFromSlot<TestClassInt>("testClassIntInst");
        Assert.AreEqual(75, testClassIntInst.GetParameter());
    }

    private class TestClassInt
    {
        private readonly int _parameter1;

        public TestClassInt(int parameter1)
        {
            _parameter1 = parameter1;
        }

        public int GetParameter() => _parameter1;
    }

    [TestMethod]
    public void BoolParameterTrue()
    {
        vm.FullyAutoMap<TestClassBool>();

        InterpretAssert("var testClassBoolInstT = TestClassBool.new(true)");
        var testClassBoolInstT = LoadInstanceFromSlot<TestClassBool>("testClassBoolInstT");
        Assert.AreEqual(true, testClassBoolInstT.GetParameter());

    }

    [TestMethod]
    public void BoolParameterFalse()
    {
        vm.FullyAutoMap<TestClassBool>();

        InterpretAssert("var testClassBoolInstF = TestClassBool.new(false)");
        var testClassBoolInstF = LoadInstanceFromSlot<TestClassBool>("testClassBoolInstF");
        Assert.AreEqual(false, testClassBoolInstF.GetParameter());

    }


    private class TestClassBool
    {
        private readonly bool _parameter1;

        public TestClassBool(bool parameter1)
        {
            _parameter1 = parameter1;
        }

        public bool GetParameter() => _parameter1;
    }

    [TestMethod]
    public void MultiParameter()
    {
        vm.FullyAutoMap<TestClassMulti>();
        InterpretAssert("var testClassMultiInst = TestClassMulti.new(\"hi\", 34, 0.81)");
        var testClassIntInst = LoadInstanceFromSlot<TestClassMulti>("testClassMultiInst");
        Assert.AreEqual("hi", testClassIntInst.GetParameter1());
        Assert.AreEqual(34, testClassIntInst.GetParameter2());
        Assert.AreEqual(0.81, testClassIntInst.GetParameter3());
    }

    private class TestClassMulti
    {
        private readonly string _parameter1;
        private readonly int _parameter2;
        private readonly double _parameter3;

        public TestClassMulti(string parameter1, int parameter2, double parameter3)
        {
            _parameter1 = parameter1;
            _parameter2 = parameter2;
            _parameter3 = parameter3;
        }

        public string GetParameter1() => _parameter1;
        public int GetParameter2() => _parameter2;
        public double GetParameter3() => _parameter3;
    }
}
