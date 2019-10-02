using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ALObjectParser.Library
{
    public class ALTestCodeunitParser : ALObjectParser
    {
        public ALTestCodeunitParser(): base()
        {
            ALObject = new ALCodeunit();
        }

        public ALTestCodeunitParser(string FilePath) : base(FilePath)
        {
            ALObject = new ALCodeunit();
        }


        public override string OnWrite(IALObject Target, List<ITestFeature> Features = null)
        {
            var result = "";
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new IndentedTextWriter(stringWriter))
                {
                    var methods = Target.Methods.Select(s => s.Write());
                    var methodTxt = String.Join("\r\n", methods);

                    writer.WriteLine($"{Target.Type} {Target.Id} {Target.Name}");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine(methodTxt);
                    writer.Indent--;
                    writer.WriteLine("}");                    
                }

                result = stringWriter.ToString();
            }

            return result;
        }
    }
}
