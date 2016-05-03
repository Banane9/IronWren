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

            using (var vm = new WrenVM(config))
            {
                var result = vm.Interpret("System.print(\"Hi from Wren!\")");
            }

            Console.ReadLine();
        }
    }
}