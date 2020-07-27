using System;
using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALMethod
    {
        public ALMethod()
        {
            Parameters = new List<ALParameter>();
            Attributes = new List<ALAttribute>();
            Comments = new List<ALComment>();
        }

        public string Name { get; set; }
        public ALMethodKind MethodKind { get; set; }
        public bool TestMethod { get; set; }
        public bool IsMethodDeclaration { get; set; }
        public bool IsLocal { get; set; }
        public ICollection<ALParameter> Parameters { get; set; }
        public ALReturnTypeDefinition ReturnTypeDefinition { get; set; }
        public string Content { get; set; }
        public ALMethodBody MethodBody { get; set; }
        public ICollection<ALAttribute> Attributes { get; set; }
        public IEnumerable<ALComment> Comments { get; set; }
        public Range MethodRange { get; set; }
    }
}