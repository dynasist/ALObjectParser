using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALPageAction: ALPageControlBase
    {
        public IEnumerable<ALPageAction> Actions { get; set; }
        public new ALPageAction Parent { get; set; }
        public new ALActionKind Kind { get; set; }
    }
}