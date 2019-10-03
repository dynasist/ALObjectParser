using System;
using System.Collections.Generic;
using System.Text;

namespace ALObjectParser.Library
{
    public interface ITestScenarioElement
    {
        ScenarioElementType Type { get; set; }
        string Value { get; set; }
    }
}
