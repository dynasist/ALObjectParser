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
        public string ObjectHeaderPattern { get; set; }

        public ALObjectReaderBase()
        {
            ObjectHeaderPattern = @"\b^(codeunit|page|pageextension|pagecustomization|dotnet|enum|interface|enumextension|query|report|table|tableextension|xmlport|profile|controladdin)\s([0-9]+|.*?)\s?(.*)"; //@"^([a-z]+)\s(?(1)([0-9]+|))(.*)";
        }

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
            return ReadObjectInfos(Lines);
        }

        public IEnumerable<IALObject> ReadObjectInfos(IEnumerable<string> Lines)
        {
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

        public IEnumerable<IALObject> ReadObjectInfosFromString(string objectsStr)
        {
            var Lines = objectsStr.Split("\r\n");
            return ReadObjectInfos(Lines);
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
            where T : ALObject
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
            GetComments(Lines, Target);
            GetObjectProperties(Lines, Target);
            GetGlobalVariables(Lines, Target);
            GetSections(Lines, Target);
            GetRange(Lines, Target);
            Target.ProcessSections();
            IALObject NewTarget;
            OnRead(Lines, Target, out NewTarget);

            return NewTarget;
        }

        /// <summary>
        /// Method to implement custom processing during parsing
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="Target"></param>
        public virtual void OnRead(IEnumerable<string> Lines, IALObject Target, out IALObject NewTarget)
        {
            NewTarget = Target;
        }

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
                var endIndex = Lines.Count();
                var j = i + 1;
                if (j < c)
                {
                    endIndex = headers[j];
                }

                result.Add(contents.GetRange(startIndex, endIndex - startIndex));
            }

            return result;
        }

        public IEnumerable<string> GetObjectHeaderLines(IEnumerable<string> Lines)
        {
            var headers = Lines
                .Where(w => Regex.IsMatch(w.ToLower(), ObjectHeaderPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline));

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
            var line = Lines
                .Where(w => Regex.IsMatch(w.ToLower(), ObjectHeaderPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))
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

            if (!string.IsNullOrEmpty(line))
            {
                var items = Regex.Match(line, ObjectHeaderPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var type = items.Groups[1].Value.ToEnum<ALObjectType>();
                var idTxt = items.Groups[2].Value;

                Target = ALObjectTypeMap.CreateInstance(type);
                Target.Id = 0;
                if (!String.IsNullOrEmpty(idTxt))
                {
                    Target.Id = int.Parse(items.Groups[2].Value);
                }
                var nameParts = items.Groups[3].Value.Split(" extends ");
                if (nameParts.Length < 2)
                {
                    nameParts = items.Groups[3].Value.Split(" implements ");
                }
                Target.Name = nameParts[0].Replace("\"", "").Trim();
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

        public void GetObjectProperties(IEnumerable<string> Lines, IALSection Target)
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

        public virtual void OnGetObjectProperty(string Line, IALSection Target, string pattern)
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
        public void GetMethods(IEnumerable<string> Lines, IALSection Target)
        {
            var pattern = @"^\s{0,4}(local procedure|procedure|trigger)\s+(.*?)\((.*?)\)\:?(.*)";
            var procedures = Lines
                .Where(w => Regex.IsMatch(w, pattern))
                .ToList();

            if (procedures.Count() == 0)
            {
                return;
            }

            Target.Methods = procedures
            .Select(s =>
            {
                var method = new ALMethod();

                method.IsLocal = s.Trim().StartsWith("local");
                var match = Regex.Match(s, pattern);
                switch (match.Groups[1].Value.ToLower())
                {
                    case "procedure":
                        method.MethodKind = ALMethodKind.Method;
                        break;
                    case "trigger":
                        method.MethodKind = ALMethodKind.Trigger;
                        break;
                }
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
                            result.TypeDefinition = new ALTypeDefinition { Name = parts[1] };

                            return result;
                        }).ToList();
                    }
                }

                if (match.Groups.Count > 4)
                {
                    method.ReturnTypeDefinition = new ALReturnTypeDefinition { Name = match.Groups[4].Value };
                }

                // Get Method body from var|begin to end;
                var txtLines = Lines.ToList();
                var start = txtLines.IndexOf(s) + 1;
                var nextLine = txtLines.GetRange(start, txtLines.Count() - start - 1).FirstOrDefault(f => Regex.IsMatch(f, pattern));
                var end = txtLines.IndexOf(nextLine);
                string beginPattern = @"^\s{0,4}(begin)\s*$";
                string beginText = txtLines.GetRange(start, txtLines.Count() - start - 1).FirstOrDefault(f => Regex.IsMatch(f, beginPattern));
                if (!string.IsNullOrEmpty(beginText))
                {
                    if (end == -1)
                    {
                        end = Lines.Count();
                    }

                    var bodyLines = txtLines.GetRange(start, end - start - 1);
                    var body = string.Join("\r\n", bodyLines);
                    var comments = GetComments(bodyLines);
                    method.MethodBody = new ALMethodBody
                    {
                        Comments = comments,
                        Content = body,
                        ContentLines = bodyLines
                    };

                    method.Content = body;
                }
                else
                {
                    method.IsMethodDeclaration = true;
                }
                method.Attributes = new List<ALAttribute>();

                // Check for Attributes
                end = txtLines.IndexOf(s) - 1;
                start = txtLines.IndexOf(s) - 1;
                var testMethod = Lines.ElementAt(start);
                while (testMethod.Trim() != String.Empty &&
                       !testMethod.ToLower().Contains("end;") &&
                       !testMethod.ToLower().Contains("{"))
                {
                    if (testMethod.ToLower().Contains("[test"))
                    {
                        method.TestMethod = true;
                        //break;
                    }

                    var attrMatch = Regex.Match(testMethod, @"\[(\w+)(.*)\]");
                    if (attrMatch.Success)
                    {
                        var attr = new ALAttribute
                        {
                            Name = attrMatch.Groups[1].Value,
                            Content = testMethod.Trim()
                        };
                        attr.IsEvent = attr.Name.ToLower().Contains("event");
                        method.Attributes.Add(attr);
                    }

                    start -= 1;
                    if (start < 0)
                    {
                        break;
                    }
                    testMethod = Lines.ElementAt(start);
                }

                start += 1;
                end += 1;
                var linesAboveMethod = txtLines
                    .GetRange(start, end - start)
                    .Select(s => s.Trim())
                    .Where(w => !w.StartsWith("["));
                //.Where(w => w.StartsWith("//") || w.StartsWith("/*") || w.EndsWith("*/"));
                var txtAboveMethod = string.Join("\r\n", linesAboveMethod);
                txtAboveMethod = txtAboveMethod
                    .Replace("///", "")
                    .Replace("//", "")
                    .Replace("/*", "")
                    .Replace("*/", "");

                var methodComments = new List<ALComment>();
                if (!string.IsNullOrEmpty(txtAboveMethod))
                {
                    methodComments.Add(new ALComment
                    {
                        Content = txtAboveMethod.Trim(),
                        StartPos = start,
                        EndPos = end
                    });
                }

                method.Comments = methodComments;

                return method;
            })
            .ToList();
        }

        public void GetComments(IEnumerable<string> Lines, IALSection Target)
        {
            var comments = GetComments(Lines);
            Target.Comments = comments;
        }

        public ICollection<ALComment> GetComments(IEnumerable<string> Lines)
        {
            var pattern = @"\/\/";
            var comments = new List<ALComment>();
            var c = Lines.Count();

            for (int i = 0; i < c; i++)
            {
                var line = Lines.ElementAt(i);
                if (Regex.IsMatch(line, pattern))
                {

                    var comment = new ALComment
                    {
                        Content = line,
                        StartPos = i,
                        EndPos = i
                    };
                    comments.Add(comment);
                }
            }

            return comments;
        }
        public void GetRange(IEnumerable<string> Lines, IALObject Target)
        {
            string contents = String.Join("__", Lines);
            foreach (ALMethod method in Target.Methods)
            {
                Match match = Regex.Match(contents, $"(.*)(local)? (procedure|trigger) {method.Name}\\(", RegexOptions.IgnoreCase);
                if (!match.Success)
                    continue;
                string textBeforeProcedure = contents.Substring(0, match.Groups[1].Length);
                int startLineNo = (textBeforeProcedure.Length - textBeforeProcedure.Replace("__", "").Length) / 2;
                string pattern = ".{" + textBeforeProcedure.Length + "}.+?__    end;";
                match = Regex.Match(contents, pattern, RegexOptions.IgnoreCase);
                if (!match.Success)
                    continue;
                string textUntilEndOfProcedure = contents.Substring(0, match.Groups[0].Length);
                int endLineNo = (textUntilEndOfProcedure.Length - textUntilEndOfProcedure.Replace("__", "").Length) / 2;
                method.MethodRange = new Range(startLineNo, endLineNo);
            }
        }

        public void GetSections(IEnumerable<string> Lines, IALSection Target)
        {
            var contents = String.Join("\r\n", Lines);
            var sections = GetSections(contents);
            Target.Sections = sections;
        }

        public ICollection<IALSection> GetSections(string Lines)
        {
            var result = new List<IALSection>();
            var matches = Regex.Matches(Lines, @"(.*?)\s+\{((?:[^{}]|(?<counter>\{)|(?<-counter>\}))+(?(counter)(?!)))\}");

            foreach (Match match in matches)
            {
                var contents = match.Groups[2].Value;
                var section = new ALSection();
                section.Name = match.Groups[1].Value.Trim();
                section.TextContent = contents;
                section.Sections = GetSections(contents);

                var subContents = contents;
                section.Sections.ToList().ForEach(f =>
                {
                    subContents = subContents.Replace(f.TextContent, "");
                });

                var lines = subContents.Split("\r\n");
                GetObjectProperties(lines, section);
                GetMethods(lines, section);
                result.Add(section);
            }

            return result;
        }

        public void GetGlobalVariables(IEnumerable<string> Lines, IALObject Target)
        {
            var result = new List<ALVariable>();
            var patternVar = @"var";
            var selectedLine = 0;
            var c = Lines.Count();

            for (int i = 0; i < c; i++)
            {
                var line = Lines.ElementAt(i);
                if (line.Trim() == patternVar)
                {
                    if (!Lines.ElementAt(i - 1).Contains("procedure"))
                    {
                        selectedLine = i;
                        break;
                    }
                }
            }

            selectedLine += 1;
            for (int i = selectedLine; i < c; i++)
            {
                var line = Lines.ElementAt(i);
                var parts = line.Split(":");
                if (parts.Count() < 2)
                {
                    break;
                }

                var variable = new ALVariable
                {
                    Name = parts[0].Trim(),
                    TypeDefinition = new ALTypeDefinition { Name = parts[1].Trim() }
                };

                result.Add(variable);
            }

            Target.GlobalVariables = result;
        }

        #endregion
    }
}
