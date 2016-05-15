IronWren
========

.NET integration for the scripting language [Wren](https://github.com/munificent/wren).

###Features###

- [x] C# style wrapper around Wren's C-API.
- [x] No `IntPtr`s get exposed, making it type safe.
- [ ] Ability to automatically generate Wren integration code for classes defined in C#.

###Usage###

Basic usage (more extensively shown in [IronWren.ConsoleTesting/Program.cs](https://github.com/Banane9/IronWren/blob/master/IronWren.ConsoleTesting/Program.cs)):

``` CSharp
private static void Main(string[] args)
{
    var config = new WrenConfig();
    config.Write += (v, text) => Console.Write(text);
    
    using (var vm = new WrenVM(config))
    {
        var result = vm.Interpret("System.print(\"Hi from Wren!\")");
    }
    
    Console.ReadLine();
}
```

-------------------------------------------------------------------------------------------------------------------------------

![Iron Wrens](http://www.gardens2you.co.uk/4579-thickbox_default/cast-iron-hanging-heart-garden-bird-feeder-wren-bird-ornament-set.jpg)
