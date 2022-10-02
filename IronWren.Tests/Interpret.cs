namespace IronWren.Tests;

[TestClass]
public class Interpret
{
    private readonly WrenVM vm;

    public Interpret()
    {
        vm = new WrenVM();
    }

    [TestMethod]
    public void Success()
    {
        var result = vm.Interpret("var x = 2");
        Assert.AreEqual(WrenInterpretResult.Success, result);
    }

    [TestMethod]
    public void CompileError()
    {
        var result = vm.Interpret("var x = ");
        Assert.AreEqual(WrenInterpretResult.CompileError, result);
    }

    [TestMethod]
    public void RuntimeError()
    {
        var result = vm.Interpret("Fiber.abort()");
        Assert.AreEqual(WrenInterpretResult.RuntimeError, result);
    }
}
