namespace IronWren.Tests;

/// <summary>
/// Tests of basic functionality of the VM
/// </summary>
[TestClass]
public class General
{
    [TestMethod]
    public void Version()
    {
        // This will need to be updated each time the supported wren version is changed
        var version = WrenVM.GetVersionNumber();
        Assert.AreEqual(4000, version);
    }
}
