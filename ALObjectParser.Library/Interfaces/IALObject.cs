using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public interface IALObject: IALSection
    {
        int Id { get; set; }
        ALObjectType Type { get; set; }        
    }
}