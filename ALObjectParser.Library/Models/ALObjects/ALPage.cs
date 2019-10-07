using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALPage : ALObject
    {
        public ALPage()
        {
            Type = ALObjectType.page;
        }

        public IEnumerable<ALPageControl> Controls { get; set; }

        public IEnumerable<ALPageAction> Actions { get; set; }
    }
}