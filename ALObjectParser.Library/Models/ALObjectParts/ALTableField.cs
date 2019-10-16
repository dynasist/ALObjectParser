using System;
using System.Collections.Generic;
using System.Text;

namespace ALObjectParser.Library
{
    public class ALTableField
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ALTypeDefinition TypeDefinition { get; set; } 

        public IEnumerable<ALProperty> Properties {get; set;}
    }
}
