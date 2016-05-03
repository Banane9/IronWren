IronWren
========

.NET integration for [Wren](https://github.com/munificent/wren).

Currently just in proof of concept stage.

Usage shown in [IronWren.ConsoleTesting/Program.cs](https://github.com/Banane9/IronWren/blob/master/IronWren.ConsoleTesting/Program.cs):

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

![Iron Wrens](http://www.gardens2you.co.uk/4579-thickbox_default/cast-iron-hanging-heart-garden-bird-feeder-wren-bird-ornament-set.jpg)
