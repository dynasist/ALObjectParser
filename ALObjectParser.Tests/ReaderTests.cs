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
            Assert.AreEqual(@"""LookupValue UT Customer""", result.Name);
        }

        [Test]
        public void TestCodeunit_Has_Procedures()
        {
            var result = parser.Read(lines);

            Assert.IsTrue(result.Methods.Count > 0);
        }

        [Test]
        public void TestCodeunit_Procedure_Has_Parameters()
        {
            var result = parser.Read(lines);

            Assert.IsTrue(result.Methods.First().Parameters.Count > 0);
        }

        [Test]
        public void TestCodeunit_Procedure_Has_ReturnType()
        {
            var result = parser.Read(lines);

            Assert.IsTrue(!string.IsNullOrEmpty(result.Methods.First().ReturnType.Trim()));
        }

        [Test]
        public void TestCodeunit_Procedure_Has_NO_ReturnType()
        {
            var result = parser.Read(lines);

            Assert.IsTrue(string.IsNullOrEmpty(result.Methods.ElementAt(1).ReturnType.Trim()));
        }
    }
}