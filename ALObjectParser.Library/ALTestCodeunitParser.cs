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
    }
}
