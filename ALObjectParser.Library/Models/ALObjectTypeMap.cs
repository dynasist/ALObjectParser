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
            Add(ALObjectType.tableextension, typeof(ALTableExtension));
            Add(ALObjectType.page, typeof(ALPage));
            Add(ALObjectType.pagecustomization, typeof(ALPageCustomization));
            Add(ALObjectType.pageextension, typeof(ALPageExtension));
            Add(ALObjectType.report, typeof(ALTable));
            Add(ALObjectType.xmlport, typeof(ALTable));
            Add(ALObjectType.query, typeof(ALTable));
            Add(ALObjectType.codeunit, typeof(ALCodeunit));
            Add(ALObjectType.controladdin, typeof(ALTable));
            Add(ALObjectType.dotnet, typeof(ALTable));
            Add(ALObjectType.@enum, typeof(ALTable));
            Add(ALObjectType.enumextension, typeof(ALTable));
            Add(ALObjectType.profile, typeof(ALTable));
            Add(ALObjectType.@interface, typeof(ALTable));
        }

        public static IALObject CreateInstance(ALObjectType type)
        {
            var items = new ALObjectTypeMap();
            dynamic instance = Activator.CreateInstance(items[type]);

            return instance;
        }
    }
}
