IronWren
========

.NET integration for the scripting language [Wren](https://github.com/munificent/wren).

###Features###

- [x] C# style wrapper around Wren's C-API.
- [x] No `IntPtr`s get exposed, making the value- and function-handles typesafe.
- [x] Ability to automatically generate Wren integration code for classes defined in C#.

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

#####AutoMapper#####

The AutoMapper provides the ability to easily create a Wren class from a C# type, only requiring it to be decorated with the right Attributes.

An example can be found in [IronWren.ConsoleTesting/WrenVector.cs](https://github.com/Banane9/IronWren/blob/master/IronWren.ConsoleTesting/WrenVector.cs).

The Attributes are:
- `[WrenClass]` - For optionally specifying an alternative name.
- `[WrenCode]` - For dropping a string field's content into the class source.
- `[WrenConstructor]` - For marking the constructor that should be used by Wren and which overloads should exist there.
- `[WrenFinalizer]` - For marking a method that should be called when the object is GCed by the Wren VM.
- `[WrenIndexer]` - For marking methods that are get/set methods of an indexer.
- `[WrenMethod]` - For marking a method that should be present in Wren and its arguments.
- `[WrenProperty]` - For marking methods that are get/set methods of a property.

The AutoMapper will automatically handle the binding between the methods and Wren.

-------------------------------------------------------------------------------------------------------------------------------

![Iron Wrens](http://www.gardens2you.co.uk/4579-thickbox_default/cast-iron-hanging-heart-garden-bird-feeder-wren-bird-ornament-set.jpg)
