using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ALObjectParser.Library
{
    public class ALTestCodeunitParser : ALObjectParser
    {
        public ALTestCodeunitParser(): base()
        {
            ALObject = new ALCodeunit();
        }

        public ALTestCodeunitParser(string FilePath) : base(FilePath)
        {
            ALObject = new ALCodeunit();
            Path = FilePath;
        }

        public override void OnWriteObjectHeader(IndentedTextWriter writer, IALObject Target, List<ITestFeature> Features = null)
        {
            base.OnWriteObjectHeader(writer, Target, Features);
            writer.Indent++;
            writer.WriteLine("SubType = Test;");
            writer.WriteLine();
            writer.Indent--;
        }

        public override string OnWriteObjectMethod(ALMethod method)
        {
            return base.OnWriteObjectMethod(method);
        }
    }
}
