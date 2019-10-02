using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public interface IALObject
    {
        int Id { get; set; }
        string Type { get; set; }
        string Name { get; set; }
        List<ALMethod> Methods { get; set; }
        List<ITestFeature> Features { get; set; }
    }
}