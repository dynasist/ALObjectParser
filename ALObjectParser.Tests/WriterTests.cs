using ALObjectParser.Library;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ALObjectParser.Tests
{

    public class WriterTests : TestBase
    {
        [Test]
        public void WriteBackExistingObject_NoChange()
        {
            var alobjects = ALParser.Read(testPath);
            foreach (var obj in alobjects)
            {
                var result = ALParser.Write(obj);
                Assert.IsNotEmpty(result);
            }

            var allobjStr = ALParser.Write(alobjects);
            Assert.IsNotEmpty(allobjStr);
        }
    

        [Test]
        public void WriteBackExistingObject_UpdatedParameter()
        {
            var alobjects = ALParser.Read(testPath);
            var alobject = alobjects.ElementAt(1);
            alobject.Methods.ElementAt(0).Parameters.ElementAt(0).Name = "UpdatedParameter_NewNameGiven";

            var result = ALParser.Write(alobjects);

            Assert.IsNotEmpty(result);
            Assert.IsTrue(result.Contains("UpdatedParameter_NewNameGiven"));
        }

        [Test]
        public void WriteNewObject_FromALObjectClass()
        {
            var alobject = new ALCodeunit
            {
                Id = 81000,
                Name = "Test Codeunit",
                Methods = new List<ALMethod>()
            };

            var method = new ALMethod { TestMethod = true, Name = "TestMethod", MethodKind = ALMethodKind.Method };

            alobject.Methods.Add(method);

            var result = ALParser.Write(alobject);

            Assert.IsNotEmpty(result);
        }

        /*[Test]
        public void WriteNewObject_FromFeatures()
        {
            var alobject = new ALCodeunit
            {
                Id = 81000,
                Name = "Test Codeunit",
                Features = new List<ITestFeature>(),
                Methods = new List<ALMethod>()
            };

            var features = GetFeatures();

            var result = parser.Write(alobject, features);

            Assert.IsNotEmpty(result);
        }

        [Test]
        [TestCase(true, false, false, Description = "Verify Identical - Features should be Identical")]
        [TestCase(false, false, false, Description = "Verify Difference - There should be an additional Feature")]
        [TestCase(false, true, false, Description = "Verify Difference - There should be same Features with an additional Scenario")]
        [TestCase(false, false, true, Description = "Verify Difference - There should be same Features/Scenarios with an additional Given")]
        public void WriteNewObject_MergeFeatures_Verify_FeatureComparison(bool ShoulBeIdentical, bool addScenarioToDiffer, bool addGivenToDiffer)
        {
            // Create a dummy AL Object
            var alobject = new ALCodeunit
            {
                Id = 81000,
                Name = "Test Codeunit",
                Features = GetFeatures(),
                Methods = new List<ALMethod>()
            };

            // Create updated Feature set
            var features = GetFeatures();
            if (!ShoulBeIdentical)
            {
                if (addScenarioToDiffer)
                {
                    features
                        .FirstOrDefault()
                        .Scenarios
                        .Add(new TestScenario { 
                            ID = 99, 
                            Name = "Additional Scenario",
                            Feature = features.FirstOrDefault(),
                            Elements = new List<ITestScenarioElement>
                            {
                                new TestScenarioElement { Type = ScenarioElementType.GIVEN, Value = "Given Additional 1" },
                                new TestScenarioElement { Type = ScenarioElementType.THEN, Value = "Then Additional 2" },
                                new TestScenarioElement { Type = ScenarioElementType.WHEN, Value = "When Additional 3" }
                            }
                        });
                }
                else
                {
                    if (addGivenToDiffer)
                    {
                        features
                            .FirstOrDefault()
                            .Scenarios
                            .FirstOrDefault()
                            .Elements.
                            Add(new TestScenarioElement { 
                                Type = ScenarioElementType.GIVEN,
                                Value = "Given something new"
                            });
                    }
                    else
                    {
                        var newFeature = new TestFeature { Name = "Second Feature" };
                        var newScenario = new TestScenario
                        {
                            ID = 99,
                            Name = "Additional Scenario",
                            Feature = newFeature,
                            Elements = new List<ITestScenarioElement>
                            {
                                new TestScenarioElement { Type = ScenarioElementType.GIVEN, Value = "Given 1" },
                                new TestScenarioElement { Type = ScenarioElementType.THEN, Value = "Then 2" },
                                new TestScenarioElement { Type = ScenarioElementType.WHEN, Value = "When 3" }
                            }
                        };

                        var newScenarios = new List<ITestScenario>();
                        newScenarios.Add(newScenario);
                        newFeature.Scenarios = newScenarios;

                        features.Add(newFeature);
                    }
                }
            }

            var scenario = alobject.Features.ElementAt(0).Scenarios.ElementAt(0);
            alobject.Methods.Add(new ALMethod { TestMethod = true, Name = "TestScenario", MethodKind = "procedure", Scenario = scenario });
            alobject.Methods.Add(new ALMethod { TestMethod = false, Name = "Custom Method", MethodKind = "procedure", Content = "begin xx end;" });

            // Write and read back object contents
            var result = parser.Write(alobject, features);
            var lines = result.Split("\r\n").ToList();
            var testCu = parser.Read(lines);
            var actualFeatures = testCu.Features;

            // Expected Feature set
            var exptectedFeatures = GetFeatures();

            // compare parsed features with expected set
            var res = exptectedFeatures.SequenceEqual(actualFeatures, new TestFeatureComparer());

            if (ShoulBeIdentical)
            {
                Assert.IsTrue(res);
            }
            else
            {
                Assert.IsFalse(res);
            }
        }

        private List<ITestFeature> GetFeatures()
        {
            var feature = new TestFeature()
            {
                Name = "Test Feature",
                Scenarios = new List<ITestScenario>()
            };

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

            var features = new List<ITestFeature>();
            features.Add(feature);

            return features;
        }*/
    }
    
}