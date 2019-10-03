using System;
using System.Collections.Generic;
using System.Text;

namespace ALObjectParser.Library
{
    public class TestScenario : ITestScenario
    {
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
