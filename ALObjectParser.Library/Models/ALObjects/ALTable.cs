using System.Collections.Generic;

namespace ALObjectParser.Library
{
    public class ALTable : ALObject
    {
        public ALTable()
        {
            Type = ALObjectType.table;
        }

        public IEnumerable<ALTableField> Fields { get; set; }

        public IEnumerable<ALTableKey> Keys { get; set; }
        public IEnumerable<ALTableKey> FieldGroups { get; set; }
    }
}