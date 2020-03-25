using ALObjectParser.Library;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ALObjectParser.Tests
{
    public class ReaderTests: TestBase
    {
        [Test]
        public void Read_ID_Name()
        {
            var result = ALParser.Read(testPath);

            Assert.AreEqual(result.Count(), 2);
            Assert.AreEqual(81000, result.ElementAt(1).Id);
            Assert.AreEqual(@"LookupValue UT Customer", result.ElementAt(1).Name);
        }

        [Test]
        public void Read_ObjectHeaders()
        {
            var result = ALParser.ReadObjectInfos(testPath);

            Assert.AreEqual(result.Count(), 2);
            Assert.AreEqual(81000, result.ElementAt(1).Id);
            Assert.AreEqual(@"LookupValue UT Customer", result.ElementAt(1).Name);
        }

        [Test]
        public void Read_Verify_Procedures()
        {
            var result = ALParser.ReadSingle(testPath);

            Assert.IsTrue(result.Methods.Count > 0);
        }

        [Test]
        public void Read_Procedure_Verify_Parameters()
        {
            var result = ALParser.ReadSingle(testPath);

            Assert.IsTrue(result.Methods.First().Parameters.Count > 0);
        }

        [Test]
        [TestCase(0, false)] // Has Return Type
        [TestCase(1, true)] // Does not have Return Type
        public void Read_Procedure_Verify_ReturnType(int index, bool expected)
        {
            var result = ALParser.ReadSingle(testPath);
            var actual = string.IsNullOrEmpty(result.Methods.ElementAt(index).ReturnTypeDefinition.Name.Trim()) == expected;

            Assert.IsTrue(actual);
        }

        [Test]
        public void Read_Procedure_Verify_TestAttribute()
        {
            var result = ALParser.ReadSingle(testPath);
            Assert.IsTrue(result.Methods.First().TestMethod);
        }

        [Test]
        [TestCase(0, false)]
        [TestCase(12, true)]
        public void Read_Procedure_Verify_IsLocal(int index, bool expected)
        {
            var result = ALParser.ReadSingle(testPath);
            var actual = result.Methods.ElementAt(index).IsLocal == expected;

            Assert.IsTrue(actual);
        }

        [Test]
        public void Read_Comments_Object()
        {
            var result = ALParser.Read(testPath);
            var actual = result.ElementAt(1).Comments;

            Assert.IsTrue(actual.Count() > 0);
        }

        [Test]
        public void Read_Comments_Methods()
        {
            var result = ALParser.Read(testPath);
            var actual = result.ElementAt(1).Methods.FirstOrDefault(f => f.Name == "CheckThatLabelCanBeAssignedToCustomer");

            Assert.IsTrue(actual.MethodBody.Comments.Count() == 6);
        }

        [Test]
        public void Read_Sections()
        {
            var result = ALParser.Read(testPath);
            var actual = result.ElementAt(0).Sections;

            Assert.IsTrue(actual.Count() == 1);
            Assert.IsTrue(actual.ElementAt(0).Sections.Count() == 2);
        }

        [Test]
        public void Read_GlobalVariables()
        {
            var result = ALParser.Read(testPath);

            Assert.IsTrue(result.ElementAt(0).GlobalVariables.Count() == 0);
            Assert.IsTrue(result.ElementAt(1).GlobalVariables.Count() == 1);
        }

    }
}