using System;
using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class TestFeature : ITestFeature
    {
        public TestFeature()
        {
            Scenarios = new List<ITestScenario>();
        }

        public string Name { get; set; }

        public ICollection<ITestScenario> Scenarios { get; set; }
    }
}
