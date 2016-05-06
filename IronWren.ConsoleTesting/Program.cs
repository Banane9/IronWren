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
            config.Write += (v, text) => Console.Write(text);
            config.Error += (type, module, line, message) => Console.WriteLine("Error [" + type + "] in module [" + module + "] at line " + line + ":" + Environment.NewLine + message);

            config.BindForeignMethod += (v, module, className, isStatic, signature) => { Console.WriteLine("BindForeignMethod called: It's called " + signature + " and is it static? " + isStatic); return sayHi; };
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
                    "foreign sayHi(to)\n" +
                    "}\n" +
                    "var test = Test.new()\n" +
                    "test.sayHi(\"wren\")");
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