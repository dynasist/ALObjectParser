using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALPageControl: ALPageControlBase
    {
        public IEnumerable<ALPageControl> Controls { get; set; }
        public new ALPageControl Parent { get; set; }
        public new ALControlKind Kind { get; set; }
    }
}