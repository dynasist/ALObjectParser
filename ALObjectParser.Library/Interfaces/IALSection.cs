using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public interface IALSection
    {
        string Name { get; set; }
        ICollection<IALSection> Sections { get; set; }
        ICollection<ALMethod> Methods { get; set; }
        ICollection<ALProperty> Properties { get; set; }
        ICollection<ALComment> Comments { get; set; }
    }
}