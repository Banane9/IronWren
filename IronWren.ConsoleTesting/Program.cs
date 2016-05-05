using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.ConsoleTesting
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = new WrenConfig();
            config.Write += (v, text) => Console.Write(text);
            config.Error += (type, module, line, message) => Console.WriteLine("Error [" + type + "] in module [" + module + "] at line " + line + ":" + Environment.NewLine + message);

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
            }

            Console.ReadLine();
        }
    }
}