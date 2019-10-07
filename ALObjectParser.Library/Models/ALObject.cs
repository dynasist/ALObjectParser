using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALObject: IALObject
    {
        public ALObject()
        {
            Methods = new List<ALMethod>();
        }

        public int Id { get; set; }
        public ALObjectType Type { get; set; }
        public string Name { get; set; }

        public List<ALMethod> Methods { get; set; }
        public List<ITestFeature> Features { get; set; }

    }
}