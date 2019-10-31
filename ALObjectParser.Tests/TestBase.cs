using ALObjectParser.Library;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALObjectParser.Tests
{
    public class TestBase
    {
        protected string testPath;
        protected List<string> lines;

        [SetUp]
        public void Setup()
        {
            testPath = @".\test_cu.al";
        }
    }
}
