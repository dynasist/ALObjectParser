using System;
using System.Collections.Generic;
using System.Text;

namespace ALObjectParser.Library
{
    public interface ITestScenario
    {
        int ID { get; set; }
        string Name { get; set; }
        ITestFeature Feature { get; set; }
        ICollection<ITestScenarioElement> Elements { get; set; }
    }
}
