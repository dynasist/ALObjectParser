using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ALObjectParser.Library
{
    public static class ALMethodHelper
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string Write(this ALMethod method)
        {
            var result = "";
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new IndentedTextWriter(stringWriter))
                {
                    writer.Indent++;
                    writer.WriteLine();

                    var parameterTxt = "";
                    if (method.Parameters.Count > 0)
                    {
                        parameterTxt = String.Join(';', method.Parameters.Select(s => $"{(s.IsVar ? "var " : "")}{s.Name}: {s.Type}"));
                    }

                    writer.WriteLine($"{(method.IsLocal ? "local " : "")}{method.MethodKind} {(method.Name.Contains(" ") ? $"\"{method.Name}\"": method.Name)}({parameterTxt}){(!String.IsNullOrEmpty(method.ReturnType) ? ": " + method.ReturnType : "")}");

                    if (String.IsNullOrEmpty(method.Content))
                    {
                        writer.WriteLine("begin");
                        writer.WriteLine("end;");
                    }
                    else
                    {
                        writer.WriteLine(method.Content);
                    }

                    writer.Indent--;

                    result = stringWriter.ToString().Replace("}", "").Trim();
                }
            }

            return result;
        }
    }
}
