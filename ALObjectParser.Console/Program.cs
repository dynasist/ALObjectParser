using ALObjectParser.Library;
using System;
using System.IO;
using System.Linq;

namespace ALObjectParser.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args != null && args.Count() > 0 ? args[0] : @".\test_cu.al";
            var codeunit = ALParser.ReadSingle(path);

            Console.WriteLine($"Object info: {codeunit.Type} {codeunit.Id} {codeunit.Name}");
            Console.WriteLine($"Object path: {path}");
            Console.WriteLine("-----------------------------------------------------------");
            foreach (var method in codeunit.Methods)
            {
                Console.WriteLine($"Method: {method.Name} including {method.Parameters.Count()} pparameter(s)");
                Console.WriteLine();
                foreach (var param in method.Parameters)
                {
                    Console.WriteLine($"  Parameter: #{param.Name} {param.TypeDefinition?.Name}");
                }
                Console.WriteLine("-----------------------------------------------------------");
            }

            Console.ReadLine();
        }
    }
}
