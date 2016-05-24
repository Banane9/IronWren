using IronWren.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.ConsoleTesting
{
    internal class Program
    {
        private static void alloc(WrenVM vm)
        {
            Console.WriteLine("Allocator called!");
            vm.SetSlotNewForeign(0, 0, 1);
        }

        private static void Main(string[] args)
        {
            var config = new WrenConfig();
            config.Write += (vm, text) => Console.Write(text);
            config.Error += (type, module, line, message) => Console.WriteLine($"Error [{type}] in module [{module}] at line {line}:{Environment.NewLine}{message}");

            config.LoadModule += (vm, module) => $"System.print(\"Module [{module}] loaded!\")";

            config.BindForeignMethod += (vm, module, className, isStatic, signature) =>
            {
                Console.WriteLine($"BindForeignMethod called: It's called {signature}, is part of {className} and is {(isStatic ? "static" : "not static")}.");
                return (signature == "sayHi(_)" ? sayHi : (WrenForeignMethod)null);
            };
            config.BindForeignClass += (vm, module, className) => new WrenForeignClassMethods { Allocate = alloc };

            using (var vm = new WrenVM(config))
            {
                var result = vm.Interpret("System.print(\"Hi from Wren!\")");

                result = vm.Interpret("var helloTo = Fn.new { |name|\n" +
                    "System.print(\"Hello, %(name)!\")\n" +
                    "}");

                result = vm.Interpret("helloTo.call(\"IronWren\")");

                var someFnHandle = vm.MakeCallHandle("call(_)");

                vm.EnsureSlots(2);
                vm.GetVariable(WrenVM.InterpetModule, "helloTo", 0);
                vm.SetSlotString(1, "foreign method");
                result = vm.Call(someFnHandle);

                result = vm.Interpret("foreign class Test {\n" +
                    "construct new() { }\n" +
                    "isForeign { true }\n" +
                    "foreign sayHi(to)\n" +
                    "}\n" +
                    "var test = Test.new()\n" +
                    "test.sayHi(\"wren\")\n" +
                    "\n" +
                    "import \"TestModule\"\n");

                vm.EnsureSlots(1);
                vm.GetVariable(WrenVM.InterpetModule, "test", 0);
                result = vm.Call(vm.MakeCallHandle("isForeign"));
                var isTestClassForeign = vm.GetSlotBool(0);

                Console.WriteLine("Test class is foreign: " + isTestClassForeign);

                vm.AutoMap(typeof(WrenMath));
                vm.Interpret("System.print(\"The sine of pi is: %(WrenMath.sin(WrenMath.pi))!\")");
                Console.WriteLine($"And C# says it's: {Math.Sin(Math.PI)}");
            }

            Console.ReadLine();
        }

        private static void sayHi(WrenVM vm)
        {
            var to = vm.GetSlotString(1);
            Console.WriteLine("Foreign method says hi to " + to + "!");
        }
    }
}