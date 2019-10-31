using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ALObjectParser.Library
{
    /// <summary>
    /// Read/Write AL Language formatted files
    /// Base implementation that provides basic information processing
    /// </summary>
    public static class ALParser
    {
        public static IEnumerable<IALObject> Read(string Path)
        {
            return ALParser.Read<ALObjectReaderBase>(Path);
        }

        public static IEnumerable<IALObject> ReadObjectInfos(string Path)
        {
            return ALParser.ReadObjectInfos<ALObjectReaderBase>(Path);
        }

        public static IALObject ReadSingle(string Path)
        {
            return ALParser.ReadSingle<ALObjectReaderBase>(Path);
        }

        public static IEnumerable<IALObject> Read<T>(string Path)
            where T: ALObjectReaderBase, new()
        {
            var reader = new T();
            var result = reader.Read(Path);

            return result;
        }

        public static IEnumerable<IALObject> ReadObjectInfos<T>(string Path)
            where T : ALObjectReaderBase, new()
        {
            var reader = new T();
            var result = reader.ReadObjectInfos(Path);

            return result;
        }

        public static IALObject ReadSingle<T>(string Path)
            where T : ALObjectReaderBase, new()
        {
            var reader = new T();
            var result = reader.ReadSingle(Path);

            return result;
        }

        public static void Write(IALObject Target, string Path)
        {
            ALParser.Write<ALObjectWriterBase>(Target, Path);
        }

        public static void Write(IEnumerable<IALObject> Target, string Path)
        {
            ALParser.Write<ALObjectWriterBase>(Target, Path);
        }

        public static string Write(IALObject Target)
        {
            return ALParser.Write<ALObjectWriterBase>(Target);
        }

        public static string Write(IEnumerable<IALObject> Target)
        {
            return ALParser.Write<ALObjectWriterBase>(Target);
        }

        public static void Write<T>(IALObject Target, string Path)
            where T : ALObjectWriterBase, new()
        {
            var writer = new T();
            writer.Write(Target, Path);
        }

        public static void Write<T>(IEnumerable<IALObject> Target, string Path)
            where T : ALObjectWriterBase, new()
        {
            var writer = new T();
            writer.Write(Target, Path);
        }

        public static string Write<T>(IALObject Target)
            where T : ALObjectWriterBase, new()
        {
            var writer = new T();
            return writer.Write(Target);
        }

        public static string Write<T>(IEnumerable<IALObject> Target)
            where T : ALObjectWriterBase, new()
        {
            var writer = new T();
            return writer.Write(Target);
        }
    }
}
