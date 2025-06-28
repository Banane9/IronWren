namespace IronWren.Tests.FullyAutoMapper;

public class Base
{
    protected readonly WrenVM vm;
    protected List<string> output = new List<string>();

    public Base()
    {
        vm = new WrenVM();
        vm.Write += (vm, text) => output.Add(text);
        vm.Error += (vm, type, module, line, message) => Console.WriteLine($"Error [{type}] in module [{module}] at line {line}:{Environment.NewLine}{message}");
    }

    protected void InterpretAssert(string source)
    {
        Assert.AreEqual(WrenInterpretResult.Success, vm.Interpret(source));
    }

    protected void LoadVariableToSlot(string variable)
    {
        vm.EnsureSlots(1);
        vm.GetVariable(WrenVM.MainModule, variable, 0);
    }

    protected T LoadInstanceFromSlot<T>(string name)
    {
        LoadVariableToSlot(name);
        var testClassStrInst = vm.GetSlotForeign(0);
        Assert.IsInstanceOfType(testClassStrInst, typeof(T));
        return (T)testClassStrInst;
    }
}
