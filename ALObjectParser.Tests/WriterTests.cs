using NUnit.Framework;

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
    }
}