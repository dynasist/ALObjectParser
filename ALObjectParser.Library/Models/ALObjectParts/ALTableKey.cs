using System;
using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALTableKey
    {
        public string Name  {get; set;}
        public IEnumerable<string> FieldNames {get; set;}
        public IEnumerable<ALProperty> Properties {get; set;}
    }
}