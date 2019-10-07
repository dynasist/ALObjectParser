using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALPageExtension : ALTable
    {
        public ALPageExtension(): base()
        {
            Type = ALObjectType.pageextension;
        }

        public IEnumerable<dynamic> ControlChanges { get; set; }

        public string TargetObject { get; set; }

        public ALPage TargetPage { get; set; }
    }
}