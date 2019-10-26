using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALMethodBody
    {
        public ALMethodBody()
        {
            Comments = new List<ALComment>();
        }

        public IEnumerable<string> ContentLines { get; set; }
        public string Content { get; set; }
        public IEnumerable<ALComment> Comments { get; set; }
    }
}