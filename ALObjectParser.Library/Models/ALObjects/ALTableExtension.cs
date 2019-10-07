using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALTableExtension : ALTable
    {
        public ALTableExtension(): base()
        {
            Type = ALObjectType.tableextension;
        }

        public string TargetObject { get; set; }

        public ALTable TargetTable { get; set; }
    }
}