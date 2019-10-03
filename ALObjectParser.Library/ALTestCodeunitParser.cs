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
            Path = FilePath;
        }

        public ALTestCodeunitParser(ALParserConfig config) : base(config)
        {
            ALObject = new ALCodeunit();
        }

        #region Read from Object

        public override void OnRead(List<string> Lines, IALObject Target)
        {
            if (Target.Methods == null)
                return;

            if (Target.Methods.Count == 0)
                return;

            var testMethods = Target.Methods.Where(w => w.TestMethod == true).ToList();
            var pattern = @"\[([aA-zZ]+)(.*)\]\s+(.*)";
            var features = new List<ITestFeature>();

            foreach (var method in testMethods)
            {
                var matches = Regex.Matches(method.Content, pattern);
                if (matches.Count > 0)
                {
                    var matchList = matches.ToList();
                    var scenarioElems = matchList
                        .Select(m => new { 
                            Type = m.Groups[1].Value.ToEnum<ScenarioElementType>(), 
                            Id = m.Groups[2].Value.Trim().Replace("#", ""),
                            Name = m.Groups[3].Value.Trim() 
                        });

                    ITestFeature feature = new TestFeature();
                    var scenario = new TestScenario();

                    foreach (var elem in scenarioElems)
                    {
                        switch (elem.Type)
                        {
                            case ScenarioElementType.FEATURE:
                                feature = features.FirstOrDefault(a => a.Name == elem.Name);
                                if (feature == null)
                                {
                                    feature = new TestFeature {
                                        Name = elem.Name,
                                        Scenarios = new List<ITestScenario>()
                                    };
                                    features.Add(feature);
                                }
                                break;
                            case ScenarioElementType.SCENARIO:
                                scenario = new TestScenario();
                                scenario.ID = int.Parse(elem.Id);
                                scenario.Name = elem.Name;
                                scenario.Feature = feature;
                                feature.Scenarios.Add(scenario);
                                break;
                            case ScenarioElementType.GIVEN:
                                var given = new TestScenarioElement();
                                given.Type = elem.Type;
                                given.Value = elem.Name;
                                scenario.Elements.Add(given);
                                break;
                            case ScenarioElementType.WHEN:
                                var when = new TestScenarioElement();
                                when.Type = elem.Type;
                                when.Value = elem.Name;
                                scenario.Elements.Add(when);
                                break;
                            case ScenarioElementType.THEN:
                                var then = new TestScenarioElement();
                                then.Type = elem.Type;
                                then.Value = elem.Name;
                                scenario.Elements.Add(then);
                                break;
                            default:
                                break;
                        }
                    }

                    method.Scenario = scenario;
                }
            }

            Target.Features = features;
        }

        #endregion

        #region Write to Object

        public override void OnWriteObjectHeader(IndentedTextWriter writer, IALObject Target, List<ITestFeature> Features = null)
        {
            base.OnWriteObjectHeader(writer, Target, Features);
            writer.Indent++;
            writer.WriteLine("SubType = Test;");
            writer.WriteLine();
            writer.Indent--;
        }

        public override void OnWriteObjectMethods(IndentedTextWriter writer, IALObject Target, List<ITestFeature> Features = null)
        {
            if (Features != null && Features.Count() > 0)
            {
                if (Target.Methods.Count > 0)
                {
                    //TODO: implement merging scenarios
                }
                else
                {
                    Target.Methods = Features
                        .SelectMany(s => s.Scenarios)
                        .Select(s => new ALMethod()
                        {
                            Name = s.Name.SanitizeName(),
                            TestMethod = true,
                            Scenario = s,
                            MethodKind = "procedure",
                            Content = ""
                        })
                        .ToList();
                }
            }

            base.OnWriteObjectMethods(writer, Target, Features);
        }

        public override string OnWriteObjectMethod(IALObject Target, ALMethod method)
        {
            var result = "";
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new IndentedTextWriter(stringWriter))
                {
                    writer.Indent++;
                    writer.WriteLine();

                    WriteObjectMethodHeader(method, writer);
                    WriteObjectMethodBody(Target, method, writer);
                    WriteObjectMethodFooter(method, writer);

                    writer.Indent--;

                    result = stringWriter.ToString().Replace("}", "").Trim();
                }
            }

            return result;
        }

        public void WriteObjectMethodHeader(ALMethod method, IndentedTextWriter writer)
        {
            bool HasScenario = method.Scenario != null;
            if (HasScenario)
            {
                writer.WriteLine($"//#region [SCENARIO #{method.Scenario.ID:0000}] {method.Scenario.Name}");
                writer.WriteLine();
            }

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
        }

        public void WriteObjectMethodBody(IALObject Target, ALMethod method, IndentedTextWriter writer)
        {
            bool HasScenario = method.Scenario != null;
            bool NoContent = String.IsNullOrEmpty(method.Content);

            if (NoContent)
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
                                writer.WriteLine(e.WriteMethod(Config) + "();");
                                writer.WriteLine();
                            });

                        writer.WriteLine();
                    }

                    writer.Indent--;
                }

                writer.WriteLine("end;");

                method.Scenario.Elements.ToList().ForEach(e =>
                {
                    if (!Target.Methods.Any(a => a.Name == e.WriteMethod(Config)))
                    {
                        writer.WriteLine();
                        writer.WriteLine($"local procedure {e.WriteMethod(Config)}()");
                        writer.WriteLine("begin");
                        writer.WriteLine("end;");
                    }
                });
            }
            else
            {
                writer.WriteLine(method.Content);
            }
        }

        public void WriteObjectMethodFooter(ALMethod method, IndentedTextWriter writer)
        {
            bool HasScenario = method.Scenario != null;
            if (HasScenario)
            {
                writer.WriteLine();
                writer.WriteLine("//#endregion");
            }
        }

        #endregion
    }
}
