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
            var parser = new ALTestCodeunitParser(path);
            var codeunit = parser.Read();

            Console.WriteLine($"Object info: {codeunit.Type} {codeunit.Id} {codeunit.Name}");
            Console.WriteLine("-----------------------------------------------------------");
            foreach (var feature in codeunit.Features)
            {
                Console.WriteLine($"Test Feature: {feature.Name} including {feature.Scenarios.Count()} scenario(s)");
                Console.WriteLine();
                foreach (var scenario in feature.Scenarios)
                {
                    Console.WriteLine($"  Test Scenario: #{scenario.ID} {scenario.Name}");

                    foreach (var element in scenario.Elements)
                    {
                        Console.WriteLine($"    {element.Type}: {element.Value}");
                    }

                    Console.WriteLine();
                }
                Console.WriteLine("-----------------------------------------------------------");
            }

            Console.ReadLine();
        }
    }
}
