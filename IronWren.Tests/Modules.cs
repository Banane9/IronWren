namespace IronWren.Tests;

[TestClass]
public class Modules
{
    private readonly WrenVM vm;

    public Modules()
    {
        vm = new WrenVM();
    }

    [TestMethod]
    public void HasModule()
    {
        Assert.IsFalse(vm.HasModule("myModule"));
        vm.Interpret("myModule", "var otherVar = 1");
        Assert.IsTrue(vm.HasModule("myModule"));
    }

    [TestMethod]
    public void HasVariable()
    {
        Assert.IsFalse(vm.HasModule("myModule"));
        vm.Interpret("myModule", "var otherVar = 1");
        Assert.IsTrue(vm.HasModule("myModule"));

        Assert.IsFalse(vm.HasVariable("myModule", "myVar"));
        vm.Interpret("myModule", "var myVar = 1");
        Assert.IsTrue(vm.HasVariable("myModule", "myVar"));
    }
}