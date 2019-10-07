using System;
using System.Collections.Generic;
using System.Text;

namespace ALObjectParser.Library
{
    public class TestScenarioElement: ITestScenarioElement
    {
        public ScenarioElementType Type { get; set; }
        public string Value { get; set; }
    }
}
