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
            var parser = new ALTestCodeunitParser(@".\test_cu.al");
            var alcu = parser.Read();

            //parser.Write()

            Console.ReadLine();
        }
    }
}
