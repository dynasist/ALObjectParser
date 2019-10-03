using System;
using System.Collections.Generic;
using System.Text;

namespace ALObjectParser.Library
{
    public interface ITestFeature
    {
        string Name { get; set; }

        ICollection<ITestScenario> Scenarios { get; set; }
    }
}
