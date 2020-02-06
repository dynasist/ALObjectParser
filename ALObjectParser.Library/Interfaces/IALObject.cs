using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public interface IALObject: IALSection
    {
        int Id { get; set; }
        ALObjectType Type { get; set; }

        public ICollection<ALVariable> GlobalVariables { get; set; }

        virtual void ProcessSections() { }

        void Write(IndentedTextWriter writer) { }

    }
}