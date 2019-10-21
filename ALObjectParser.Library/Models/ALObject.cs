using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALObject: IALObject
    {
        public ALObject()
        {
            Methods = new List<ALMethod>();
            Properties = new List<ALProperty>();
        }

        public int Id { get; set; }
        public ALObjectType Type { get; set; }
        public string Name { get; set; }

        public ICollection<ALMethod> Methods { get; set; }
        public ICollection<ITestFeature> Features { get; set; }
        public ICollection<ALProperty> Properties { get; set; }
    }
}