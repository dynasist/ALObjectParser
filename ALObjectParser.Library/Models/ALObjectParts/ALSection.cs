using System;
using System.Collections.Generic;
using System.Text;

namespace ALObjectParser.Library
{
    public class ALSection: IALSection
    {
        public ALSection()
        {
            Sections = new List<IALSection>();
            Methods = new List<ALMethod>();
            Properties = new List<ALProperty>();
            Comments = new List<ALComment>();
        }

        public string Name { get; set; }
        public string TextContent { get; set; }

        public ICollection<IALSection> Sections { get; set; }
        public ICollection<ALMethod> Methods { get; set; }
        public ICollection<ALProperty> Properties { get; set; }
        public ICollection<ALComment> Comments { get; set; }
    }
}
