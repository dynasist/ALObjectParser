using System;
using System.Collections.Generic;
using System.Text;

namespace ALObjectParser.Library
{
    public class SymbolReference
    {
        public Guid AppId { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public Version Version { get; set; }
        public IEnumerable<ALTable> Tables { get; set; }
        public IEnumerable<ALCodeunit> Codeunits { get; set; }
        public IEnumerable<ALPage> Pages { get; set; }
        public IEnumerable<ALPageExtension> PageExtensions { get; set; }
        public IEnumerable<ALPageCustomization> PageCustomizations { get; set; }
        public IEnumerable<ALTableExtension> TableExtensions { get; set; }
        public IEnumerable<dynamic> Reports { get; set; }
        public IEnumerable<dynamic> XmlPorts { get; set; }
        public IEnumerable<dynamic> Queries { get; set; }
        public IEnumerable<dynamic> Profiles { get; set; }
        public IEnumerable<dynamic> ControlAddIns { get; set; }
        public IEnumerable<dynamic> EnumTypes { get; set; }
        public IEnumerable<dynamic> DotNetPackages { get; set; }
    }
}
