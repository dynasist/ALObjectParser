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
            var result = ALParser.Read(testPath);

            Assert.AreEqual(result.Count(), 2);
            Assert.AreEqual(81000, result.ElementAt(1).Id);
            Assert.AreEqual(@"LookupValue UT Customer", result.ElementAt(1).Name);
        }

        [Test]
        public void TestCodeunit_Verify_Procedures()
        {
            var result = ALParser.ReadSingle(testPath);

            Assert.IsTrue(result.Methods.Count > 0);
        }

        [Test]
        public void TestCodeunit_Procedure_Verify_Parameters()
        {
            var result = ALParser.ReadSingle(testPath);

            Assert.IsTrue(result.Methods.First().Parameters.Count > 0);
        }

        [Test]
        [TestCase(0, false)] // Has Return Type
        [TestCase(1, true)] // Does not have Return Type
        public void TestCodeunit_Procedure_Verify_ReturnType(int index, bool expected)
        {
            var result = ALParser.ReadSingle(testPath);
            var actual = string.IsNullOrEmpty(result.Methods.ElementAt(index).ReturnType.Trim()) == expected;

            Assert.IsTrue(actual);
        }

        [Test]
        public void TestCodeunit_Procedure_Verify_TestAttribute()
        {
            var result = ALParser.ReadSingle(testPath);
            Assert.IsTrue(result.Methods.First().TestMethod);
        }

        [Test]
        [TestCase(0, false)]
        [TestCase(12, true)]
        public void TestCodeunit_Procedure_Verify_IsLocal(int index, bool expected)
        {
            var result = ALParser.ReadSingle(testPath);
            var actual = result.Methods.ElementAt(index).IsLocal == expected;

            Assert.IsTrue(actual);
        }

    }
}