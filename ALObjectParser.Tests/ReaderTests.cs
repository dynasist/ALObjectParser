using ALObjectParser.Library;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ALObjectParser.Tests
{
    public class ReaderTests: TestBase
    {
        [Test]
        public void TestCodeunit_ID_Name()
        {
            var result = parser.Read(lines);

            Assert.AreEqual(81000, result.Id);
            Assert.AreEqual(@"LookupValue UT Customer", result.Name);
        }

        [Test]
        public void TestCodeunit_Verify_Procedures()
        {
            var result = parser.Read(lines);

            Assert.IsTrue(result.Methods.Count > 0);
        }

        [Test]
        public void TestCodeunit_Procedure_Verify_Parameters()
        {
            var result = parser.Read(lines);

            Assert.IsTrue(result.Methods.First().Parameters.Count > 0);
        }

        [Test]
        [TestCase(0, false)] // Has Return Type
        [TestCase(1, true)] // Does not have Return Type
        public void TestCodeunit_Procedure_Verify_ReturnType(int index, bool expected)
        {
            var result = parser.Read(lines);
            var actual = string.IsNullOrEmpty(result.Methods[index].ReturnType.Trim()) == expected;

            Assert.IsTrue(actual);
        }

        [Test]
        public void TestCodeunit_Procedure_Verify_TestAttribute()
        {
            var result = parser.Read(lines);

            Assert.IsTrue(result.Methods.First().TestMethod);
        }

        [Test]
        [TestCase(0, false)]
        [TestCase(12, true)]
        public void TestCodeunit_Procedure_Verify_IsLocal(int index, bool expected)
        {
            var result = parser.Read(lines);
            var actual = result.Methods[index].IsLocal == expected;

            Assert.IsTrue(actual);
        }

        [Test]
        public void TestCodeunit_Verify_TestFeaturesAndScenarios()
        {
            var result = parser.Read(lines);

            // There should be 1 Feature + 3 Scenarios
            Assert.IsTrue(result.Features.Count == 1);
            Assert.IsTrue(result.Features.SelectMany(s => s.Scenarios).Count() == 3);
        }

    }
}