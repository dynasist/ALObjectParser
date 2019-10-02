using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ALObjectParser.Library
{
    public static class ALMethodHelper
    {
        public static string Write(this ALMethod method)
        {
            var result = "";
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new IndentedTextWriter(stringWriter))
                {
                    writer.Indent++;
                    writer.WriteLine();
                    if (method.TestMethod)
                    {
                        writer.WriteLine("[Test]");
                    }

                    var parameterTxt = "";
                    if (method.Parameters.Count > 0)
                    {
                        parameterTxt = String.Join(';', method.Parameters.Select(s => $"{(s.IsVar ? "var " : "")}{s.Name}: {s.Type}"));
                    }

                    writer.WriteLine($"{(method.IsLocal ? "local " : "")} {method.MethodKind} {method.Name}({parameterTxt}){(!String.IsNullOrEmpty(method.ReturnType) ? ": " + method.ReturnType : "")}");
                    if (String.IsNullOrEmpty(method.Content))
                    {
                        writer.WriteLine("begin");
                        writer.WriteLine("end;");
                    } 
                    else
                    {
                        writer.WriteLine(method.Content);
                    }

                    writer.WriteLine();
                    writer.Indent--;

                    result = stringWriter.ToString().Replace("}", "");
                }
            }

            return result;
        }
    }
}
