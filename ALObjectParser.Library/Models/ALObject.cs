using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALObject: IALObject
    {
        public ALObject()
        {
            GlobalVariables = new List<ALVariable>();
            Methods = new List<ALMethod>();
            Properties = new List<ALProperty>();
            Comments = new List<ALComment>();
        }

        public int Id { get; set; }
        public ALObjectType Type { get; set; }
        public string Name { get; set; }
        public string TextContent { get; set; }
        public string SymbolZipName { get; set; }

        public ICollection<ALVariable> GlobalVariables { get; set; }
        public ICollection<IALSection> Sections { get; set; }
        public ICollection<ALMethod> Methods { get; set; }
        public ICollection<ALProperty> Properties { get; set; }
        public ICollection<ALComment> Comments { get; set; }

        public virtual void ProcessSections() { 

        }

        public virtual void Write(IndentedTextWriter writer)
        {
            writer.Indent++;
            foreach (var s in Properties)
            {
                var value = s.Value.Contains(" ") ? $"\"{s.Value}\"" : s.Value;
                writer.WriteLine($"{s.Name} = {value};");
            }
            writer.Indent--;
        }
    }
}