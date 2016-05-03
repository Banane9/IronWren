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
            config.initialHeapSize = 200000000;
            //config.write += (v, text) => Console.WriteLine(text);

            var vm = new WrenVM(config);

            var result = vm.Interpret("System.print(\"Hi!\")");

            Console.ReadLine();
        }
    }
}