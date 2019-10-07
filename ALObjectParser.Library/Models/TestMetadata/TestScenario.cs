using System;
using System.Collections.Generic;
using System.Text;

namespace ALObjectParser.Library
{
    /// <summary>
    /// Test Scenario object
    /// </summary>
    public class TestScenario : ITestScenario
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public TestScenario()
        {
            Elements = new List<ITestScenarioElement>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public ITestFeature Feature { get; set; }
        public ICollection<ITestScenarioElement> Elements { get; set; }
    }
}
