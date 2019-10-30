using System;
using System.Collections.Generic;
using System.Text;

namespace ALObjectParser.Library
{
    public class ALObjectTypeMap: Dictionary<ALObjectType, Type>
    {
        public ALObjectTypeMap()
        {
            Add(ALObjectType.table, typeof(ALTable));
            Add(ALObjectType.tableextension, typeof(ALTable));
            Add(ALObjectType.page, typeof(ALTable));
            Add(ALObjectType.pagecustomization, typeof(ALTable));
            Add(ALObjectType.pageextension, typeof(ALTable));
            Add(ALObjectType.report, typeof(ALTable));
            Add(ALObjectType.xmlport, typeof(ALTable));
            Add(ALObjectType.query, typeof(ALTable));
            Add(ALObjectType.codeunit, typeof(ALTable));
            Add(ALObjectType.controladdin, typeof(ALTable));
            Add(ALObjectType.dotnet, typeof(ALTable));
            Add(ALObjectType.@enum, typeof(ALTable));
            Add(ALObjectType.profile, typeof(ALTable));
        }

        public static IALObject CreateInstance(ALObjectType type)
        {
            var items = new ALObjectTypeMap();
            dynamic instance = Activator.CreateInstance(items[type]);

            return instance;
        }
    }
}
