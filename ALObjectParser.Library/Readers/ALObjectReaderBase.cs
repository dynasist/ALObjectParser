using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ALObjectParser.Library
{
    public class ALObjectReaderBase
    {
        public ALObjectReaderBase()
        { }

        #region Read Object from file

        public IEnumerable<IALObject> Read(string Path)
        {
            var Lines = File.ReadAllLines(Path);
            var splittedLines = SplitObjectLines(Lines);

            var result = new List<IALObject>();
            foreach (var objLines in splittedLines)
            {
                var obj = Read(objLines);
                result.Add(obj);
            }

            return result;
        }

        public IEnumerable<IALObject> ReadObjectInfos(string Path)
        {
            var Lines = File.ReadAllLines(Path);
            var headerLines = GetObjectHeaderLines(Lines);

            var result = new List<IALObject>();
            foreach (var header in headerLines)
            {
                IALObject Target;
                GetObjectInfo(header, out Target);
                result.Add(Target);
            }

            return result;
        }

        /// <summary>
        /// Read File specified in "Path" property
        /// </summary>
        /// <returns></returns>
        public IALObject ReadSingle(string Path)
        {
            var Lines = File.ReadAllLines(Path);
            return Read(Lines);
        }

        public T Read<T>(string Path) 
            where T: ALObject
        {
            var Lines = File.ReadAllLines(Path);
            IALObject result = Read(Lines);

            return (result as T);
        }

        /// <summary>
        /// Read File contents converted to String array
        /// </summary>
        /// <param name="Lines">Array of textlines</param>
        /// <returns></returns>
        public IALObject Read(IEnumerable<string> Lines)
        {
            IALObject Target;
            GetObjectInfo(Lines, out Target);
            GetMethods(Lines, Target);
            GetObjectProperties(Lines, Target);
            OnRead(Lines, Target);

            return Target;
        }

        /// <summary>
        /// Method to implement custom processing during parsing
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="Target"></param>
        public virtual void OnRead(IEnumerable<string> Lines, IALObject Target)
        { }

        public IEnumerable<IEnumerable<string>> SplitObjectLines(IEnumerable<string> Lines)
        {
            var contents = Lines.ToList();
            var headerLines = GetObjectHeaderLines(contents);
            var headers = headerLines                
                .Select(s => contents.IndexOf(s))
                .ToList();

            if (headers.Count() < 2)
            {
                return new List<IEnumerable<string>>() { Lines };
            }

            var result = new List<IEnumerable<string>>();
            var c = headers.Count();
            for (int i = 0; i < c; i++)    
            {
                var startIndex = headers[i];
                var endIndex = Lines.Count()-1;
                var j = i + 1;
                if (j < c)
                {
                    endIndex = headers[j];
                }

                result.Add(contents.GetRange(startIndex, endIndex-startIndex));
            }

            return result;
        }

        public IEnumerable<string> GetObjectHeaderLines(IEnumerable<string> Lines)
        {
            var pattern = @"([a-z]+)\s([0-9]+)\s(.*)";
            var headers = Lines
                .Where(w => Regex.IsMatch(w.ToLower(), pattern));               

            return headers;
        }

        /// <summary>
        /// Basic object information, such as Type, ID, Name
        /// </summary>
        /// <param name="Lines">Array of textlines</param>
        /// <param name="Target">Current ALObject instance</param>
        public void GetObjectInfo(IEnumerable<string> Lines, out IALObject Target)
        {
            Target = new ALObject();
            var pattern = @"([a-z]+)\s([0-9]+)\s(.*)";
            var line = Lines
                .Where(w => Regex.IsMatch(w.ToLower(), pattern))
                .FirstOrDefault();

            GetObjectInfo(line, out Target);
        }

        /// <summary>
        /// Basic object information, such as Type, ID, Name
        /// </summary>
        /// <param name="Lines">Array of textlines</param>
        /// <param name="Target">Current ALObject instance</param>
        public void GetObjectInfo(string line, out IALObject Target)
        {
            Target = new ALObject();
            var pattern = @"([a-z]+)\s([0-9]+)\s(.*)";

            if (!string.IsNullOrEmpty(line))
            {
                var items = Regex.Match(line, pattern);
                var type = items.Groups[1].Value.ToEnum<ALObjectType>();

                switch (type)
                {
                    case ALObjectType.table:
                        Target = new ALTable();
                        break;
                    case ALObjectType.tableextension:
                        Target = new ALTableExtension();
                        break;
                    case ALObjectType.page:
                        Target = new ALPage();
                        break;
                    case ALObjectType.pagecustomization:
                        Target = new ALPageCustomization();
                        break;
                    case ALObjectType.pageextension:
                        Target = new ALPageExtension();
                        break;
                    case ALObjectType.report:
                        break;
                    case ALObjectType.codeunit:
                        Target = new ALCodeunit();
                        break;
                    case ALObjectType.xmlport:
                        break;
                    case ALObjectType.query:
                        break;
                    case ALObjectType.controladdin:
                        break;
                    case ALObjectType.@enum:
                        break;
                    case ALObjectType.dotnet:
                        break;
                    case ALObjectType.profile:
                        break;
                    default:
                        break;
                }


                Target.Id = int.Parse(items.Groups[2].Value);
                Target.Name = items.Groups[3].Value.Replace("\"", "");
                Target.Type = type;
            }

            OnGetObjectInfo(line, Target);
        }

        /// <summary>
        /// Method to implement custom  for extended classes
        /// </summary>
        /// <param name="Line">Top line of object definition</param>
        /// <param name="Target">Current ALObject instance</param>
        public virtual void OnGetObjectInfo(string Line, IALObject Target)
        { }

        public void GetObjectProperties(IEnumerable<string> Lines, IALObject Target)
        {
            var firstMatch = false;
            var pattern = @"\s+(.*?)\s+\=\s+(.*)\;$";
            foreach (var line in Lines)
            {
                if (Regex.IsMatch(line, pattern))
                {
                    if (!firstMatch)
                    {
                        firstMatch = true;
                    }

                    OnGetObjectProperty(line, Target, pattern);
                }
                else
                {
                    if (firstMatch)
                    {
                        break;
                    }
                }
            }
        }

        public virtual void OnGetObjectProperty(string Line, IALObject Target, string pattern)
        {
            var match = Regex.Match(Line, pattern);

            if (match.Groups.Count > 1)
            {
                var prop = new ALProperty { Name = match.Groups[1].Value, Value = match.Groups[2].Value };
                Target.Properties.Add(prop);
            }
        }

        /// <summary>
        /// Parse method of AL Object: triggers and procedures as well
        /// </summary>
        /// <param name="Lines">Array of textlines</param>
        /// <param name="Target">Current ALObject instance</param>
        public void GetMethods(IEnumerable<string> Lines, IALObject Target)
        {
            var pattern = @"(procedure|trigger)\s+(.*?)\((.*?)\)\:?(.*)";
            var procedures = Lines
                .Where(w => Regex.IsMatch(w, pattern))
                .ToList();

            Target.Methods = procedures
            .Select(s =>
            {
                var method = new ALMethod();

                method.IsLocal = s.Trim().StartsWith("local");
                var match = Regex.Match(s, pattern);
                method.MethodKind = match.Groups[1].Value;
                method.Name = match.Groups[2].Value;

                if (match.Groups.Count > 3)
                {
                    var paramTxt = match.Groups[3].Value.Trim();
                    if (!string.IsNullOrEmpty(paramTxt))
                    {
                        method.Parameters = paramTxt.Split(';').Select(s =>
                        {
                            var result = new ALParameter();
                            var parts = s.Split(':');
                            result.IsVar = parts[0].Trim().StartsWith("var ");
                            result.Name = parts[0].Replace("var ", "").Trim();
                            result.Type = parts[1];

                            return result;
                        }).ToList();
                    }
                }

                if (match.Groups.Count > 4)
                {
                    method.ReturnType = match.Groups[4].Value;
                }

                // Get Method body from var|begin to end;
                var txtLines = Lines.ToList();
                var start = txtLines.IndexOf(s) + 1;
                var nextLine = txtLines.GetRange(start, txtLines.Count() - start - 1).FirstOrDefault(f => Regex.IsMatch(f, pattern));
                var end = txtLines.IndexOf(nextLine);
                if (end == -1)
                {
                    end = Lines.Count();
                }
                var body = string.Join("\r\n", txtLines.GetRange(start, end - start - 1));
                method.Content = body;

                // Check for Test Attribute
                start = txtLines.IndexOf(s) - 1;
                var testMethod = Lines.ElementAt(start);
                if (testMethod.ToLower().Contains("test"))
                {
                    method.TestMethod = true;
                }

                return method;
            })
            .ToList();
        }

        #endregion
    }
}
