using System;
using System.Collections.Generic;
using System.Text;

namespace ALObjectParser.Library
{
    public class ALPageControlBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Kind  { get; set; }
        public ALTypeDefinition TypeDefinition { get; set; }
        public IEnumerable<ALProperty> Properties { get; set; }
        public string ControlType { get; set; }
        public string SourceExpression { get; set; }
        public string Caption { get; set; }
        public string SourceCodeAnchor { get; set; }
        public ALPageControlBase Parent { get; set; }
        public string GroupName { get; set; }
        public string FsPath { get; set; }
        public ALPage Symbol { get; set; }
    }
}
