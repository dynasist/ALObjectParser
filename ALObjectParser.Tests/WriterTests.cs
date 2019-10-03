using ALObjectParser.Library;
using NUnit.Framework;
using System.Collections.Generic;

namespace ALObjectParser.Tests
{
    public class WriterTests : TestBase
    {
        [Test]
        public void WriteBackExistingObject_NoChange()
        {
            var alobject = parser.Read(lines);
            var result = parser.Write(alobject);

            Assert.IsNotEmpty(result);
        }

        [Test]
        public void WriteBackExistingObject_UpdatedParameter()
        {
            var alobject = parser.Read(lines);
            alobject.Methods[0].Parameters[0].Name = "UpdatedParameter_NewNameGiven";

            var result = parser.Write(alobject);

            Assert.IsNotEmpty(result);
            Assert.IsTrue(result.Contains("UpdatedParameter_NewNameGiven"));
        }

        [Test]
        public void WriteBackExistingObject_AddedTestInfo()
        {
            var alobject = new ALCodeunit
            {
                Id = 81000,
                Name = "Test Codeunit",
                Features = new List<ITestFeature>(),
                Methods = new List<ALMethod>()
            };

            var feature = new TestFeature() { Name = "Test Feature", Scenarios = new List<ITestScenario>() };
            var scenario = new TestScenario()
            {
                ID = 1,
                Name = "Test Scenario",
                Feature = feature
            };

            scenario.Elements = new List<ITestScenarioElement>();
            scenario.Elements.Add(new TestScenarioElement { Type = ScenarioElementType.GIVEN, Value = "customer is set" });
            scenario.Elements.Add(new TestScenarioElement { Type = ScenarioElementType.GIVEN, Value = "Vendor Is Set" });
            scenario.Elements.Add(new TestScenarioElement { Type = ScenarioElementType.WHEN, Value = "When This is that" });
            scenario.Elements.Add(new TestScenarioElement { Type = ScenarioElementType.THEN, Value = "This Happens" });

            feature.Scenarios.Add(scenario);

            var method = new ALMethod { TestMethod = true, Name = "Test Method", MethodKind = "procedure", Scenario = scenario };

            alobject.Methods.Add(method);

            var result = parser.Write(alobject);

            Assert.IsNotEmpty(result);
        }
    }
}