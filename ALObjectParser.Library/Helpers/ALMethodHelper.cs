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
        public static string Write(this ALMethod method)
        {
            var result = "";
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new IndentedTextWriter(stringWriter))
                {
                    bool HasScenario = method.Scenario != null;

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

                    writer.WriteLine($"{(method.IsLocal ? "local " : "")}{method.MethodKind} {method.Name}({parameterTxt}){(!String.IsNullOrEmpty(method.ReturnType) ? ": " + method.ReturnType : "")}");

                    if (String.IsNullOrEmpty(method.Content))
                    {
                        if (HasScenario)
                        {
                            writer.WriteLine(method.Scenario.Feature.Write());
                        }

                        writer.WriteLine("begin");

                        if (HasScenario)
                        {
                            writer.Indent++;

                            writer.WriteLine(method.Scenario.Write());
                            writer.WriteLine("Initialize();");
                            if (method.Scenario.Elements != null)
                            {
                                writer.WriteLine();
                                method.Scenario.Elements
                                    .ToList()
                                    .ForEach(e => {
                                        writer.WriteLine(e.Write());
                                        writer.WriteLine(e.WriteMethod());
                                        writer.WriteLine();
                                    });

                                writer.WriteLine();
                            }

                            writer.Indent--;
                        }

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

        public static string Write(this ITestFeature feature)
        {
            return $"// [FEATURE] {feature.Name}";
        }

        public static string Write(this ITestScenario scenario)
        {
            return $"// [SCENARIO #{scenario.ID:0000}] {scenario.Name}";
        }

        public static string Write(this ITestScenarioElement element)
        {
            return $"// [{element.Type}] {element.Value}";
        }

        public static string WriteMethod(this ITestScenarioElement element)
        {
            return $"{element.Value.SanitizeName()}();";
        }

        public static string SanitizeName(this string name)
        {
            var result = 
                String.Join("", 
                    Regex
                        .Split(name, @"\W", RegexOptions.CultureInvariant)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Select(s => Regex.Replace(s, "^.", m => m.Value.ToUpperInvariant()))
                );

            return result;
        }
    }
}
